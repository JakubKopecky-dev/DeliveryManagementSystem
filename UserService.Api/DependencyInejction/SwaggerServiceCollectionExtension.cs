using Microsoft.OpenApi;
using System.Reflection;

namespace UserService.Api.DependencyInejction
{
    public static class SwaggerServiceCollectionExtension
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("UserService", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "User Service API",
                    Description = "Web API for User management in DeliveryManamgnetSystem app.",
                    Contact = new OpenApiContact
                    {
                        Name = "Jakub Kopecký",
                        Url = new("https://github.com/JakubKopecky-dev")
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token in the format: Bearer <token>",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(document =>new OpenApiSecurityRequirement
                {
                    [ new OpenApiSecuritySchemeReference("Bearer",document)] = []
                });

            

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);


                options.AddServer(new OpenApiServer { Url = "/" });
                options.AddServer(new OpenApiServer { Url = "/user" });

                /*
                if (env.IsDevelopment())
                {
                    options.AddServer(new OpenApiServer { Url = "/" });
                    options.AddServer(new OpenApiServer { Url = "/user" });
                }
                else
                {
                    options.AddServer(new OpenApiServer { Url = "/user" });
                }
                */


            });

            return services;
        }
    }
}
