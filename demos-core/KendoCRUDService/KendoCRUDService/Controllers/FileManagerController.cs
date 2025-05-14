using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.FileBrowser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Net.Mail;

namespace KendoCRUDService.Controllers
{
    public class FileManagerController : Controller
    {
        private const string DefaultFilter = "*.txt,*.docx,*.xlsx,*.ppt,*.pptx,*.zip,*.rar,*.jpg,*.jpeg,*.gif,*.png";
        private readonly DirectoryRepository _directoryRepository;

        public FileManagerController(DirectoryRepository directoryRepository)
        {
            _directoryRepository = directoryRepository;
            _directoryRepository.ContentRootPath = "wwwroot/Content/filemanager";
            _directoryRepository.DefaultFilter = DefaultFilter;
        }

        public virtual IActionResult Read(string target)
        {
            var path = _directoryRepository.NormalizePath(target);

            try
            {
                var result = _directoryRepository
                    .GetContent(path??"", DefaultFilter)
                    .Select(f => new
                    {
                        name = f.Name,
                        size = f.Size,
                        path = f.Path,
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

        [HttpPost]
        public virtual ActionResult Destroy(FileManagerEntry entry)
        {
            _directoryRepository.Destroy(entry);

            return Json(new object[0]);
        }

        [HttpPost]
        public virtual ActionResult Create(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (String.IsNullOrEmpty(entry.Path))
            {
                newEntry = _directoryRepository.Create(entry, target);
            }
            else
            {
                newEntry = _directoryRepository.CopyEntry(target, entry);
            }

            return Json(new
            {
                name = newEntry.Name,
                size = newEntry.Size,
                path = newEntry.Path,
                extension = newEntry.Extension,
                isDirectory = newEntry.IsDirectory,
                hasDirectories = newEntry.HasDirectories,
                created = newEntry.Created,
                createdUtc = newEntry.CreatedUtc,
                modified = newEntry.Modified,
                modifiedUtc = newEntry.ModifiedUtc
            });
        }



        [HttpPost]
        public virtual ActionResult Update(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry = _directoryRepository.RenameEntry(entry);

            return Json(new
            {
                name = newEntry.Name,
                size = newEntry.Size,
                path = newEntry.Path,
                extension = newEntry.Extension,
                isDirectory = newEntry.IsDirectory,
                hasDirectories = newEntry.HasDirectories,
                created = newEntry.Created,
                createdUtc = newEntry.CreatedUtc,
                modified = newEntry.Modified,
                modifiedUtc = newEntry.ModifiedUtc
            });
        }

        [HttpPost]
        public virtual ActionResult Upload(string path, IFormFile file)
        {
            FileManagerEntry newEntry;
            newEntry = _directoryRepository.Upload(path, file);

            return Json(new
            {
                name = newEntry.Name,
                size = newEntry.Size,
                path = newEntry.Path,
                extension = newEntry.Extension,
                isDirectory = newEntry.IsDirectory,
                hasDirectories = newEntry.HasDirectories,
                created = newEntry.Created,
                createdUtc = newEntry.CreatedUtc,
                modified = newEntry.Modified,
                modifiedUtc = newEntry.ModifiedUtc
            });
        }
    }
}
