using System;
using System.Collections.Generic;
using System.Text;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastructure.Elastic
{
    public static class ElasticConfig
    {
        public static IServiceCollection AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            Uri uri = new(configuration["Elastic:Uri"]!);

            ElasticsearchClientSettings settings = new(uri);

            ElasticsearchClient client = new(settings);

            services.AddSingleton(client);
            services.AddScoped<ElasticBootstrapper>();

            return services;
        }

    }
}
