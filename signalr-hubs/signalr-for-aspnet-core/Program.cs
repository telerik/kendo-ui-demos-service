using System;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Azure;
using Azure.AI.OpenAI;
using signalr_for_aspnet_core.Hubs;
using signalr_for_aspnet_core.Models;
using signalr_for_aspnet_core.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Services.AddSingleton(
    new AzureOpenAIClient(
        new Uri(builder.Configuration["AI:AzureOpenAI:Endpoint"] ??
            throw new InvalidOperationException("The required AzureOpenAI endpoint was not configured for this application.")),
        new AzureKeyCredential(builder.Configuration["AI:AzureOpenAI:Key"] ??
            throw new InvalidOperationException("The required AzureOpenAI Key was not configured for this application."))
    ));

    builder.Services.AddChatClient(services => services.GetRequiredService<AzureOpenAIClient>()
        .AsChatClient(builder.Configuration["AI:AzureOpenAI:Chat:ModelId"] ?? "gpt-4o-mini"));
}

builder.Services.AddCors(corsOption => corsOption.AddPolicy("CorsPolicy", corsBuilder =>
    corsBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(o => CorsExtension.IsOriginAllowed(o))
    ));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 2000; // Limit to 2000 cache entries
    options.CompactionPercentage = 0.25; // Remove 25% of entries when limit hit
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<SampleEntitiesDataContext>(options => options.UseSqlite());

builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseRouting();
app.MapHealthChecks("/health");
app.MapHub<ProductHub>("/products");
app.MapHub<MeetingHub>("/meetings");
app.MapHub<ChatHub>("/aichat");

app.Run();