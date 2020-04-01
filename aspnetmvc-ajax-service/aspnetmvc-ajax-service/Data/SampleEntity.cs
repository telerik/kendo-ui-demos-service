using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetmvc_ajax_service.Data
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
        public virtual DbSet<Customer> Customers { get; set; }
    }
}
