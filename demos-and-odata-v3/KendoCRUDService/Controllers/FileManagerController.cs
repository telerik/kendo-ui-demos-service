using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KendoCRUDService.Models;
using KendoCRUDService.Models.FileManager;

namespace KendoCRUDService.Controllers
{
    public class FileManagerController : Controller
    {
        private const string contentFolderRoot = "~/Content/";
        private const string prettyName = "Files/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/editor/" };
        private const string DefaultFilter = "*.txt,*.doc,*.docx,*.xls,*.xlsx,*.ppt,*.pptx,*.zip,*.rar,*.jpg,*.jpeg,*.gif,*.png";

        private readonly DirectoryProvider directoryProvider;
        private readonly ContentInitializer contentInitializer;

        public FileManagerController()
        {
            directoryProvider = new DirectoryProvider();
            contentInitializer = new ContentInitializer(contentFolderRoot, foldersToCopy, prettyName);
        }

        public string ContentPath
        {
            get
            {
                return contentInitializer.CreateUserFolder(Server);
            }
        }

        private string ToAbsolute(string virtualPath)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        private string ToVirtual(string path)
        {
            return VirtualPathUtility.ToAppRelative(path.Replace(Server.MapPath("~/"), "~/").Replace(@"\", "/"));
        }

        private string CombinePaths(string basePath, string relativePath)
        {
            return VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(basePath), relativePath);
        }

        public virtual bool AuthorizeRead(string path)
        {
            return CanAccess(path);
        }

        protected virtual bool CanAccess(string path)
        {
            return Server.MapPath(path).StartsWith(Server.MapPath(ContentPath), StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ToAbsolute(ContentPath);
            }

            return CombinePaths(ToAbsolute(ContentPath), path);
        }

        public virtual JsonResult Read(string path)
        {
            path = NormalizePath(path);

            if (AuthorizeRead(path))
            {
                try
                {
                    directoryProvider.Server = Server;

                    var result = directoryProvider
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
                            modifiedUtc = f.ModifiedUtc,
                            parentId = ToVirtual(f.ParentId)
                        });

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (DirectoryNotFoundException)
                {
                    throw new HttpException(404, "File Not Found");
                }
            }

            throw new HttpException(403, "Forbidden");
        }

        public virtual JsonResult DirectoryTreeRead(string path)
        {
            path = NormalizePath(path);

            if (AuthorizeRead(path))
            {
                try
                {
                    directoryProvider.Server = Server;

                    var result = GetDirectoryTree(path); 

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (DirectoryNotFoundException)
                {
                    throw new HttpException(404, "File Not Found");
                }
            }

            throw new HttpException(403, "Forbidden");
        }

        private IEnumerable<object> GetDirectoryTree(string path)
        {
           return directoryProvider
                    .GetContent(path, null)
                    .Where(d => d.IsDirectory == true)
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
                        modifiedUtc = f.ModifiedUtc,
                        parentId = ToVirtual(f.ParentId),
                        directories = GetDirectoryTree(ToVirtual(f.Path))
                    });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Destroy(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);

            if (!string.IsNullOrEmpty(path))
            {
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
            throw new HttpException(404, "File Not Found");
        }

        public virtual bool AuthorizeDeleteFile(string path)
        {
            return CanAccess(path);
        }

        public virtual bool AuthorizeDeleteDirectory(string path)
        {
            return CanAccess(path);
        }

        protected virtual void DeleteFile(string path)
        {
            if (!AuthorizeDeleteFile(path))
            {
                throw new HttpException(403, "Forbidden");
            }

            var physicalPath = Server.MapPath(path);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        protected virtual void DeleteDirectory(string path)
        {
            if (!AuthorizeDeleteDirectory(path))
            {
                throw new HttpException(403, "Forbidden");
            }

            var physicalPath = Server.MapPath(path);

            if (Directory.Exists(physicalPath))
            {
                Directory.Delete(physicalPath, true);
            }
        }

        public virtual bool Authorize(string path)
        {
            return CanAccess(path);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Create(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var name = entry.Name;
            FileManagerEntry newEntry;

            if (!string.IsNullOrEmpty(name) && Authorize(path))
            {
                var physicalPath = Path.Combine(Server.MapPath(path), name);

                var sequence = 0;
                var tempName = name;

                while (Directory.Exists(physicalPath)) {
                    tempName = name + String.Format("({0})", ++sequence);
                    physicalPath = Path.Combine(Server.MapPath(path), tempName);
                }

                if (entry.IsDirectory && !Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);

                    newEntry = directoryProvider.GetDirectory(physicalPath);

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
                        modifiedUtc = newEntry.ModifiedUtc,
                        parentId = ToVirtual(newEntry.ParentId)
                    });
                }
            }

            throw new HttpException(403, "Forbidden");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Update(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var name = entry.Name;
            FileManagerEntry newEntry;
            

            if (!string.IsNullOrEmpty(name) && Authorize(path))
            {
                var physicalPath = Server.MapPath(path);

                if (entry.IsDirectory)
                {
                    var directory = new DirectoryInfo(physicalPath);
                    var newPath = Path.Combine(directory.Parent.FullName, name);
                    Directory.Move(physicalPath, newPath);
                    newEntry = directoryProvider.GetDirectory(newPath);
                }
                else 
                {
                    var file = new FileInfo(physicalPath);
                    var newPath = Path.Combine(file.DirectoryName, name);
                    System.IO.File.Move(file.FullName, newPath);
                    newEntry = directoryProvider.GetFile(newPath);
                }

                return Json(new {
                    name = newEntry.Name,
                    size = newEntry.Size,
                    path = ToVirtual(newEntry.Path),
                    extension = newEntry.Extension,
                    isDirectory = newEntry.IsDirectory,
                    hasDirectories = newEntry.HasDirectories,
                    created = newEntry.Created,
                    createdUtc = newEntry.CreatedUtc,
                    modified = newEntry.Modified,
                    modifiedUtc = newEntry.ModifiedUtc,
                    parentId = ToVirtual(newEntry.ParentId)
                });
            }

            throw new HttpException(403, "Forbidden");
        }

        public virtual bool AuthorizeUpload(string path, HttpPostedFileBase file)
        {
            return CanAccess(path) && IsValidFile(file.FileName);
        }

        private bool IsValidFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var allowedExtensions = DefaultFilter.Split(',');

            return allowedExtensions.Any(e => e.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Upload(string path, HttpPostedFileBase file)
        {
            path = NormalizePath(path);
            var fileName = Path.GetFileName(file.FileName);

            if (AuthorizeUpload(path, file))
            {
                file.SaveAs(Path.Combine(Server.MapPath(path), fileName));

                return Json(new
                {
                    size = file.ContentLength,
                    name = fileName,
                    type = "f"
                }, "text/plain");
            }

            throw new HttpException(403, "Forbidden");
        }

        //[OutputCache(Duration = 360, VaryByParam = "path")]
        //public ActionResult File(string fileName)
        //{
        //    var path = NormalizePath(fileName);

        //    if (AuthorizeFile(path))
        //    {
        //        var physicalPath = Server.MapPath(path);

        //        if (System.IO.File.Exists(physicalPath))
        //        {
        //            const string contentType = "application/octet-stream";
        //            return File(System.IO.File.OpenRead(physicalPath), contentType, fileName);
        //        }
        //    }

        //    throw new HttpException(403, "Forbidden");
        //}

        public virtual bool AuthorizeFile(string path)
        {
            return CanAccess(path) && IsValidFile(Path.GetExtension(path));
        }
    }
}
