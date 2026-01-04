using CourierService.Api.Auth;
using CourierService.Api.DependencyInjection;
using CourierService.Api.GraphQL;
using CourierService.Api.GraphQL.Lookups;
using CourierService.Api.Middleware;
using CourierService.Application;
using CourierService.Persistence;
using HotChocolate.Execution;
using Microsoft.Extensions.Diagnostics.Buffering;


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
    .AddTypeExtension(typeof(CourierLookup))
    .AddAuthorization()
    .UseRequest(
        next => async context =>
        {
            await next(context);

            if (context.Result is IOperationResult result &&
                result.Errors is { Count: > 0 })
            {
                if (context.ContextData.TryGetValue(nameof(HttpContext), out var value) &&
                    value is HttpContext http)
                {
                    http.Items["FlushLogBuffer"] = true;
                }
            }
        })
    .UseDefaultPipeline();


// Grpc Clients
builder.Services.AddGrpcClients(builder.Configuration);

// Open Telemetry
builder.Services.AddOpenTelemetryService();

// Log buffer
builder.Logging.AddGlobalBuffer(options =>
{
    options.MaxBufferSizeInBytes = 100 * 1024 * 1024; // 100 MB
    options.MaxLogRecordSizeInBytes = 50 * 1024; // 50KB
});

builder.Logging.AddPerIncomingRequestBuffer(options =>
{
    options.AutoFlushDuration = TimeSpan.Zero;

    options.Rules.Add(new LogBufferingFilterRule(
        categoryName: "CourierService.", 
        logLevel: LogLevel.Warning));
});



var app = builder.Build();

var env = app.Services.GetRequiredService<IWebHostEnvironment>();

// Apply migration
if (!env.IsEnvironment("Test"))
    app.ApplyMigrations();


// Redirection
app.UseHttpsRedirection();

app.UseMiddleware<LogBufferHttpMiddleware>();


// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);

app.Run();


