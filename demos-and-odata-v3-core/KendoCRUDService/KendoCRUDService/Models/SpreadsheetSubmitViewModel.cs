using KendoCRUDService.Data.Models;

namespace KendoCRUDService.Models
{
    public class SpreadsheetSubmitViewModel
    {
        public IList<Product> Created { get; set; }

        public IList<Product> Destroyed { get; set; }

        public IList<Product> Updated { get; set; }
    }
}
