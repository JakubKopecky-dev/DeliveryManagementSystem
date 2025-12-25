using CourierService.Api.Auth;
using CourierService.Api.DependencyInjection;
using CourierService.Api.GraphQL;
using CourierService.Api.Middleware;
using CourierService.Application;
using CourierService.Application.DTOs.Courier;
using CourierService.Persistence;
using HotChocolate.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Persistence (DbContext, Repositories)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application
builder.Services.AddApplicationServices();

// Auth
builder.Services.AddAuthenticationAndIdentityServiceCollection(builder.Configuration);

// GraphQL
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddAuthorization();    

var app = builder.Build();

var env = app.Services.GetRequiredService<IWebHostEnvironment>();

// Apply migration
if (!env.IsEnvironment("Test"))
    app.ApplyMigrations();


app.UseClientCancellationLogging();


// Redirection
app.UseHttpsRedirection();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.Run();
