using KendoCRUDService.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using KendoCRUDService.Hubs;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.FileBrowser;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider()
    .AddJsonOptions(options =>
                options.JsonSerializerOptions.PropertyNamingPolicy = null);

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseKestrel(option => option.AddServerHeader = false);
}


var dbPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "data", "sample.db");
var tmpDbPath = Path.Combine("tmp", "demos-local.db");

if (builder.Environment.IsProduction())
{
    // In production, the SQLite DB is copied to a temp location to avoid locking issues.
    File.Delete(tmpDbPath);
    File.Copy(dbPath, tmpDbPath);
    dbPath = tmpDbPath;
}

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
{
    builder.WithOrigins("https://jquery-demos-staging.azurewebsites.net", "http://127.0.0.1:8080", "https://dojo.telerik.com", "https://runner.telerik.io")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));


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
    Mode = SqliteOpenMode.ReadOnly,
    Cache = SqliteCacheMode.Shared
};

var connectionString = connectionStringBuilder.ToString();
var connection = new SqliteConnection(connectionString)
{
    DefaultTimeout = 120
};
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSignalR();
builder.Services.AddDbContextFactory<DemoDbContext>(options => options.UseSqlite(connection).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddSingleton<CountryRepository>();
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddSingleton<DetailProductRepository>();
builder.Services.AddSingleton<DiagramConnectionsRepository>();
builder.Services.AddSingleton<DiagramShapesRepository>();
builder.Services.AddSingleton<DirectoryRepository>(x => new DirectoryRepository(builder.Environment.ContentRootPath));
builder.Services.AddSingleton<EmployeeDirectoryRepository>();
builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddSingleton<GanttDependencyRepository>();
builder.Services.AddSingleton<GanttResourceAssignmentsRepository>();
builder.Services.AddSingleton<GanttResourcesRepository>();
builder.Services.AddSingleton<ContentInitializer>();
builder.Services.AddSingleton<GanttTaskRepository>();
builder.Services.AddSingleton<MeetingsRepository>();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<CategoriesRepository>();
builder.Services.AddSingleton<TaskBoardRepository>();
builder.Services.AddSingleton<TasksRepository>();
builder.Services.AddSingleton<WeatherRepository>();

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
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseFileServer();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ProductHub>("/signalr/products");
app.MapHub<MeetingHub>("/signalr/meetings");

app.Run();
