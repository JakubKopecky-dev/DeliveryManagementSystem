using System.Text.Json.Serialization;
using UserService.Api.Auth;
using UserService.Api.DependencyInejction;
using UserService.Infrastructure;
using UserService.Persistence;
using UserService.Api.Middleware;
using UserService.Api.Grpc.GrpcServices;
using Microsoft.Extensions.Diagnostics.Buffering;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

// Persistence
builder.Services.AddPersistenceServices(builder.Configuration);

// Infrastructure
builder.Services.AddInfrastructureServices();

// Identity & Authentication (JWT, UserManager, TokenGenerator)
builder.Services.AddAuthenticationAndIdentityServiceCollection(builder.Configuration);

// Grpc server
builder.Services.AddGrpc();

// Open Telemetry
builder.Services.AddOpenTelemetryService();

// Controllers & JSON setting
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerWithJwt(builder.Environment);

// Log buffer
builder.Logging.AddGlobalBuffer(options =>
{
    options.MaxBufferSizeInBytes = 100 * 1024 * 1024; // 100 MB
    options.MaxLogRecordSizeInBytes = 50 * 1024; // 50 KB
});

builder.Logging.AddPerIncomingRequestBuffer(options =>
{
    options.AutoFlushDuration = TimeSpan.FromSeconds(5);

    options.Rules.Add(new LogBufferingFilterRule(
        categoryName: "UserService.",
        logLevel: LogLevel.Warning));
});


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

// gRPC map services
app.MapGrpcService<UserGrpcService>();

// Add roles
if (!env.IsEnvironment("Test"))
    await RoleSeeder.SeedRolesAsync(app.Services);



app.Run();

