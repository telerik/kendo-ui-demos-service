using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using signalr_for_aspnet_core.Hubs;

namespace signalr_for_aspnet_core;

public class Startup
{
    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
        Configuration = configuration;
        WebRootPath = env.WebRootPath;
    }

    public IConfiguration Configuration { get; }

    public static string WebRootPath { get; private set; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        {
            builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
        }));

        services.AddSignalR().AddJsonProtocol(options => {
            options.PayloadSerializerOptions.PropertyNamingPolicy = null;
        });

    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseCors("CorsPolicy");

        app.UseFileServer();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ProductHub>("/products");
        });
    }
}