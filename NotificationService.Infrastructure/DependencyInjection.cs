using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }
    }
}
