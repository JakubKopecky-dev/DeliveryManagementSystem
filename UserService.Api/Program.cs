using System.Text.Json.Serialization;
using UserService.Api.Auth;
using UserService.Api.DependencyInejction;
using UserService.Infrastructure;
using UserService.Persistence;
using UserService.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Persistence
builder.Services.AddPersistenceServices(builder.Configuration);

// Infrastructure
builder.Services.AddInfrastructureServices();

// Identity & Authentication (JWT, UserManager, TokenGenerator)
builder.Services.AddAuthenticationAndIdentityServiceCollection(builder.Configuration);

// Controllers & JSON setting
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerWithJwt(builder.Environment);

var app = builder.Build();


var env = app.Services.GetRequiredService<IWebHostEnvironment>();

// Apply migration
if (!env.IsEnvironment("Test"))
    app.ApplyMigrations();

// Swagger
if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("./UserService/swagger.json", "UserService - v1");
    });
}

// Global error handling
app.UseGlobalExceptionHandling();

// Client cancellation logging
app.UseClientCancellationLogging();

// Authentitaction and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller map
app.MapControllers();

// Add roles
if (!env.IsEnvironment("Test"))
    await RoleSeeder.SeedRolesAsync(app.Services);



app.Run();

