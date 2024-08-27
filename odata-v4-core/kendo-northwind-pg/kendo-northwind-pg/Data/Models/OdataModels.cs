using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace kendo_northwind_pg.Data.Models
{
    public static class OdataModels
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.Action("odata/EmployeeDedoviq");
            builder.EntitySet<Employee>("TopEmployees");
            builder.EntitySet<Employee>("EmployeeTerritories");
            builder.EntitySet<Employee>("EmployeeOrders");
            builder.EntitySet<Employee>("EmployeeSubordinates");
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Employee>("Employees");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Region>("Regions");
            builder.EntitySet<Shipper>("Shippers");
            builder.EntitySet<Supplier>("Suppliers");
            builder.EntitySet<Territory>("Territories");
            builder.StructuralTypes.First(t => t.ClrType == typeof(Employee)).AddProperty(typeof(Employee).GetProperty("hasChildren"));

            return builder.GetEdmModel();
        }
    }
}
 