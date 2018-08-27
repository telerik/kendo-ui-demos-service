using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using graphql_aspnet_core.Data.Contracts;

namespace graphql_aspnet_core.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private SampleEntitiesDataContext _db;

        public ProductRepository(SampleEntitiesDataContext db)
        {
            _db = db;
        }
        public async Task<List<Product>> AllAsync()
        {
            return await _db.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Product> GetAsync(int ProductID)
        {
            return await _db.Products.FirstOrDefaultAsync(p => p.ProductID == ProductID);
        }

        public async Task<Product> AddAsync(Product product)
        {            
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            return product;
        }

        public async Task<Product> Delete(Product product)
        {
            return product;
        }

        public async Task<int> GetTotalRecords()
        {
            var total = await _db.Products.CountAsync();

            return total;
        }
    }
}
