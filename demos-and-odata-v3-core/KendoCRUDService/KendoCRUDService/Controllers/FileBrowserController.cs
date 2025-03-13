using KendoCRUDService.FileBrowser;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Net.Mail;

namespace KendoCRUDService.Controllers
{
    public class FileBrowserController : Controller
    {
        private const string contentFolderRoot = "Content\\";
        private const string prettyName = "Images\\";
        private static readonly string[] foldersToCopy = new[] { "Content\\editor" };
        private const string DefaultFilter = "*.txt,*.doc,*.docx,*.xls,*.xlsx,*.ppt,*.pptx,*.zip,*.rar,*.jpg,*.jpeg,*.gif,*.png";

        private readonly DirectoryBrowser directoryBrowser;
        private readonly ContentInitializer contentInitializer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileBrowserController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment, ContentInitializer initializer)
        {
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
            directoryBrowser = new DirectoryBrowser();
            contentInitializer = initializer;
            initializer.prettyName = prettyName;
            initializer.foldersToCopy = foldersToCopy;
            initializer.rootFolder = contentFolderRoot;
            contentInitializer.prettyName = prettyName;
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
            return path.StartsWith(ToAbsolute(ContentPath), StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ToAbsolute(ContentPath);
            }

            return CombinePaths(ToAbsolute(ContentPath), path.Replace(@"/", "\\")).Replace(@"/", string.Empty);
        }

        public virtual IActionResult Read(string path)
        {
            path = NormalizePath(path);

            if (AuthorizeRead(path))
            {
                try
                {
                    var result = directoryBrowser
                        .GetContent(_hostingEnvironment.ContentRootPath, path, DefaultFilter)
                        .Select(f => new
                        {
                            name = f.Name,
                            type = f.Type == EntryType.File ? "f" : "d",
                            size = f.Size
                        });

                    return Json(result);
                }
                catch (DirectoryNotFoundException)
                {
                    return new ObjectResult("File Not Found") { StatusCode = 404 };
                }
            }

            return new ObjectResult("Forbidden") { StatusCode = 403 };
        }

        [HttpPost]
        public virtual IActionResult Destroy(string path, string name, string type)
        {
            path = NormalizePath(path);

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
            {
                path = CombinePaths(path, name);

                if (!AuthorizeDelete(path))
                {
                    return new ObjectResult("Forbidden") { StatusCode = 403 };
                }

                if (type.ToLowerInvariant() == "f")
                {

                    DeleteFile(path);
                }
                else
                {
                    DeleteDirectory(path);
                }

                return Json(new object[0]);
            }

            return new ObjectResult("File Not Found") { StatusCode = 404 };
        }

        public virtual bool AuthorizeDelete(string path)
        {
            return CanAccess(path);
        }

        protected virtual void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        protected virtual void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public virtual bool AuthorizeCreateDirectory(string path, string name)
        {
            return CanAccess(path);
        }

        [HttpPost]
        public virtual ActionResult Create(string path, FileBrowserEntry entry)
        {
            path = NormalizePath(path);
            var name = entry.Name;

            if (!string.IsNullOrEmpty(name) && AuthorizeCreateDirectory(path, name))
            {
                var physicalPath = Path.Combine(path, name);

                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }

                return Json(new
                {
                    name = entry.Name,
                    type = "d",
                    size = entry.Size
                });
            }

            return new ObjectResult("Forbidden") { StatusCode = 403 };
        }


        public virtual bool AuthorizeUpload(string path, IFormFile file)
        {
            return CanAccess(path) && IsValidFile(file.FileName);
        }

        private bool IsValidFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var allowedExtensions = DefaultFilter.Split(',');

            return allowedExtensions.Any(e => e.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        [HttpPost]
        public virtual IActionResult Upload(string path, IFormFile file)
        {
            path = NormalizePath(path);
            var fileName = Path.GetFileName(file.FileName);

            if (AuthorizeUpload(path, file))
            {
                string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Move the file to the user target folder
                var savedFile = new FileInfo(filePath);
                string newPath = Path.Combine(path, fileName);
                System.IO.File.Move(savedFile.FullName, newPath);

                return Json(new
                {
                    size = file.Length,
                    name = fileName,
                    type = "f"
                });
            }

            return new ObjectResult("Forbidden") { StatusCode = 403};
        }

        [OutputCache(Duration = 360, VaryByQueryKeys = new string[] { "path" })]
        public IActionResult File(string fileName)
        {
            var path = NormalizePath(fileName);

            if (AuthorizeFile(path))
            {
                var physicalPath = Path.Combine(_hostingEnvironment.ContentRootPath, path);

                if (System.IO.File.Exists(physicalPath))
                {
                    const string contentType = "application/octet-stream";
                    return File(System.IO.File.OpenRead(physicalPath), contentType, fileName);
                }
            }

            return new ObjectResult("Forbidden") { StatusCode = 403};
        }

        public virtual bool AuthorizeFile(string path)
        {
            return CanAccess(path) && IsValidFile(Path.GetExtension(path));
        }
    }
}
