using KendoCRUDService.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using KendoCRUDService.Hubs;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Filters;
using KendoCRUDService.Settings;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection("ApiKeys"));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider()
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseKestrel(option => option.AddServerHeader = false);
}


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

bool IsOriginAllowed(string origin)
{
    var uri = new Uri(origin);
    var allowedDomains = new string[] {
        "aspnet-core-demos-staging.azurewebsites.net",
        "aspnet-mvc-demos-staging.azurewebsites.net",
        "jquery-demos-staging.azurewebsites.net",
        "aspnet-core-demos-test.azurewebsites.net",
        "aspnet-mvc-demos-test.azurewebsites.net",
        "jquery-demos-test.azurewebsites.net",
        "blazor-repl-server-staging.azurewebsites.net",
        "wwwsit.telerik.com",
        "telerik.com",
        "demos.telerik.com",
        "127.0.0.1",
        "dojo.telerik.com",
        "runner.telerik.io",
        "stackblitz.com",
        "codesandbox.io",
        "sitdemos.telerik.com"
    };
    var wildcardDomains = new string[] {
        "stackblitz.io",
        "telerik.com",
        "webcontainer.io",
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
    DefaultTimeout = 1000,
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
builder.Services.AddSingleton<DirectoryRepository>();
builder.Services.AddSingleton<FileBrowserRepository>();
builder.Services.AddSingleton<EmployeeDirectoryRepository>();
builder.Services.AddSingleton<EmployeeOrgChartRepository>();
builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddSingleton<GanttDependencyRepository>();
builder.Services.AddSingleton<GanttResourceAssignmentsRepository>();
builder.Services.AddSingleton<GanttResourcesRepository>();
builder.Services.AddSingleton<GanttTaskRepository>();
builder.Services.AddSingleton<MeetingsRepository>();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<CategoriesRepository>();
builder.Services.AddSingleton<TaskBoardRepository>();
builder.Services.AddSingleton<TasksRepository>();
builder.Services.AddSingleton<WeatherRepository>();
builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddHealthChecks();

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
app.UseForwardedHeaders();
app.UseFileServer();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ProductHub>("/signalr/products");
app.MapHub<MeetingHub>("/signalr/meetings");
app.MapHealthChecks("/health");

app.Run();
