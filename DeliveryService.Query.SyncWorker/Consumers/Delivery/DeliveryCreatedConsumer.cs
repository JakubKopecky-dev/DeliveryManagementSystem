using Elastic.Clients.Elasticsearch;
using MassTransit;
using Shared.Contracts.Events;

namespace DeliveryService.Query.SyncWorker.Consumers.Delivery
{
    public class DeliveryCreatedConsumer(ElasticsearchClient client) : IConsumer<DeliveryCreatedEvent>
    {
        private readonly ElasticsearchClient _client = client;



        public async Task Consume(ConsumeContext<DeliveryCreatedEvent> context)
        {
            var message = context.Message;
            var ct = context.CancellationToken;

            Domain.Models.Delivery delivery = new()
            {
                Id = message.Id,
                OwnerId = message.OwnerId,
                ExternalOrderId = message.ExternalOrderId,
                CourierId = message.CourierId,
                RecipientName = message.RecipientName,
                Address = message.Address,
                Phone = message.Phone,
                PackageCount = message.PackageCount,
                PackageWeightKg = message.PackageWeightKg,
                TotalVolumeM3 = message.TotalVolumeM3,
                Status = Domain.Enums.DeliveryStatus.Created,
                CreatedAt = message.CreatedAt
            };

            await _client.IndexAsync(delivery, i => i.Index("delivery").Id(delivery.Id), ct);

        }


    }
}
