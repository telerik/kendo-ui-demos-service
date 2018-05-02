using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.IO;

namespace signalr_for_aspnet_core.Models
{
    public partial class SampleEntitiesDataContext : DbContext
    {
        public SampleEntitiesDataContext()
            : base(new DbContextOptions<SampleEntitiesDataContext>())
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dataDirectory = Path.Combine(Startup.WebRootPath, "App_Data");

            options.UseSqlite(@"Data Source=" + dataDirectory + System.IO.Path.DirectorySeparatorChar + @"sample.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasKey(e => e.CategoryID);

                entity.HasIndex(e => e.CategoryName).HasName("Categories_CategoryName");

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

                entity.HasIndex(e => e.City).HasName("Customers_City");

                entity.HasIndex(e => e.CompanyName).HasName("Customers_CompanyName");

                entity.HasIndex(e => e.PostalCode).HasName("Customers_PostalCode");

                entity.HasIndex(e => e.Region).HasName("Customers_Region");

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

            modelBuilder.Entity<EmployeeDirectory>(entity =>
            {
                entity.ToTable("EmployeeDirectory");

                entity.HasKey(e => e.EmployeeID);

                entity.Property(e => e.Address)
                    .HasColumnType("VARCHAR(255)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.City)
                    .HasColumnType("VARCHAR(255)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Country)
                    .HasColumnType("VARCHAR(100)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Extension)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FirstName)
                    .HasColumnType("VARCHAR(255)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.HireDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastName)
                    .HasColumnType("VARCHAR(255)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Phone)
                    .HasColumnType("VARCHAR(100)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Position)
                    .HasColumnType("NVARCHAR(255)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReportsTo)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation).HasForeignKey(d => d.ReportsTo);
            });

            modelBuilder.Entity<EmployeeTerritory>(entity =>
            {
                entity.ToTable("EmployeeTerritories");

                entity.HasKey(e => new { e.EmployeeID, e.TerritoryID });

                entity.Property(e => e.EmployeeID).HasColumnType("INT");

                entity.Property(e => e.TerritoryID).HasColumnType("NVARCHAR(20)");

                entity.HasOne<Employee>(d => d.Employee).WithMany(p => p.EmployeeTerritories).HasForeignKey(d => d.EmployeeID).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Territory).WithMany(p => p.EmployeeTerritories).HasForeignKey(d => d.TerritoryID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasKey(e => e.EmployeeID);

                entity.HasIndex(e => e.LastName).HasName("Employees_LastName");

                entity.HasIndex(e => e.PostalCode).HasName("Employees_PostalCode");

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

            modelBuilder.Entity<GanttDependency>(entity =>
            {
                entity.ToTable("GanttDependencies");

                entity.Property(e => e.PredecessorID).HasColumnType("INT");

                entity.Property(e => e.SuccessorID).HasColumnType("INT");

                entity.Property(e => e.Type).HasColumnType("INT");
            });

            modelBuilder.Entity<GanttResourceAssignment>(entity =>
            {
                entity.ToTable("GanttResourceAssignments");

                entity.Property(e => e.ResourceID).HasColumnType("INT");

                entity.Property(e => e.TaskID).HasColumnType("INT");

                entity.Property(e => e.Units).HasColumnType("FLOAT(5,2)");
            });

            modelBuilder.Entity<GanttResource>(entity =>
            {
                entity.ToTable("GanttResources");

                entity.Property(e => e.Color)
                    .HasColumnType("CHAR(10)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(50)");
            });

            modelBuilder.Entity<GanttTask>(entity =>
            {
                entity.ToTable("GanttTasks");

                entity.Property(e => e.End)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.Expanded)
                    .IsRequired()
                    .HasColumnType("BOOLEAN");

                entity.Property(e => e.OrderID).HasColumnType("INT");

                entity.Property(e => e.ParentID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.PercentComplete).HasColumnType("FLOAT(5,2)");

                entity.Property(e => e.Start)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.Summary)
                    .IsRequired()
                    .HasColumnType("BOOLEAN");

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentID);
            });

            modelBuilder.Entity<Intraday>(entity =>
            {
                entity.ToTable("Intradays");

                entity.HasIndex(e => e.Date).HasName("IX_Intraday_Date");

                entity.Property(e => e.Close).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.High).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Low).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Open).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");

                entity.Property(e => e.Volume).HasColumnType("INT");
            });

            modelBuilder.Entity<MeetingAttendee>(entity =>
            {
                entity.ToTable("MeetingAttendees");

                entity.HasKey(e => new { e.MeetingID, e.AttendeeID });

                entity.Property(e => e.MeetingID).HasColumnType("INT");

                entity.Property(e => e.AttendeeID).HasColumnType("INT");

                entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingAttendees).HasForeignKey(d => d.MeetingID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.ToTable("Meetings");

                entity.HasKey(e => e.MeetingID);

                entity.Property(e => e.End)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.IsAllDay)
                    .IsRequired()
                    .HasColumnType("BOOLEAN");

                entity.Property(e => e.RecurrenceID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RoomID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Start)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Recurrence).WithMany(p => p.InverseRecurrence).HasForeignKey(d => d.RecurrenceID);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetails");

                entity.HasKey(e => new { e.OrderID, e.ProductID });

                entity.HasIndex(e => e.OrderID).HasName("Order_Details_OrdersOrder_Details");

                entity.HasIndex(e => e.ProductID).HasName("Order_Details_ProductsOrder_Details");

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

                entity.HasIndex(e => e.CustomerID).HasName("Orders_CustomersOrders");

                entity.HasIndex(e => e.EmployeeID).HasName("Orders_EmployeesOrders");

                entity.HasIndex(e => e.OrderDate).HasName("Orders_OrderDate");

                entity.HasIndex(e => e.ShipPostalCode).HasName("Orders_ShipPostalCode");

                entity.HasIndex(e => e.ShipVia).HasName("Orders_ShippersOrders");

                entity.HasIndex(e => e.ShippedDate).HasName("Orders_ShippedDate");

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

                entity.HasOne<Employee>(d => d.Employee).WithMany(p => p.Orders).HasForeignKey(d => d.EmployeeID);

                entity.HasOne(d => d.ShipViaNavigation).WithMany(p => p.Orders).HasForeignKey(d => d.ShipVia);
            });

            modelBuilder.Entity<OrgChartConnection>(entity =>
            {
                entity.ToTable("OrgChartConnections");

                entity.Property(e => e.FromPointX)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FromPointY)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FromShapeId)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ToPointX)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ToPointY)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ToShapeId)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<OrgChartShape>(entity =>
            {
                entity.ToTable("OrgChartShapes");

                entity.Property(e => e.Color)
                    .HasColumnType("NVARCHAR(50)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.JobTitle)
                    .HasColumnType("NVARCHAR(200)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.ProductID);

                entity.HasIndex(e => e.CategoryID).HasName("Products_CategoryID");

                entity.HasIndex(e => e.ProductName).HasName("Products_ProductName");

                entity.HasIndex(e => e.SupplierID).HasName("Products_SuppliersProducts");

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

                entity.HasOne<Category>(d => d.Category).WithMany(p => p.Products).HasForeignKey(d => d.CategoryID);

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

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("Stocks");

                entity.HasIndex(e => e.Date).HasName("IX_Stock_Date");

                entity.Property(e => e.Close).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.High).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Low).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Open).HasColumnType("FLOAT(9,3)");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");

                entity.Property(e => e.Volume).HasColumnType("INT");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");

                entity.HasKey(e => e.SupplierID);

                entity.HasIndex(e => e.CompanyName).HasName("Suppliers_CompanyName");

                entity.HasIndex(e => e.PostalCode).HasName("Suppliers_PostalCode");

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

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("Tasks");

                entity.HasKey(e => e.TaskID);

                entity.Property(e => e.End)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.IsAllDay)
                    .IsRequired()
                    .HasColumnType("BOOLEAN");

                entity.Property(e => e.OwnerID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RecurrenceID)
                    .HasColumnType("INT")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Start)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Recurrence).WithMany(p => p.InverseRecurrence).HasForeignKey(d => d.RecurrenceID);
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

            modelBuilder.Entity<UrbanArea>(entity =>
            {
                entity.ToTable("UrbanAreas");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(256)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(256)");

                entity.Property(e => e.Country_ISO3)
                    .IsRequired()
                    .HasColumnType("CHAR(3)");

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

            modelBuilder.Entity<Weather>(entity =>
            {
                entity.ToTable("Weather");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.Station)
                    .IsRequired()
                    .HasColumnType("VARCHAR(255)");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.TMax)
                    .IsRequired()
                    .HasColumnType("FLOAT(5,2)");

                entity.Property(e => e.TMin)
                    .IsRequired()
                    .HasColumnType("FLOAT(5,2)");

                entity.Property(e => e.Wind)
                    .IsRequired()
                    .HasColumnType("FLOAT(5,2)");

                entity.Property(e => e.Gust)
                    .HasColumnType("FLOAT(5,2)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Rain)
                    .IsRequired()
                    .HasColumnType("FLOAT(5,2)");

                entity.Property(e => e.Snow)
                    .HasColumnType("FLOAT(5,2)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Events)
                    .HasColumnType("VARCHAR(255)")
                    .HasDefaultValueSql("NULL");
            });
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CustomerCustomerDemo> CustomerCustomerDemo { get; set; }
        public virtual DbSet<CustomerDemographic> CustomerDemographics { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<EmployeeDirectory> EmployeeDirectories { get; set; }
        public virtual DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<GanttDependency> GanttDependencies { get; set; }
        public virtual DbSet<GanttResourceAssignment> GanttResourceAssignments { get; set; }
        public virtual DbSet<GanttResource> GanttResources { get; set; }
        public virtual DbSet<GanttTask> GanttTasks { get; set; }
        public virtual DbSet<Intraday> Intradays { get; set; }
        public virtual DbSet<MeetingAttendee> MeetingAttendees { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrgChartConnection> OrgChartConnections { get; set; }
        public virtual DbSet<OrgChartShape> OrgChartShapes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<Territory> Territories { get; set; }
        public virtual DbSet<UrbanArea> UrbanAreas { get; set; }
        public virtual DbSet<Weather> Weather { get; set; }
    }
}