using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class ExportController : Controller
    {
        [HttpPost]
        public ActionResult Index(string contentType, string base64, string fileName)
        {
            if (Request.Host.Host.EndsWith("telerik.com"))
            {
                var fileContents = Convert.FromBase64String(base64);

                return File(fileContents, contentType, fileName);
            }

            return new ObjectResult("Available only for demos.telerik.com") { StatusCode = 403};
        }
    }
}
