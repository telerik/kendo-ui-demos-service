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
        private const string contentFolderRoot = "wwwroot/Content/editor";
        private const string DefaultFilter = "*.png,*.gif,*.jpg,*.jpeg";

        private readonly FileBrowserRepository _fileBrowserRepository;

        public ImageBrowserController(FileBrowserRepository directoryRepository)
        {
            _fileBrowserRepository = directoryRepository;
            _fileBrowserRepository.DefaultFilter = DefaultFilter;
            _fileBrowserRepository.ContentRootPath = contentFolderRoot;

        }

        public virtual IActionResult Read(string path)
        {
            try
            {
                var result = _fileBrowserRepository
                    .GetContent(path ?? "", DefaultFilter)
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

        [OutputCache(Duration = 3600, VaryByQueryKeys = new string[] { "path" })]
        public virtual ActionResult Thumbnail(string path)
        {
           return File(_fileBrowserRepository.CreateThumbnail(path), "image/png");
        }

        [HttpPost]
        public virtual ActionResult Destroy(string path, string name, string type)
        {
            _fileBrowserRepository.Destroy(path, name, type);
            return Json(new object[0]);
        }

        [HttpPost]
        public virtual ActionResult Create(string path, FileBrowserEntry entry)
        {
            _fileBrowserRepository.Create(path, entry);

            return Json(new
            {
                name = entry.Name,
                type = "d",
                size = entry.Size
            });
        }

        [HttpPost]
        public virtual ActionResult Upload(string path, IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);

            _fileBrowserRepository.Upload(path, file);

            return Json(new
            {
                size = file.Length,
                name = fileName,
                type = "f"
            });
        }

        [OutputCache(Duration = 360, VaryByQueryKeys = new string[] { "path" })]
        public ActionResult Image(string path)
        {
            const string contentType = "image/png";
            return File(_fileBrowserRepository.Download(path), contentType, path);
        }
    }
}
