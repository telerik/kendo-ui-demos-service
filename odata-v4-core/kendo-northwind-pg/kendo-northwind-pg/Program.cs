using kendo_northwind_pg;
using kendo_northwind_pg.Controllers;
using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Data.Repositories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var edmModel = OdataModels.GetEdmModel();
var batchHandler = new DefaultODataBatchHandler();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
});

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null)
    .AddOData(opt => opt.Select()
                            .OrderBy()
                            .SetMaxTop(1000)
                            .EnableQueryFeatures(1000)
                            .Filter()
                            .Count()
                            .Expand()
                            .AddRouteComponents("", edmModel, batchHandler));
builder.Services.AddSession();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "data", "sample.db");
var tmpDir = Path.Combine(builder.Environment.ContentRootPath, "tmp");
var tmpDbPath = Path.Combine(tmpDir, "demos-local.db");

if (builder.Environment.IsProduction())
{
    // In production, the SQLite DB is copied to a temp location to avoid locking issues.
    if (!Directory.Exists(tmpDir))
    {
        Console.WriteLine($"Creating directory: {tmpDir}");
        Directory.CreateDirectory(tmpDir);
    }

    if (File.Exists(tmpDbPath))
    {
        Console.WriteLine($"Deleting existing file: {tmpDbPath}");
        File.Delete(tmpDbPath);
    }

    File.Copy(dbPath, tmpDbPath);
    dbPath = tmpDbPath;
}

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var connectionStringBuilder = new SqliteConnectionStringBuilder
{
    DataSource = dbPath,
    Mode = SqliteOpenMode.ReadWriteCreate,
    Cache = SqliteCacheMode.Shared
};

var connectionString = connectionStringBuilder.ToString();
var connection = new SqliteConnection(connectionString)
{
    DefaultTimeout = 1000
};
bool IsOriginAllowed(string origin)
{
    var uri = new Uri(origin);
    var allowedDomains = new string[] {
        "aspnet-core-demos-staging.azurewebsites.net",
        "aspnet-mvc-demos-staging.azurewebsites.net",
        "jquery-demos-staging.azurewebsites.net",
        "sitdemos.telerik.com",
        "127.0.0.1",
        "telerik.com",
        "demos.telerik.com",
        "dojo.telerik.com",
        "stackblitz.com",
        "codesandbox.io",
        "runner.telerik.io"
    };
    var wildcardDomains = new string[] {
        "stackblitz.io",
        "webcontainer.io",
        "telerik.com",
        "csb.app",
    };

    bool isAllowed = false;
    foreach (var allowedDomain in wildcardDomains)
    {
        if (uri.Host.EndsWith(allowedDomain, StringComparison.OrdinalIgnoreCase))
        {
            isAllowed = true;
            break;
        }
    }

    foreach (var allowedDomain in allowedDomains)
    {
        if (uri.Host.Equals(allowedDomain, StringComparison.OrdinalIgnoreCase))
        {
            isAllowed = true;
            break;
        }
    }

    if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) {
        isAllowed = true;
    }

    return isAllowed;
}

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
{
    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
        .SetIsOriginAllowed(d=> IsOriginAllowed(d))
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContext<DemoDbContext>(options => options.UseSqlite(connection).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddSingleton<ProductRepository>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseForwardedHeaders();
app.UseMiddleware<ODataBatchBodyPreProcessMiddleware>();
app.UseODataBatching();
app.UseMiddleware<ODataBatchHttpContextMiddleware>();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");

app.Run();
