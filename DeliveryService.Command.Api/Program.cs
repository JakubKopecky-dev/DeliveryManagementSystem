using DeliveryService.Command.Api.GraphQL;
using DeliveryService.Command.Application;
using DeliveryService.Command.Persistence;
using DeliveryService.Command.Api.Middleware;
using HotChocolate.Types;
using DeliveryService.Command.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Persistence
builder.Services.AddPersistenceServices(builder.Configuration);

// Application
builder.Services.AddApplicationServices();

// Auth
builder.Services.AddAuthenticationAndIdentityServiceCollection(builder.Configuration);

// HttpContext accessor
builder.Services.AddHttpContextAccessor();

// Kafka
builder.Services.AddKafkaMassTransit(builder.Configuration);

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddMutationType<Mutation>()
    .AddQueryType<Query>()
    .AddAuthorization();


var app = builder.Build();

var env = app.Services.GetRequiredService<IWebHostEnvironment>();

// Apply migration
if (!env.IsEnvironment("Test"))
    app.ApplyMigrations();

// Client cancellation logging
app.UseClientCancellationLogging();

app.UseHttpsRedirection();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();


app.Run();
