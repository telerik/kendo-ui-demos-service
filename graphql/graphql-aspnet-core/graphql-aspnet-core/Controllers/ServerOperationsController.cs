using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_aspnet_core.Data;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace graphql_aspnet_core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerOperationsController : Controller
    {
        private CustomersEntitiesDataContext context;

        public ServerOperationsController(CustomersEntitiesDataContext context)
        {
            this.context = context;
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult Get([DataSourceRequest]DataSourceRequest request)
        {
            return Json(this.context.Customers.ToList().ToDataSourceResult(request));
        }
    }
}