using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using signalr_for_aspnet_core.Hubs;
using signalr_for_aspnet_core.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(corsOption => corsOption.AddPolicy("CorsPolicy", corsBuilder => 
    corsBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    ));

builder.Services.AddDbContext<SampleEntitiesDataContext>(options => options.UseSqlite());

builder.Services.AddSignalR().AddJsonProtocol(options => {
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseRouting();
app.MapHub<ProductHub>("/products");

app.Run();