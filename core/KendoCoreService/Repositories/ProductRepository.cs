using KendoCoreService.Extensions;
using KendoCoreService.Interfaces;
using KendoCoreService.Models;

namespace KendoCoreService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISession _session;

        public ProductRepository(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        public IList<ProductModel> All()
        {
            var result = this._session.GetList<IList<ProductModel>>("Products");

            if (result == null)
            {
                result = Enumerable.Range(1, 500)
                        .Select(i => new ProductModel()
                        {
                            ProductID = i,
                            ProductName = "Product Name " + i,
                            UnitPrice = i * 3.41,
                            UnitsInStock = i % 10,
                            Discontinued = i % 3 == 0
                        }).ToList();

                this._session.SetList("Products", result);
            }

            return result;
        }
    }
}
