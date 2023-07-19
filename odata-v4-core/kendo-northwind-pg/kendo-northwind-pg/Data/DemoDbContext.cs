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
    }
}
