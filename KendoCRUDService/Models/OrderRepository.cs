using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public static class OrderRepository
    {
        public static IList<OrderModel> All()
        {
            IList<OrderModel> result = HttpContext.Current.Session["Orders"] as IList<OrderModel>;

            if (result == null)
            {
                HttpContext.Current.Session["Customers"] = result = new SampleDataContext().Orders.Select(c => new OrderModel
                {
                    OrderID = c.OrderID,
                    ShipName = c.ShipName,
                    ShipCity = c.ShipCity,
                    ShipCountry = c.ShipCountry
                }).ToList();
            }

            return result;
        }
    }
}