using Azure;
using Azure.AI.OpenAI;
using KendoAIService.Filters;
using KendoAIService.Interfaces;
using KendoAIService.Repositories;
using KendoAIService.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.AI;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection("ApiKeys"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(options => {
        options.AllowAnyOrigin();
        options.AllowAnyMethod();
        options.AllowAnyHeader();
    });
});

// Validate and parse AzureOpenAI endpoint URI
var endpointString = builder.Configuration["AI:AzureOpenAI:Endpoint"];
if (string.IsNullOrWhiteSpace(endpointString) || !Uri.TryCreate(endpointString, UriKind.Absolute, out var endpointUri))
{
    throw new InvalidOperationException("The required AzureOpenAI endpoint was not configured correctly for this application. Please provide a valid absolute URI.");
}

builder.Services.AddSingleton(
    new AzureOpenAIClient(
        endpointUri,
        new AzureKeyCredential(builder.Configuration["AI:AzureOpenAI:Key"] ??
            throw new InvalidOperationException("The required AzureOpenAI Key was not configured for this application."))
    ));

builder.Services.AddChatClient(services => services.GetRequiredService<AzureOpenAIClient>()
    .AsChatClient(builder.Configuration["AI:AzureOpenAI:Chat:ModelId"] ?? "gpt-4o-mini"));

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// DI Services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ApiKeyAuthFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.Run();
