using KendoCRUDService.Data.Repositories;
using KendoCRUDService.FileBrowser;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.IO;
using System.Net.Mail;
using System.Xml.Linq;

namespace KendoCRUDService.Controllers
{
    public class FileBrowserController : Controller
    {
        private const string contentFolderRoot = "wwwroot/Content/editor";
        private const string DefaultFilter = "*.txt,*.doc,*.docx,*.xls,*.xlsx,*.ppt,*.pptx,*.zip,*.rar,*.jpg,*.jpeg,*.gif,*.png";

        private readonly FileBrowserRepository _fileBrowserRepository;

        public FileBrowserController(FileBrowserRepository browserRepository)
        {
            _fileBrowserRepository = browserRepository;
            _fileBrowserRepository.ContentRootPath = contentFolderRoot;
            _fileBrowserRepository.DefaultFilter = DefaultFilter;
        }

        public virtual IActionResult Read(string path)
        {
            try
            {
                var result = _fileBrowserRepository
                    .GetContent(path??"", DefaultFilter)
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

        [HttpPost]
        public virtual IActionResult Destroy(string path, string name, string type)
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
        public virtual IActionResult Upload(string path, IFormFile file)
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

        public IActionResult File(string fileName)
        {
            const string contentType = "application/octet-stream";
            return File(_fileBrowserRepository.Download(fileName), contentType, fileName);
        }
    }
}
