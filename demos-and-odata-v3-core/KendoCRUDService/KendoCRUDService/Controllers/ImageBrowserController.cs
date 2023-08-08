using KendoCRUDService.Data.Repositories;
using KendoCRUDService.FileBrowser;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.FileProviders;
using System.Net.Mail;

namespace KendoCRUDService.Controllers
{
    public class ImageBrowserController : Controller
    {
        private const string contentFolderRoot = "~/Content/";
        private const string prettyName = "Images/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/editor/" };
        private const string DefaultFilter = "*.png,*.gif,*.jpg,*.jpeg";

        private const int ThumbnailHeight = 80;
        private const int ThumbnailWidth = 80;

        private readonly DirectoryBrowser directoryBrowser;
        private readonly ContentInitializer contentInitializer;
        private readonly ThumbnailCreator thumbnailCreator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ImageBrowserController(DirectoryRepository directoryRepository, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
            directoryBrowser = new DirectoryBrowser();
            contentInitializer = new ContentInitializer(_httpContextAccessor, contentFolderRoot, foldersToCopy, prettyName);
            thumbnailCreator = new ThumbnailCreator();
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

            return CombinePaths(ToAbsolute(ContentPath), path);
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


        public virtual bool AuthorizeThumbnail(string path)
        {
            return CanAccess(path);
        }

        [OutputCache(Duration = 3600, VaryByQueryKeys = new string[]{ "path"})]
        public virtual ActionResult Thumbnail(string path)
        {
            path = NormalizePath(path);

            if (AuthorizeThumbnail(path))
            {
                var physicalPath =  MapPath(path);

                if (System.IO.File.Exists(physicalPath))
                {
                    var fileInfo = new FileInfo(physicalPath);
                    Response.GetTypedHeaders().LastModified = fileInfo.LastWriteTime;

                    return CreateThumbnail(physicalPath);
                }
                else
                {
                    return new ObjectResult("File Not Found") { StatusCode = 404 };
                }
            }
            else
            {
                return new ObjectResult("Forbidden") { StatusCode = 403 };
            }
        }

        private FileContentResult CreateThumbnail(string physicalPath)
        {
            using (var fileStream = System.IO.File.OpenRead(physicalPath))
            {
                var desiredSize = new ImageSize
                {
                    Width = ThumbnailWidth,
                    Height = ThumbnailHeight
                };

                const string contentType = "image/png";

                return File(thumbnailCreator.Create(fileStream, desiredSize, contentType), contentType);
            }
        }

        [HttpPost]
        public virtual ActionResult Destroy(string path, string name, string type)
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
                var physicalPath = Path.Combine(MapPath(path), name);

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

            return new ObjectResult("Forbidden") { StatusCode = 403};
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

            return new ObjectResult("Forbidden") { StatusCode = 403 };
        }

        [OutputCache(Duration = 360, VaryByQueryKeys = new string[] { "path" })]
        public ActionResult Image(string path)
        {
            path = NormalizePath(path);

            if (AuthorizeImage(path))
            {
                var physicalPath = MapPath(path);

                if (System.IO.File.Exists(physicalPath))
                {
                    const string contentType = "image/png";
                    return File(System.IO.File.OpenRead(physicalPath), contentType);
                }
            }

            return new ObjectResult("Forbidden") { StatusCode = 403 };
        }

        public virtual bool AuthorizeImage(string path)
        {
            return CanAccess(path) && IsValidFile(Path.GetExtension(path));
        }

        private string MapPath(string path)
        {
            return Path.Combine(_hostingEnvironment.ContentRootPath, path);
        }
    }
}
