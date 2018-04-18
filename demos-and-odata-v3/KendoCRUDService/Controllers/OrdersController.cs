using System.Linq;
using System.Web.Mvc;
using KendoCRUDService.Models;
using KendoCRUDService.Common;
using System.Collections.Generic;

namespace KendoCRUDService.Controllers
{
    public class OrdersController : Controller
    {
        public ActionResult ValueMapper(int[] values)
        {
            var indices = new List<int>();

            if (values != null && values.Any())
            {
                var index = 0;
                foreach (var order in OrderRepository.All()) 
                {
                    if (values.Contains(order.OrderID))
                    {
                        indices.Add(index);
                    }

                    index += 1;
                }
            }

            return this.Jsonp(indices);
        }
    }
}
