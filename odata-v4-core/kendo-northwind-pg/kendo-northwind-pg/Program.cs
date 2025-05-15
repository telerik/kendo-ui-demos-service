using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var edmModel = OdataModels.GetEdmModel();

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider()
    .AddOData(opt => opt.Select()
                            .OrderBy()
                            .Filter()
                            .Count()
                            .Expand()
                            .AddRouteComponents("odata", edmModel)); 
builder.Services.AddSession();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "data", "sample.db");
var tmpDbPath = Path.Combine("tmp", "demos-local.db");

if (builder.Environment.IsProduction())
{
    // In production, the SQLite DB is copied to a temp location to avoid locking issues.
    File.Delete(tmpDbPath);
    File.Copy(dbPath, tmpDbPath);
    dbPath = tmpDbPath;
}

var connectionStringBuilder = new SqliteConnectionStringBuilder
{
    DataSource = dbPath,
    Mode = SqliteOpenMode.ReadOnly,
    Cache = SqliteCacheMode.Shared
};

var connectionString = connectionStringBuilder.ToString();
var connection = new SqliteConnection(connectionString)
{
    DefaultTimeout = 120
};

builder.Services.AddDbContext<DemoDbContext>(options => options.UseSqlite(connection).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
{
    builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
