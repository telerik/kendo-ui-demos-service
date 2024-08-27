using KendoCRUDService.FileBrowser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Hosting.Internal;

namespace KendoCRUDService.Controllers
{
    public class RedditImagesController : Controller
    {
        private const string contentFolderRoot = "~/Content/reddit";
        private const string prettyName = "Images/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/reddit/" };

        private readonly DirectoryBrowser directoryBrowser;
        private readonly ContentInitializer contentInitializer;
        private readonly ThumbnailCreator thumbnailCreator;


        public RedditImagesController(IHttpContextAccessor httpContextAccessor)
        {
            directoryBrowser = new DirectoryBrowser();
            contentInitializer = new ContentInitializer(httpContextAccessor, contentFolderRoot, foldersToCopy, prettyName);
            thumbnailCreator = new ThumbnailCreator();
        }

        [OutputCache(Duration = 3600, VaryByQueryKeys = new string[] { "*" })]
        public async virtual Task<ActionResult> Index(string url, int width, int height)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));

            try
            {
                var httpClient = new HttpClient();
                var webResponse = await httpClient.GetAsync(new Uri(url));

                await webResponse.Content.LoadIntoBufferAsync();

                if (webResponse.Content.Headers.ContentLength < 1024 * 1024 * 5)
                {
                    Directory.CreateDirectory(path);

                    string physicalPath = Path.Combine(path, Guid.NewGuid().ToString("n"));
                    var response = await httpClient.GetAsync(url);

                    using (var fs = new FileStream(physicalPath, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fs);
                    }

                    return CreateThumbnail(physicalPath, width, height);
                }
                else
                {
                    return new ObjectResult("Forbidden") { StatusCode = 403};
                }
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        private FileContentResult CreateThumbnail(string physicalPath, int width, int height)
        {
            using (var fileStream = System.IO.File.OpenRead(physicalPath))
            {
                var desiredSize = new ImageSize
                {
                    Width = width,
                    Height = height
                };

                const string contentType = "image/png";

                return File(thumbnailCreator.CreateFill(fileStream, desiredSize, contentType), contentType);
            }
        }
    }
}
