using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Text;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Repositories
{
    public class NotificationRepository(ElasticsearchClient client) : INotificationRepository
    {
        private readonly ElasticsearchClient _client = client;
        private const string IndexName = "notifications";


        public async Task<IReadOnlyList<Notification>> GetNotificationsByCustomerEmail(string customerEmail, CancellationToken ct = default)
        {
            var response = await _client.SearchAsync<Notification>(s => s
            .Indices(IndexName)
            .Query(q => q
                .Term(t => t
                    .Field(f => f.CustomerEmail)
                    .Value(customerEmail)
                )
            )
            .Size(100), ct);

            return [.. response.Documents];
        }



        public async Task<Notification> CreateAsync(Notification notification, CancellationToken ct = default)
        {
            var response = await _client.CreateAsync(notification, i => i.Index(IndexName).Id(notification.Id), ct);

            if (!response.IsSuccess())
                throw new Exception(response.DebugInformation);

            return notification;
        }




    }
}
