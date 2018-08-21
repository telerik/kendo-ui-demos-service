using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GraphQL;
using GraphQL.Types;
using graphql_aspnet_core.Data;
using graphql_aspnet_core.Data.Contracts;
using graphql_aspnet_core.Data.Repositories;
using graphql_aspnet_core.Models.GraphQL;
using graphql_aspnet_core.Models.GraphQL.Product;

namespace graphql_aspnet_core
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            WebRootPath = env.WebRootPath;
        }

        public static string WebRootPath
        {
            get; private set;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowCredentials();
            }));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // SampleEntities db context
            services.AddDbContext<SampleEntitiesDataContext>();

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();

            // Product
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddSingleton<ProductQuery>();
            services.AddSingleton<ProductMutation>();
            services.AddSingleton<ProductType>();
            services.AddSingleton<ProductInputType>();


            services.AddSingleton<ISchema>(new ProductsSchema(new FuncDependencyResolver(type => services.BuildServiceProvider().GetService(type))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");           
            app.UseMvc();
            app.UseGraphiQl();
        }
    }
}
