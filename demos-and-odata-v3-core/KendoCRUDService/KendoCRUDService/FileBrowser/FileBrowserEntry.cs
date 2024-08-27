using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KendoCRUDService.FileBrowser
{
    public class FileBrowserEntry
    {
        public string Name { get; set; }
        public EntryType Type { get; set; }
        public long Size { get; set; }
    }
}
