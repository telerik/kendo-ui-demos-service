using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KendoCRUDService.Common
{
    public static class PagingExtension
    {
        public static IList ApplyPaging<T>(int skip, int take, IList data)
        {
            return data
                .OfType<T>()
                .Skip(skip)
                .Take(take)
                .ToList();
        }
    }
}