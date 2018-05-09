using System.Web.Http;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Builder;
using kendo_northwind_pg.Models;
using Microsoft.AspNet.OData.Batch;

namespace kendo_northwind_pg
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Employee>("Employees").EntityType.Ignore(t => t.Photo);
            builder.EntityType<Employee>().Collection.Function("TopEmployees").ReturnsCollectionFromEntitySet<Employee>("bindingParameter/Employees");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Region>("Regions");
            builder.EntitySet<Shipper>("Shippers");
            builder.EntitySet<Supplier>("Suppliers");
            builder.EntitySet<Territory>("Territories");

            config.EnableCors();
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
        }
    }
}
