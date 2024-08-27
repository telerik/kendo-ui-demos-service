using KendoCRUDService.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using KendoCRUDService.Hubs;
using KendoCRUDService.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider()
    .AddJsonOptions(options =>
                options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddSession();

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

builder.Services.AddSignalR();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContextFactory<DemoDbContext>(options => options.UseSqlite(connection).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddScoped<CountryRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<DetailProductRepository>();
builder.Services.AddScoped<DiagramConnectionsRepository>();
builder.Services.AddScoped<DiagramShapesRepository>();
builder.Services.AddScoped<DirectoryRepository>(x => new DirectoryRepository(builder.Environment.ContentRootPath));
builder.Services.AddScoped<EmployeeDirectoryRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<GanttDependencyRepository>();
builder.Services.AddScoped<GanttResourceAssignmentsRepository>();
builder.Services.AddScoped<GanttResourcesRepository>();
builder.Services.AddScoped<GanttTaskRepository>();
builder.Services.AddScoped<MeetingsRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<TasksRepository>();
builder.Services.AddScoped<WeatherRepository>();
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

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.UseFileServer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ProductHub>("/signalr/products");
app.MapHub<MeetingHub>("/signalr/meetings");

app.Run();
