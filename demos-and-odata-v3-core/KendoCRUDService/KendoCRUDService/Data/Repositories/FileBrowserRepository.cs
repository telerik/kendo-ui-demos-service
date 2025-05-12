using KendoCRUDService.Data.Models;
using KendoCRUDService.FileBrowser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Collections.Concurrent;
using System.Net.Mime;

namespace KendoCRUDService.Data.Repositories
{
    public class FileBrowserRepository
    {
        public string ContentRootPath { get; set; }
        public string DefaultFilter { get; set; }
        public string ContentPath
        {
            get
            {
                return Path.Combine(hostingEnvironment.ContentRootPath, ContentRootPath);
            }
        }
        private ConcurrentDictionary<string, List<FileBrowserEntry>> _entries;
        private Dictionary<string, byte[]> _files;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment hostingEnvironment;
        private ThumbnailCreator thumbnailCreator;


        public FileBrowserRepository(IWebHostEnvironment _hostingEnvironment, IHttpContextAccessor contextAccessor)
        {
            hostingEnvironment = _hostingEnvironment;
            _contextAccessor = contextAccessor;
            _files = new Dictionary<string, byte[]>();
            _entries = new ConcurrentDictionary<string, List<FileBrowserEntry>>();
            thumbnailCreator = new ThumbnailCreator();
        }

        private List<FileBrowserEntry> Content()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            List<FileBrowserEntry> entries = new List<FileBrowserEntry>();
            _entries.TryGetValue(SessionUtils.GetUserKey(_contextAccessor), out entries);

            return entries;
        }

        public IEnumerable<FileBrowserEntry> GetContent(string path, string filter)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            var entries = _entries.GetOrAdd(userKey, key =>
            {
                return GetAll(ContentPath, filter).ToList();
            });

            var virtualized = entries.Where(d => TargetMatch(path, d.Path)).Select(VirtualizePath);

            return entries.Where(d => TargetMatch(path, d.Path)).Select(VirtualizePath).ToList();
        }

        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ContentPath;
            }
            else
            {
                return Path.Combine(ContentPath, path);
            }
        }

        private bool TargetMatch(string target, string path)
        {
            var targetFullPath = Path.Combine(ContentPath, NormalizePath(target));
            var parentPath = Directory.GetParent(path).FullName;

            return targetFullPath.Trim('\\') == parentPath.Trim('\\');
        }

        protected virtual FileBrowserEntry VirtualizePath(FileBrowserEntry entry)
        {
            return new FileBrowserEntry
            {
                Name = entry.Name,
                Path = entry.Path.Replace(ContentPath, "").Replace(@"\", "/"),
                Size = entry.Size,
                Type = entry.Type
            };
        }

        private IEnumerable<FileBrowserEntry> GetFiles(string path, string filter)
        {
            var directory = new DirectoryInfo(Path.Combine(ContentPath, path));

            var extensions = (filter ?? "*").Split(",|;".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

            return extensions.SelectMany(directory.GetFiles)
                .Select(file => new FileBrowserEntry
                {
                    Name = Path.GetFileName(file.Name),
                    Size = file.Length,
                    Path = file.FullName,
                    Type = EntryType.File
                });
        }

        public ICollection<FileBrowserEntry> GetAll(string path, string filter)
        {
            var directory = new DirectoryInfo(path);
            var directories = directory.GetDirectories();
            var files = GetFiles(path, filter);
            var result = new List<FileBrowserEntry>();


            foreach (var item in files)
            {
                result.Add(item);
            }

            foreach (var item in directories)
            {
                result.Add(new FileBrowserEntry
                {
                    Name = item.Name,
                    Path = item.FullName,
                    Type = EntryType.Directory
                });

                if (item.GetDirectories().Length > 0 || item.GetFiles().Length > 0)
                {
                    result.AddRange(GetAll(item.FullName, filter));
                }
            }

            return result;
        }

        public void Destroy(string path, string name, string type)
        {
            var currentEntries = Content();
            var normalizedPath = NormalizePath((path??"") + name);
            
            currentEntries.RemoveAll(x => x.Path.Contains(normalizedPath));

            UpdateContent(currentEntries);
        }

        public byte[] Download(string path)
        {
            var currentEntries = Content();
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            var normalizedPath = NormalizePath((path ?? ""));
            if (_files.ContainsKey(userKey + normalizedPath))
            {
                return _files[userKey + normalizedPath];
            }
            else if (File.Exists(normalizedPath))
            {
                return new MemoryStream(File.ReadAllBytes(normalizedPath)).ToArray();
            }
            throw new Exception("File Not Found!");
        }

        public void Upload(string path, IFormFile file)
        {
            var currentEntries = Content();
            var fileName = Path.GetFileName(file.FileName);
            var directory = NormalizePath(path ?? "");
            var normalizedPath = directory + fileName;
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            
            if (AuthorizeUpload(directory, file))
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    _files.Add(userKey + normalizedPath, ms.ToArray());
                }

                FileBrowserEntry newEntry = new FileBrowserEntry
                {
                    Name = fileName,
                    Path = normalizedPath,
                    Size = file.Length,
                    Type = EntryType.File
                };

                currentEntries.Add(newEntry);

                UpdateContent(currentEntries);
                return;
            }
            throw new Exception("Forbidden");
        }

        private void UpdateContent(List<FileBrowserEntry> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _entries[userKey] = entries;
        }

        public void Create(string path, FileBrowserEntry entry)
        {
            path = NormalizePath(path);
            var currentEntries = Content();

            FileBrowserEntry newEntry = new FileBrowserEntry
            {
                Name = entry.Name,
                Path = path + entry.Name,
                Size = entry.Size,
                Type = EntryType.Directory
            };

            currentEntries.Add(newEntry);
            UpdateContent(currentEntries);
        }

        public virtual bool AuthorizeUpload(string path, IFormFile file)
        {
            if (!CanAccess(path))
            {
                throw new DirectoryNotFoundException(String.Format("The specified path cannot be found - {0}", path));
            }

            if (!IsValidFile(file.FileName))
            {
                throw new InvalidDataException(String.Format("The type of file is not allowed. Only {0} extensions are allowed.", DefaultFilter));
            }

            return true;
        }

        protected virtual bool CanAccess(string path)
        {
            return Content().Select(x => x.Path == path).Count() > 0;
        }

        private bool IsValidFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var allowedExtensions = DefaultFilter.Split(',');

            return allowedExtensions.Any(e => e.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        public byte[] CreateThumbnail(string physicalPath)
        {
           var desiredSize = new ImageSize
           {
               Width = 80,
               Height = 80
           };
           const string contentType = "image/png";
           return thumbnailCreator.Create(Download(physicalPath), desiredSize, contentType);
        }
    }
}
