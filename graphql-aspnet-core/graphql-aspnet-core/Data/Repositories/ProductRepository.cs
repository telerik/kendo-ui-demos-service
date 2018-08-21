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
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            var existingProduct = await _db.Products.FindAsync(product.ProductID);
            existingProduct.ProductName = product.ProductName;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.UnitsInStock = product.UnitsInStock;
            _db.Update(existingProduct);

            await _db.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<Product> Delete(Product product)
        {
            var existingProduct = await _db.Products.FindAsync(product.ProductID);
            _db.Products.Remove(existingProduct);

            await _db.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<int> GetTotalRecords()
        {
            var total = await _db.Products.CountAsync();

            return total;
        }
    }
}
