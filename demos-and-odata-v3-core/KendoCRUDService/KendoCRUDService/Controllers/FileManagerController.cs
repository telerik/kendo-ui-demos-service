using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.FileBrowser;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace KendoCRUDService.Controllers
{
    public class FileManagerController : Controller
    {
        private const string contentFolderRoot = "/Content/";
        private const string prettyName = "Files/";
        private static readonly string[] foldersToCopy = new[] { "/Content/filemanager/" };
        private const string DefaultFilter = "*.txt,*.docx,*.xlsx,*.ppt,*.pptx,*.zip,*.rar,*.jpg,*.jpeg,*.gif,*.png";

        private readonly DirectoryRepository _directoryRepository;
        private readonly ContentInitializer contentInitializer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileManagerController(DirectoryRepository directoryRepository, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
            _directoryRepository = directoryRepository;
            contentInitializer = new ContentInitializer(_httpContextAccessor, contentFolderRoot, foldersToCopy, prettyName);
        }

        public string ContentPath
        {
            get
            {
                return contentInitializer.CreateUserFolder(_hostingEnvironment.ContentRootPath);
            }
        }

        private string ToAbsolute(string virtualPath)
        {
            return HttpContext.Request.PathBase + virtualPath;
        }

        private string ToVirtual(string path)
        {
            return path.Replace(Path.Combine(_hostingEnvironment.ContentRootPath, contentFolderRoot), "").Replace(@"\", "/");
        }

        private string CombinePaths(string basePath, string relativePath)
        {
            return basePath + "/" + relativePath;
        }

        public virtual bool AuthorizeRead(string path)
        {
            return CanAccess(path);
        }

        protected virtual bool CanAccess(string path)
        {
            var appRoothPath = _hostingEnvironment.ContentRootPath;

            return Path.Combine(appRoothPath, path).StartsWith(Path.Combine(appRoothPath, ContentPath), StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ToAbsolute(ContentPath);
            }

            return CombinePaths(ToAbsolute(ContentPath), path);
        }

        public virtual IActionResult Read(string target)
        {
            var path = NormalizePath(target);

            if (AuthorizeRead(path))
            {
                try
                {
                    var result = _directoryRepository
                        .GetContent(path, DefaultFilter)
                        .Select(f => new
                        {
                            name = f.Name,
                            size = f.Size,
                            path = ToVirtual(f.Path),
                            extension = f.Extension,
                            isDirectory = f.IsDirectory,
                            hasDirectories = f.HasDirectories,
                            created = f.Created,
                            createdUtc = f.CreatedUtc,
                            modified = f.Modified,
                            modifiedUtc = f.ModifiedUtc
                        });

                    return Json(result);
                }
                catch (DirectoryNotFoundException)
                {
                    return new ObjectResult("File Not Found") { StatusCode = 404};
                }
            }

            return new ObjectResult("Forbidden") { StatusCode = 403 };
        }

        [HttpPost]
        public virtual ActionResult Destroy(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);

            if (!string.IsNullOrEmpty(path))
            {
                if (!AuthorizeDelete(path))
                {
                    return new ObjectResult("Forbidden");
                }

                if (entry.IsDirectory)
                {
                    DeleteDirectory(path);
                }
                else
                {
                    DeleteFile(path);
                }

                return Json(new object[0]);
            }
            return new ObjectResult("File Not Found") { StatusCode = 404};
        }

        public virtual bool AuthorizeDelete(string path)
        {
            return CanAccess(path);
        }

        protected virtual void DeleteFile(string path)
        {
            var physicalPath = _hostingEnvironment.ContentRootPath;

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        protected virtual void DeleteDirectory(string path)
        {
            var physicalPath = _hostingEnvironment.ContentRootPath;

            if (Directory.Exists(physicalPath))
            {
                Directory.Delete(physicalPath, true);
            }
        }

        public virtual bool Authorize(string path)
        {
            return CanAccess(path);
        }

        [HttpPost]
        public virtual ActionResult Create(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(target)))
            {
                return new ObjectResult("Forbidden") { StatusCode = 403};
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


            return Json(new
            {
                name = newEntry.Name,
                size = newEntry.Size,
                path = ToVirtual(newEntry.Path),
                extension = newEntry.Extension,
                isDirectory = newEntry.IsDirectory,
                hasDirectories = newEntry.HasDirectories,
                created = newEntry.Created,
                createdUtc = newEntry.CreatedUtc,
                modified = newEntry.Modified,
                modifiedUtc = newEntry.ModifiedUtc
            });
        }

        private FileManagerEntry CopyEntry(string target, FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
            var physicalTarget = EnsureUniqueName(NormalizePath(target), entry);

            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                CopyDirectory(new DirectoryInfo(physicalPath), Directory.CreateDirectory(physicalTarget));
                newEntry = _directoryRepository.GetDirectory(physicalTarget);
            }
            else
            {
                System.IO.File.Copy(physicalPath, physicalTarget);
                newEntry = _directoryRepository.GetFile(physicalTarget);
            }

            return newEntry;
        }

        private void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }

        private string EnsureUniqueName(string target, FileManagerEntry entry)
        {
            var tempName = entry.Name + entry.Extension;
            int sequence = 0;
            var physicalTarget = NormalizePath(Path.Combine(target, entry.Name + entry.Extension));

            physicalTarget = Path.Combine(_hostingEnvironment.ContentRootPath, physicalTarget);

            if (entry.IsDirectory)
            {
                while (Directory.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence);
                    physicalTarget = Path.Combine(Path.Combine(_hostingEnvironment.ContentRootPath, target), tempName);
                }
            }
            else
            {
                while (System.IO.File.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence) + entry.Extension;
                    physicalTarget = Path.Combine(Path.Combine(_hostingEnvironment.ContentRootPath, target), tempName);
                }
            }

            return physicalTarget;
        }

        private FileManagerEntry CreateNewFolder(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;
            var path = NormalizePath(target);
            string physicalPath = EnsureUniqueName(path, entry);

            Directory.CreateDirectory(physicalPath);

            newEntry = _directoryRepository.GetDirectory(physicalPath);

            return newEntry;
        }

        [HttpPost]
        public virtual ActionResult Update(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(entry.Path)) && !Authorize(NormalizePath(target)))
            {
                return new ObjectResult("Forbidden") { StatusCode = 403};
            }

            if (!Authorize(NormalizePath(Path.Combine(target, entry.Name + entry.Extension))))
            {
                throw new Exception("Forbidden");
            }

            newEntry = RenameEntry(entry);

            return Json(new
            {
                name = newEntry.Name,
                size = newEntry.Size,
                path = ToVirtual(newEntry.Path),
                extension = newEntry.Extension,
                isDirectory = newEntry.IsDirectory,
                hasDirectories = newEntry.HasDirectories,
                created = newEntry.Created,
                createdUtc = newEntry.CreatedUtc,
                modified = newEntry.Modified,
                modifiedUtc = newEntry.ModifiedUtc
            });
        }

        private FileManagerEntry RenameEntry(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = Path.Combine(_hostingEnvironment.ContentRootPath, path);
            var physicalTarget = EnsureUniqueName(Path.GetDirectoryName(path), entry);
            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                Directory.Move(physicalPath, physicalTarget);
                newEntry = _directoryRepository.GetDirectory(physicalTarget);
            }
            else
            {
                var file = new FileInfo(physicalPath);
                System.IO.File.Move(file.FullName, physicalTarget);
                newEntry = _directoryRepository.GetFile(physicalTarget);
            }

            return newEntry;
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

        [HttpPost]
        public virtual ActionResult Upload(string path, IFormFile file)
        {
            path = NormalizePath(path);
            var fileName = Path.GetFileName(file.FileName);

            if (AuthorizeUpload(path, file))
            {
                string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, fileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                return Json(new
                {
                    size = file.Length,
                    name = fileName,
                    type = "f"
                }, "text/plain");
            }

            return new ObjectResult("Forbidden") { StatusCode = 403};
        }

        public virtual bool AuthorizeFile(string path)
        {
            return CanAccess(path) && IsValidFile(Path.GetExtension(path));
        }
    }
}
