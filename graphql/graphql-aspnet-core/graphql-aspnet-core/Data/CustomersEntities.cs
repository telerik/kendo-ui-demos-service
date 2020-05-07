using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace graphql_aspnet_core.Data
{
    public class CustomersEntitiesDataContext : DbContext
    {
        public CustomersEntitiesDataContext() : base(new DbContextOptions<CustomersEntitiesDataContext>())
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dataDirectory = Path.Combine(Startup.WebRootPath, "App_Data");

            options.UseSqlite(@"Data Source=" + dataDirectory + System.IO.Path.DirectorySeparatorChar + @"customers.db;", x => x.SuppressForeignKeyEnforcement());
        }
        public virtual DbSet<graphql_aspnet_core.Data.Entities.Customer> Customers { get; set; }
    }
}
