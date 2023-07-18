namespace KendoCRUDService.FileBrowser
{
    public class ContentInitializer
    {
        private string rootFolder;
        private string[] foldersToCopy;
        private string prettyName;
        private readonly ISession _session;


        public ContentInitializer(IHttpContextAccessor httpContextAccessor, string rootFolder, string[] foldersToCopy, string prettyName)
        {
            this.rootFolder = rootFolder;
            this.foldersToCopy = foldersToCopy;
            this.prettyName = prettyName;
            _session = httpContextAccessor.HttpContext.Session;
        }

        private string UserID
        {
            get
            {
                var obj = _session.GetString("UserID"); 
                if (obj == null)
                {
                    obj = DateTime.Now.Ticks.ToString();
                    _session.SetString("UserID", obj);
                }
                return (string)obj;
            }
        }

        public string CreateUserFolder(string contentRootPath)
        {
            var virtualPath = Path.Combine(rootFolder, Path.Combine("UserFiles", UserID), prettyName);

            var path = Path.Combine(contentRootPath, virtualPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    CopyFolder(Path.Combine(contentRootPath, sourceFolder), path);
                }
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }
    }
}
