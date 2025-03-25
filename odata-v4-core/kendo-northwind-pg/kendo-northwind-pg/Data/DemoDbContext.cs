using kendo_northwind_pg.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace kendo_northwind_pg.Data
{
    public partial class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options)
            : base(options)
        {
            if (Database.IsSqlite())
            {
                // This is a workaround for the SQLite locking issues.
                // see more: https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/database-errors
                var connection = Database.GetDbConnection();
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PRAGMA journal_mode=WAL;";
                    command.ExecuteNonQuery();
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasKey(e => e.CategoryID);

                entity.HasIndex(e => e.CategoryName).HasDatabaseName("Categories_CategoryName");

                entity.Property(e => e.CategoryName)
                .IsRequired()
                    .HasColumnType("NVARCHAR(15)");
            });


            modelBuilder.Entity<CustomerCustomerDemo>(entity =>
            {
                entity.ToTable("CustomerCustomerDemo");

                entity.HasKey(e => new { e.CustomerID, e.CustomerTypeID });

                entity.Property(e => e.CustomerID).HasColumnType("CHAR(5)");

                entity.Property(e => e.CustomerTypeID).HasColumnType("CHAR(10)");

                entity.HasOne(d => d.Customer).WithMany(p => p.CustomerCustomerDemo).HasForeignKey(d => d.CustomerID).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CustomerType).WithMany(p => p.CustomerCustomerDemo).HasForeignKey(d => d.CustomerTypeID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CustomerDemographic>(entity =>
            {
                entity.ToTable("CustomerDemographics");
                entity.HasKey(e => e.CustomerTypeID);

                entity.Property(e => e.CustomerTypeID).HasColumnType("CHAR(10)");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");

                entity.HasKey(e => e.CustomerID);

                entity.HasIndex(e => e.City).HasDatabaseName("Customers_City");

                entity.HasIndex(e => e.CompanyName).HasDatabaseName("Customers_CompanyName");

                entity.HasIndex(e => e.PostalCode).HasDatabaseName("Customers_PostalCode");

                entity.HasIndex(e => e.Region).HasDatabaseName("Customers_Region");

                entity.Property(e => e.CustomerID).HasColumnType("CHAR(5)");

                entity.Property(e => e.Address)
                    .HasColumnType("NVARCHAR(60)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Bool)
                    .HasColumnType("BOOLEAN")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.City)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(40)");

                entity.Property(e => e.ContactName)
                    .HasColumnType("NVARCHAR(30)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ContactTitle)
                    .HasColumnType("NVARCHAR(30)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Country)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Fax)
                    .HasColumnType("NVARCHAR(24)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Phone)
                    .HasColumnType("NVARCHAR(24)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.PostalCode)
                    .HasColumnType("NVARCHAR(10)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Region)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<EmployeeTerritory>(entity =>
            {
                entity.ToTable("EmployeeTerritories");

                entity.HasKey(e => new { e.EmployeeID, e.TerritoryID });

                entity.Property(e => e.EmployeeID).HasColumnType("INT");

                entity.Property(e => e.TerritoryID).HasColumnType("NVARCHAR(20)");

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeTerritories).HasForeignKey(d => d.EmployeeID).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Territory).WithMany(p => p.EmployeeTerritories).HasForeignKey(d => d.TerritoryID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasKey(e => e.EmployeeID);

                entity.HasIndex(e => e.LastName).HasDatabaseName("Employees_LastName");

                entity.HasIndex(e => e.PostalCode).HasDatabaseName("Employees_PostalCode");

                entity.Property(e => e.Address)
                    .HasColumnType("NVARCHAR(60)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.City)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Country)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Extension)
                    .HasColumnType("NVARCHAR(4)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(10)");

                entity.Property(e => e.HireDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.HomePhone)
                    .HasColumnType("NVARCHAR(24)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(20)");

                entity.Property(e => e.PhotoPath)
                    .HasColumnType("NVARCHAR(255)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.PostalCode)
                    .HasColumnType("NVARCHAR(10)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Region)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReportsTo)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Title)
                    .HasColumnType("NVARCHAR(30)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.TitleOfCourtesy)
                    .HasColumnType("NVARCHAR(25)")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation).HasForeignKey(d => d.ReportsTo);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetails");

                entity.HasKey(e => new { e.OrderID, e.ProductID });

                entity.HasIndex(e => e.OrderID).HasDatabaseName("Order_Details_OrdersOrder_Details");

                entity.HasIndex(e => e.ProductID).HasDatabaseName("Order_Details_ProductsOrder_Details");

                entity.Property(e => e.OrderID).HasColumnType("INT");

                entity.Property(e => e.ProductID).HasColumnType("INT");

                entity.Property(e => e.Discount)
                    .HasColumnType("REAL(24,0)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Quantity)
                    .HasColumnType("SMALLINT")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("FLOAT(5,2)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasForeignKey(d => d.OrderID).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasForeignKey(d => d.ProductID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(e => e.OrderID);

                entity.HasIndex(e => e.CustomerID).HasDatabaseName("Orders_CustomersOrders");

                entity.HasIndex(e => e.EmployeeID).HasDatabaseName("Orders_EmployeesOrders");

                entity.HasIndex(e => e.OrderDate).HasDatabaseName("Orders_OrderDate");

                entity.HasIndex(e => e.ShipPostalCode).HasDatabaseName("Orders_ShipPostalCode");

                entity.HasIndex(e => e.ShipVia).HasDatabaseName("Orders_ShippersOrders");

                entity.HasIndex(e => e.ShippedDate).HasDatabaseName("Orders_ShippedDate");

                entity.Property(e => e.CustomerID)
                    .HasColumnType("CHAR(5)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.EmployeeID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Freight)
                    .HasColumnType("FLOAT(6,2)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RequiredDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShipAddress)
                    .HasColumnType("NVARCHAR(60)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShipCity)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShipCountry)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShipName)
                    .HasColumnType("NVARCHAR(40)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShippedDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShipPostalCode)
                    .HasColumnType("NVARCHAR(10)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShipRegion)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ShipVia)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasForeignKey(d => d.CustomerID);

                entity.HasOne(d => d.Employee).WithMany(p => p.Orders).HasForeignKey(d => d.EmployeeID);

                entity.HasOne(d => d.ShipViaNavigation).WithMany(p => p.Orders).HasForeignKey(d => d.ShipVia);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.ProductID);

                entity.HasIndex(e => e.CategoryID).HasDatabaseName("Products_CategoryID");

                entity.HasIndex(e => e.ProductName).HasDatabaseName("Products_ProductName");

                entity.HasIndex(e => e.SupplierID).HasDatabaseName("Products_SuppliersProducts");

                entity.Property(e => e.CategoryID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Discontinued)
                    .IsRequired()
                    .HasColumnType("BOOLEAN")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(40)");

                entity.Property(e => e.QuantityPerUnit)
                    .HasColumnType("NVARCHAR(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReorderLevel)
                    .HasColumnType("SMALLINT")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SupplierID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("FLOAT(5,2)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.UnitsInStock)
                    .HasColumnType("SMALLINT")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.UnitsOnOrder)
                    .HasColumnType("SMALLINT")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Category).WithMany(p => p.Products).HasForeignKey(d => d.CategoryID);

                entity.HasOne(d => d.Supplier).WithMany(p => p.Products).HasForeignKey(d => d.SupplierID);
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("Regions");

                entity.Property(e => e.RegionID).HasColumnType("INT");

                entity.Property(e => e.RegionDescription)
                    .IsRequired()
                    .HasColumnType("CHAR(50)");
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.ToTable("Shippers");

                entity.HasKey(e => e.ShipperID);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(40)");

                entity.Property(e => e.Phone)
                    .HasColumnType("NVARCHAR(24)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");

                entity.HasKey(e => e.SupplierID);

                entity.HasIndex(e => e.CompanyName).HasDatabaseName("Suppliers_CompanyName");

                entity.HasIndex(e => e.PostalCode).HasDatabaseName("Suppliers_PostalCode");

                entity.Property(e => e.Address)
                    .HasColumnType("NVARCHAR(60)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.City)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(40)");

                entity.Property(e => e.ContactName)
                    .HasColumnType("NVARCHAR(30)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ContactTitle)
                    .HasColumnType("NVARCHAR(30)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Country)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Fax)
                    .HasColumnType("NVARCHAR(24)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Phone)
                    .HasColumnType("NVARCHAR(24)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.PostalCode)
                    .HasColumnType("NVARCHAR(10)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Region)
                    .HasColumnType("NVARCHAR(15)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Territory>(entity =>
            {
                entity.ToTable("Territories");

                entity.HasKey(e => e.TerritoryID);

                entity.Property(e => e.TerritoryID).HasColumnType("NVARCHAR(20)");

                entity.Property(e => e.RegionID).HasColumnType("INT");

                entity.Property(e => e.TerritoryDescription)
                    .IsRequired()
                    .HasColumnType("CHAR(50)");

                entity.HasOne(d => d.Region).WithMany(p => p.Territories).HasForeignKey(d => d.RegionID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DetailProduct>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId)
                    .ValueGeneratedNever()
                    .HasColumnType("INT")
                    .HasColumnName("ProductID");
                entity.Property(e => e.CategoryId)
                    .HasColumnType("INT")
                    .HasColumnName("CategoryID");
                entity.Property(e => e.CountryId)
                    .HasColumnType("tinyint")
                    .HasColumnName("CountryID");
                entity.Property(e => e.CustomerRating).HasColumnType("tinyint");
                entity.Property(e => e.Discontinued).HasColumnType("bit");
                entity.Property(e => e.LastSupply).HasColumnType("DATETIME");
                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnType("nvarchar(40)");
                entity.Property(e => e.QuantityPerUnit).HasColumnType("nvarchar(20)");
                entity.Property(e => e.TargetSales).HasColumnType("INT");
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(5,2)");
                entity.Property(e => e.UnitsInStock).HasColumnType("smallint");
                entity.Property(e => e.UnitsOnOrder).HasColumnType("INT");

                entity.HasOne(d => d.Category).WithMany(p => p.DetailProducts).HasForeignKey(d => d.CategoryId);
            });

            modelBuilder.Entity<EmployeeDirectory>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);

                entity.ToTable("EmployeeDirectory");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
                entity.Property(e => e.Address)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("VARCHAR(255)");
                entity.Property(e => e.BirthDate)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("DATETIME");
                entity.Property(e => e.City)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("VARCHAR(255)");
                entity.Property(e => e.Country)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("VARCHAR(100)");
                entity.Property(e => e.Extension)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");
                entity.Property(e => e.FirstName)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("VARCHAR(255)");
                entity.Property(e => e.HireDate)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("DATETIME");
                entity.Property(e => e.LastName)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("VARCHAR(255)");
                entity.Property(e => e.Phone)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("VARCHAR(100)");
                entity.Property(e => e.Position)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("NVARCHAR(255)");
                entity.Property(e => e.ReportsTo)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");

                entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation).HasForeignKey(d => d.ReportsTo);
            });

            modelBuilder.Entity<GanttDependency>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.PredecessorId)
                    .HasColumnType("INT")
                    .HasColumnName("PredecessorID");
                entity.Property(e => e.SuccessorId)
                    .HasColumnType("INT")
                    .HasColumnName("SuccessorID");
                entity.Property(e => e.Type).HasColumnType("INT");
            });

            modelBuilder.Entity<GanttResource>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Color)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("CHAR(10)");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(50)");
            });

            modelBuilder.Entity<GanttResourceAssignment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ResourceId)
                    .HasColumnType("INT")
                    .HasColumnName("ResourceID");
                entity.Property(e => e.TaskId)
                    .HasColumnType("INT")
                    .HasColumnName("TaskID");
                entity.Property(e => e.Units).HasColumnType("FLOAT(5,2)");
            });

            modelBuilder.Entity<GanttTask>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.End).HasColumnType("DATETIME");
                entity.Property(e => e.Expanded).HasColumnType("BOOLEAN");
                entity.Property(e => e.OrderId)
                    .HasColumnType("INT")
                    .HasColumnName("OrderID");
                entity.Property(e => e.ParentId)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT")
                    .HasColumnName("ParentID");
                entity.Property(e => e.PercentComplete).HasColumnType("FLOAT(5,2)");
                entity.Property(e => e.Start).HasColumnType("DATETIME");
                entity.Property(e => e.Summary).HasColumnType("BOOLEAN");
                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentId);
            });

            modelBuilder.Entity<Intraday>(entity =>
            {
                entity.HasIndex(e => e.Date, "IX_Intraday_Date").IsDescending();

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Close).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Date).HasColumnType("DATETIME");
                entity.Property(e => e.High).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Low).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Open).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");
                entity.Property(e => e.Volume).HasColumnType("INT");
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.Property(e => e.MeetingId).HasColumnName("MeetingID");
                entity.Property(e => e.End).HasColumnType("DATETIME");
                entity.Property(e => e.IsAllDay).HasColumnType("BOOLEAN");
                entity.Property(e => e.RecurrenceId)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT")
                    .HasColumnName("RecurrenceID");
                entity.Property(e => e.RoomId)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT")
                    .HasColumnName("RoomID");
                entity.Property(e => e.Start).HasColumnType("DATETIME");
                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Recurrence).WithMany(p => p.InverseRecurrence).HasForeignKey(d => d.RecurrenceId);
            });

            modelBuilder.Entity<MeetingAttendee>(entity =>
            {
                entity.HasKey(e => new { e.MeetingId, e.AttendeeId });

                entity.Property(e => e.MeetingId)
                    .HasColumnType("INT")
                    .HasColumnName("MeetingID");
                entity.Property(e => e.AttendeeId)
                    .HasColumnType("INT")
                    .HasColumnName("AttendeeID");

                entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingAttendees)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrgChartConnection>(entity =>
            {
                entity.Property(e => e.FromPointX)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");
                entity.Property(e => e.FromPointY)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");
                entity.Property(e => e.FromShapeId)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");
                entity.Property(e => e.ToPointX)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");
                entity.Property(e => e.ToPointY)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");
                entity.Property(e => e.ToShapeId)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT");
            });

            modelBuilder.Entity<OrgChartShape>(entity =>
            {
                entity.Property(e => e.Color)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("NVARCHAR(50)");
                entity.Property(e => e.JobTitle)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("NVARCHAR(200)");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasIndex(e => e.Date, "IX_Stock_Date").IsDescending();

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Close).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Date).HasColumnType("DATETIME");
                entity.Property(e => e.High).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Low).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Open).HasColumnType("FLOAT(9,3)");
                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");
                entity.Property(e => e.Volume).HasColumnType("INT");
            });

            modelBuilder.Entity<kendo_northwind_pg.Data.Models.Task>(entity =>
            {
                entity.Property(e => e.TaskId).HasColumnName("TaskID");
                entity.Property(e => e.End).HasColumnType("DATETIME");
                entity.Property(e => e.IsAllDay).HasColumnType("BOOLEAN");
                entity.Property(e => e.OwnerId)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT")
                    .HasColumnName("OwnerID");
                entity.Property(e => e.RecurrenceId)
                    .HasDefaultValueSql("NULL")
                    .HasColumnType("INT")
                    .HasColumnName("RecurrenceID");
                entity.Property(e => e.Start).HasColumnType("DATETIME");
                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Recurrence).WithMany(p => p.InverseRecurrence).HasForeignKey(d => d.RecurrenceId);
            });

            modelBuilder.Entity<UrbanArea>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(256)");
                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(256)");
                entity.Property(e => e.CountryIso3)
                    .IsRequired()
                    .HasColumnType("CHAR(3)")
                    .HasColumnName("Country_ISO3");
                entity.Property(e => e.Latitude).HasColumnType("FLOAT(9,6)");
                entity.Property(e => e.Longitude).HasColumnType("FLOAT(9,6)");
                entity.Property(e => e.Pop1950).HasColumnType("INT");
                entity.Property(e => e.Pop1955).HasColumnType("INT");
                entity.Property(e => e.Pop1960).HasColumnType("INT");
                entity.Property(e => e.Pop1965).HasColumnType("INT");
                entity.Property(e => e.Pop1970).HasColumnType("INT");
                entity.Property(e => e.Pop1975).HasColumnType("INT");
                entity.Property(e => e.Pop1980).HasColumnType("INT");
                entity.Property(e => e.Pop1985).HasColumnType("INT");
                entity.Property(e => e.Pop1990).HasColumnType("INT");
                entity.Property(e => e.Pop1995).HasColumnType("INT");
                entity.Property(e => e.Pop2000).HasColumnType("INT");
                entity.Property(e => e.Pop2005).HasColumnType("INT");
                entity.Property(e => e.Pop2010).HasColumnType("INT");
                entity.Property(e => e.Pop2015).HasColumnType("INT");
                entity.Property(e => e.Pop2020).HasColumnType("INT");
                entity.Property(e => e.Pop2025).HasColumnType("INT");
                entity.Property(e => e.Pop2050).HasColumnType("INT");
            });
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CustomerCustomerDemo> CustomerCustomerDemo { get; set; }
        public virtual DbSet<CustomerDemographic> CustomerDemographics { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Territory> Territories { get; set; }

        public virtual DbSet<DetailProduct> DetailProducts { get; set; }
        public virtual DbSet<EmployeeDirectory> EmployeeDirectories { get; set; }
        public virtual DbSet<GanttDependency> GanttDependencies { get; set; }

        public virtual DbSet<GanttResource> GanttResources { get; set; }

        public virtual DbSet<GanttResourceAssignment> GanttResourceAssignments { get; set; }

        public virtual DbSet<GanttTask> GanttTasks { get; set; }

        public virtual DbSet<Intraday> Intradays { get; set; }

        public virtual DbSet<Meeting> Meetings { get; set; }

        public virtual DbSet<MeetingAttendee> MeetingAttendees { get; set; }
        public virtual DbSet<OrgChartConnection> OrgChartConnections { get; set; }

        public virtual DbSet<OrgChartShape> OrgChartShapes { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<kendo_northwind_pg.Data.Models.Task> Tasks { get; set; }
        public virtual DbSet<UrbanArea> UrbanAreas { get; set; }
    }
}
