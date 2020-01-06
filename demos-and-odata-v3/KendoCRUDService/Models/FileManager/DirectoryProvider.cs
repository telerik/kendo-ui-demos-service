using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models.FileManager
{
    public class DirectoryProvider
    {
        public IEnumerable<FileManagerEntry> GetContent(string path, string filter)
        {
            return GetFiles(path, filter).Concat(GetDirectories(path));
        }

        private IEnumerable<FileManagerEntry> GetFiles(string path, string filter)
        {
            var directory = new DirectoryInfo(Server.MapPath(path));

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

        private IEnumerable<FileManagerEntry> GetDirectories(string path)
        {
            var directory = new DirectoryInfo(Server.MapPath(path));

            return directory.GetDirectories()
                .Select(subDirectory => new FileManagerEntry
                {
                    Name = subDirectory.Name,
                    Path = subDirectory.FullName,
                    Extension = subDirectory.Extension,
                    IsDirectory = true,
                    HasDirectories = subDirectory.GetDirectories().Length > 0,
                    Created = subDirectory.CreationTime,
                    CreatedUtc = subDirectory.CreationTimeUtc,
                    Modified = subDirectory.LastWriteTime,
                    ModifiedUtc = subDirectory.LastWriteTimeUtc
                });
        }

        public FileManagerEntry GetDirectory(string path)
        {
            var directory = new DirectoryInfo(path);

            return  new FileManagerEntry
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

        public System.Web.HttpServerUtilityBase Server { get; set; }
    }
}