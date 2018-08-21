using System.Collections.Generic;
using System.Threading.Tasks;

namespace graphql_aspnet_core.Data.Contracts
{
    public interface IProductRepository
    {
        Task<Product> GetAsync(int ProductID);

        Task<List<Product>> AllAsync();

        Task<Product> AddAsync(Product product);

        Task<Product> UpdateAsync(Product product);

        Task<Product> Delete(Product product);

        Task<int> GetTotalRecords();
    }
}
