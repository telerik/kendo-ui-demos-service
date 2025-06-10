using KendoAIService.Models;

namespace KendoAIService.Interfaces
{
    public interface IProductRepository
    {
        IList<ProductModel> All();
    }
}
