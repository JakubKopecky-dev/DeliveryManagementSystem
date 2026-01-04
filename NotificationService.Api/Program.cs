using NotificationService.Api.DependencyInjection;
using NotificationService.Api.GraphQL;
using NotificationService.Api.Hubs;
using NotificationService.Application;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Elastic;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure
builder.Services.AddInfrastructureServices();

// ElasticSearch config
builder.Services.AddElastic(builder.Configuration);

// Application
builder.Services.AddApplicationServices();

// Auth
builder.Services.AddAuthenticationAndIdentityServiceCollection(builder.Configuration);

// HttpContext accessor
builder.Services.AddHttpContextAccessor();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddAuthorization();


// Open Telemetry
builder.Services.AddOpenTelemetryService();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var bootsrapper = scope.ServiceProvider.GetRequiredService<ElasticBootstrapper>();
await bootsrapper.EnsureIndicesExistAsync();

app.UseHttpsRedirection();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

// Notification hub map
app.MapHub<NotificationHub>("hubs/notifications");

app.RunWithGraphQLCommands(args);

app.Run();

