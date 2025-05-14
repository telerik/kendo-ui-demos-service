using KendoCRUDService.Data.Models;

namespace KendoCRUDService.Models
{
    public class DetailProductSubmitViewModel
    {
        public IList<DetailProduct> Created { get; set; }

        public IList<DetailProduct> Destroyed { get; set; }

        public IList<DetailProduct> Updated { get; set; }
    }
}
