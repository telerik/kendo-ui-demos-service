using KendoCRUDService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class DirectoryRepository
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
        private ConcurrentDictionary<string, List<FileManagerEntry>> _directories;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment hostingEnvironment;

        public DirectoryRepository(IWebHostEnvironment _hostingEnvironment, IHttpContextAccessor contextAccessor)
        {
            hostingEnvironment = _hostingEnvironment;
            _contextAccessor = contextAccessor;
            _directories = new ConcurrentDictionary<string, List<FileManagerEntry>>();
        }

        public ICollection<FileManagerEntry> GetAll(string path, string filter)
        {
            var directory = new DirectoryInfo(path);
            var directories = directory.GetDirectories();
            var files = GetFiles(path,filter);
            var result = new List<FileManagerEntry>();


            foreach (var item in files)
            {
                result.Add(item);
            }

            foreach (var item in directories)
            {
                result.Add(GetDirectory(item.FullName));

                if (item.GetDirectories().Length > 0 || item.GetFiles().Length > 0)
                {
                    result.AddRange(GetAll(item.FullName, filter));
                }
            }

            return result;
        }

        private bool TargetMatch(string target, string path)
        {
            var targetFullPath = Path.Combine(ContentPath, NormalizePath(target));
            var parentPath = Directory.GetParent(path).FullName;

            return targetFullPath.Trim('\\') == parentPath.Trim('\\');
        }

        protected virtual FileManagerEntry VirtualizePath(FileManagerEntry entry)
        {
            return new FileManagerEntry
            {
                Created = entry.Created,
                CreatedUtc = entry.CreatedUtc,
                Extension = entry.Extension,
                HasDirectories = entry.HasDirectories,
                IsDirectory = entry.IsDirectory,
                Modified = entry.Modified,
                ModifiedUtc = entry.ModifiedUtc,
                Name = entry.Name,
                Path = entry.Path.Replace(ContentPath, "").Replace(@"\", "/"),
                Size = entry.Size
            };
        }

        public IEnumerable<FileManagerEntry> GetContent(string path, string filter)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            //path = path.Replace(@"\", "\\").Replace(@"/", "\\");

            var entries = _directories.GetOrAdd(userKey, key =>
            {
                return GetAll(ContentPath, filter).ToList();
            });

            var virtualized = entries.Where(d => TargetMatch(path, d.Path)).Select(VirtualizePath);

            return entries.Where(d => TargetMatch(path, d.Path)).Select(VirtualizePath).ToList();
        }

        private IEnumerable<FileManagerEntry> GetFiles(string path, string filter)
        {
            var directory = new DirectoryInfo(Path.Combine(ContentPath, path));

            var extensions = (filter ?? "*").Split(",|;".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

            return extensions.SelectMany(directory.GetFiles)
                .Select(file => new FileManagerEntry
                {
                    Name = Path.GetFileNameWithoutExtension(file.Name),
                    Size = file.Length,
                    Path = file.FullName,
                    Extension = file.Extension,
                    IsDirectory = false,
                    HasDirectories = false,
                    Created = file.CreationTime,
                    CreatedUtc = file.CreationTimeUtc,
                    Modified = file.LastWriteTime,
                    ModifiedUtc = file.LastWriteTimeUtc
                });
        }

        public  virtual FileManagerEntry RenameEntry(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);//.Replace(@"/", "\\")
            var physicalPath = path;
            var physicalTarget = EnsureUniqueName(Path.GetDirectoryName(path), entry);
            var currentEntries = Content();

            foreach (var item in currentEntries)
            {

                if (item.Path.Contains(path))
                {
                    item.Path = item.Path.Replace(path, physicalTarget);
                }
            }

            FileManagerEntry currentEntry = currentEntries.FirstOrDefault(x => x.Path == physicalTarget);

            currentEntry.Name = entry.Name;
            UpdateContent(currentEntries);

            return VirtualizePath(currentEntry);
        }

        public FileManagerEntry GetDirectory(string path)
        {
            var directory = new DirectoryInfo(path);

            return new FileManagerEntry
            {
                Name = directory.Name,
                Path = directory.FullName,
                Extension = directory.Extension,
                IsDirectory = true,
                HasDirectories = directory.GetDirectories().Length > 0,
                Created = directory.CreationTime,
                CreatedUtc = directory.CreationTimeUtc,
                Modified = directory.LastWriteTime,
                ModifiedUtc = directory.LastWriteTimeUtc
            };
        }

        public FileManagerEntry GetFile(string path)
        {
            var file = new FileInfo(path);

            return new FileManagerEntry
            {
                Name = Path.GetFileNameWithoutExtension(file.Name),
                Path = file.FullName,
                Size = file.Length,
                Extension = file.Extension,
                IsDirectory = false,
                HasDirectories = false,
                Created = file.CreationTime,
                CreatedUtc = file.CreationTimeUtc,
                Modified = file.LastWriteTime,
                ModifiedUtc = file.LastWriteTimeUtc
            };
        }

        public virtual bool AuthorizeFile(string path)
        {
            return CanAccess(path) && IsValidFile(Path.GetExtension(path));
        }

        public virtual bool AuthorizeDelete(string path)
        {
            return CanAccess(path);
        }

        public virtual bool Authorize(string path)
        {
            return CanAccess(path);
        }

        public void Destroy(FileManagerEntry entry)
        {
            var currentEntries = Content();
            var path = NormalizePath(entry.Path);//.Replace(@"/", "\\")

            currentEntries.RemoveAll(x => x.Path.Contains(path));

            UpdateContent(currentEntries);
        }

        public FileManagerEntry Create(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(target)))
            {
                throw new Exception("Forbidden");
            }

            if (!Authorize(NormalizePath(Path.Combine(target, entry.Name + entry.Extension))))
            {
                throw new Exception("Forbidden");
            }

            if (String.IsNullOrEmpty(entry.Path))
            {
                newEntry = CreateNewFolder(target, entry);
            }
            else
            {
                newEntry = CopyEntry(target, entry);
            }

            return newEntry;
        }

        public FileManagerEntry Upload(string path, IFormFile file)
        {
            var currentEntries = Content();

            FileManagerEntry newEntry = new FileManagerEntry();
            path = NormalizePath(path);//.Replace(@"/", "\\")
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);

            if (AuthorizeUpload(path, file))
            {
                newEntry.Path = Path.Combine(path, file.FileName);
                newEntry.Name = fileName;
                newEntry.Modified = DateTime.Now;
                newEntry.ModifiedUtc = DateTime.Now;
                newEntry.Created = DateTime.Now;
                newEntry.CreatedUtc = DateTime.UtcNow;
                newEntry.Size = file.Length;
                newEntry.Extension = Path.GetExtension(file.FileName);
                currentEntries.Add(newEntry);

                UpdateContent(currentEntries);
            } else
            {
                throw new Exception("Forbidden");
            }

            return VirtualizePath(newEntry);
        }

        public FileManagerEntry CreateNewFolder(string target, FileManagerEntry entry)
        {
            var currentEntries = Content();

            var path = NormalizePath((target??""));//.Replace("/", "\\")
            string physicalPath = EnsureUniqueName(path, entry);

            entry.Path = physicalPath;
            entry.Created = DateTime.Now;
            entry.CreatedUtc = DateTime.UtcNow;
            entry.Modified = DateTime.Now;
            entry.ModifiedUtc = DateTime.UtcNow;
            currentEntries.Add(entry);

            UpdateContent(currentEntries);

            return VirtualizePath(entry);
        }

        protected virtual string EnsureUniqueName(string target, FileManagerEntry entry)
        {
            var tempName = entry.Name + entry.Extension;
            int sequence = 0;
            var physicalTarget = Path.Combine(NormalizePath(target), tempName);

            if (!Authorize(NormalizePath(physicalTarget)))
            {
                throw new Exception("Forbidden");
            }

            if (entry.IsDirectory)
            {
                while (Directory.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence);
                    physicalTarget = Path.Combine(NormalizePath(target), tempName);
                }
            }
            else
            {
                while (System.IO.File.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence) + entry.Extension;
                    physicalTarget = Path.Combine(NormalizePath(target), tempName);
                }
            }

            return physicalTarget;
        }

        public virtual FileManagerEntry CopyEntry(string target, FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = path;
            var physicalTarget = EnsureUniqueName(NormalizePath(target), entry);

            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                var newDirectory = Directory.CreateDirectory(physicalTarget);
                newEntry = CopyDirectory(new DirectoryInfo(physicalPath), newDirectory);
                newDirectory.Delete();
            }
            else
            {
                newEntry = CopyFile(physicalPath, physicalTarget);
            }

            return newEntry;
        }

        private FileManagerEntry CopyFile(string source, string target)
        {
            var currentEntries = Content();
            var entry = currentEntries.FirstOrDefault(x => x.Path == source);

            var newEntry = entry.Clone();
            newEntry.Path = target;
            currentEntries.Add(newEntry);

            UpdateContent(currentEntries);

            return entry;
        }

        public FileManagerEntry CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            var currentEntries = Content();
            var currentEntry = currentEntries.FirstOrDefault(x => x.Path == source.FullName);

            var newEntry = currentEntry.Clone();
            newEntry.Path = target.FullName;
            currentEntries.Add(newEntry);

            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                if (IsValidFile(fi.FullName)) 
                {
                    CopyFile(fi.FullName, Path.Combine(target.FullName, fi.Name));
                }
                
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
                target.Delete();
            }

            UpdateContent(currentEntries);

            return currentEntry;
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

        private bool IsValidFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var allowedExtensions = DefaultFilter.Split(',');

            return allowedExtensions.Any(e => e.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        private List<FileManagerEntry> Content() 
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            List<FileManagerEntry> entries = new List<FileManagerEntry>();
            _directories.TryGetValue(SessionUtils.GetUserKey(_contextAccessor), out entries);

            return entries;
        }

        private void UpdateContent(List<FileManagerEntry> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _directories[userKey] = entries;
        }

        protected virtual bool CanAccess(string path)
        {
            return Content().Select(x=>x.Path == path).Count() > 0;
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
    }
}
