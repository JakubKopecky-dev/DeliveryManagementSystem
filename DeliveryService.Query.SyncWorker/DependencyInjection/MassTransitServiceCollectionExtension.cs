using DeliveryService.Query.SyncWorker.Consumers.Delivery;
using MassTransit;
using Shared.Contracts.Events;

namespace DeliveryService.Query.SyncWorker.DependencyInjection
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
                    rider.AddConsumer<DeliveryCreatedConsumer>();
                    rider.AddConsumer<DeliveryDeletedConsumer>();
                    rider.AddConsumer<DeliveryStatusChangedConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(configuration["Kafka:BootstrapServers"]);

                        k.TopicEndpoint<DeliveryCreatedEvent>("delivery-created","delivery-service-group",e =>
                            {
                                e.ConfigureConsumer<DeliveryCreatedConsumer>(context);
                            });

                        k.TopicEndpoint<DeliveryDeletedEvent>("delivery-deleted","delivery-service-group",e =>
                            {
                                e.ConfigureConsumer<DeliveryDeletedConsumer>(context);
                            });

                        k.TopicEndpoint<DeliveryStatusChangedEvent>("delivery-status-changed","delivery-service-group",e =>
                            {
                                e.ConfigureConsumer<DeliveryStatusChangedConsumer>(context);
                            });
                    });
                });
            });

            return services;
        }

    }
}
