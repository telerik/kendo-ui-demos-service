using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class CategoriesRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public CategoriesRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<Category> All()
        {
            IList<Category> result = _session.GetObjectFromJson<IList<Category>>("Categories");

            if (result == null)
            {
                result = _contextFactory.CreateDbContext().Categories.Select(c => new Category
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    Picture = c.Picture
                }).ToList();

                _session.SetObjectAsJson("Categories", result);
            }

            return result;
        }
    }
}
