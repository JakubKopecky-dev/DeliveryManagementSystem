using DeliveryService.Query.Api.DependencyInjection;
using DeliveryService.Query.Api.GraphQL;
using DeliveryService.Query.Api.Middleware;
using DeliveryService.Query.Application;
using DeliveryService.Query.Infrastructure;
using DeliveryService.Query.Infrastructure.Elastic;

var builder = WebApplication.CreateBuilder(args);

// Infrastrucutre
builder.Services.AddInfrastructureServices();

// ElasticSearch config
builder.Services.AddElastic(builder.Configuration);

// Application
builder.Services.AddApplicationServices();

//Auth
builder.Services.AddAuthenticationAndIdentityServiceCollection(builder.Configuration);

// HttpContext accessor
builder.Services.AddHttpContextAccessor();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddAuthorization();

var app = builder.Build();


// Client cancellation logging
app.UseClientCancellationLogging();

using var scope = app.Services.CreateScope();
var bootsrapper = scope.ServiceProvider.GetRequiredService<ElasticBootstrapper>();
await bootsrapper.EnsureIndicesExistAsync();

app.UseHttpsRedirection();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.Run();
