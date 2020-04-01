using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnetmvc_ajax_service.Data;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace aspnetmvc_ajax_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private SampleEntitiesDataContext context;

        public CustomersController(SampleEntitiesDataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Get([DataSourceRequest]DataSourceRequest request)
        {
            return Json(this.context.Customers.ToDataSourceResult(request));
        }
    }
}
