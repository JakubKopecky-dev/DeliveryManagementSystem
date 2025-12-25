using MassTransit;
using Shared.Contracts.Events;

namespace DeliveryService.Command.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtension
    {
        public static IServiceCollection AddKafkaMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                });

                x.AddRider(rider =>
                {
                    rider.AddProducer<DeliveryCreatedEvent>("delivery-created");
                    rider.AddProducer<DeliveryStatusChangedEvent>("delivery-status-changed");
                    rider.AddProducer<DeliveryDeletedEvent>("delivery-deleted");

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(configuration["Kafka:BootstrapServers"]);
                    });
                });
            });



            return services;

        }
    }
}
