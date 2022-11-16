using KendoCoreService.Models;

namespace KendoCoreService.Interfaces
{
    public interface IProductRepository
    {
        IList<ProductModel> All();
    }
}
