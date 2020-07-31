using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public static class CountryRepository
    {
        public static IList<CountryModel> All()
        {
            IList<CountryModel> result = HttpContext.Current.Session["Countries"] as IList<CountryModel>;

            if (result == null)
            {
                HttpContext.Current.Session["Countries"] = result = new SampleDataContext().Countries.Select(c => new CountryModel
                {
                    CountryID = c.CountryID,
                    CountryNameLong = c.CountryNameLong,
                    CountryNameShort = c.CountryNameShort,
                }).ToList();
            }

            return result;
        }
    }
}