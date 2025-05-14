namespace KendoCRUDService.Data.Models
{
    public class FileManagerEntry
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public bool IsDirectory { get; set; }
        public bool HasDirectories { get; set; }

        public DateTime Created { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime Modified { get; set; }
        public DateTime ModifiedUtc { get; set; }

        public IEnumerable<FileManagerEntry> Directories { get; set; }

        public FileManagerEntry Clone()
        {
            return new FileManagerEntry
            {
                Name = this.Name,
                Size = this.Size,
                Path = this.Path,
                Extension = this.Extension,
                IsDirectory = this.IsDirectory,
                HasDirectories = this.HasDirectories,
                Created = this.Created,
                CreatedUtc = this.CreatedUtc,
                Modified = this.Modified,
                ModifiedUtc = this.ModifiedUtc,
                Directories = this.Directories?.Select(d => d.Clone()).ToList()
            };
        }
    }
}
