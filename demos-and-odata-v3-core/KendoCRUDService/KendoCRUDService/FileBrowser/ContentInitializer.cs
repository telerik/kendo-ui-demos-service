namespace KendoCRUDService.FileBrowser
{
    public class ContentInitializer
    {
        private readonly ISession _session;


        public ContentInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        public string prettyName { get; set; }
        public string[] foldersToCopy { get; set; }
        public string rootFolder { get; set; }

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
            return path;
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
