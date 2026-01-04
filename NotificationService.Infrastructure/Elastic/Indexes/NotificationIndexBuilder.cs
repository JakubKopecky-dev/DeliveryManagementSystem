using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Elastic.Indexes
{
    public class NotificationIndexBuilder(ElasticsearchClient client)
    {
        private readonly ElasticsearchClient _client = client;
        private const string IndexName = "notifications";



        public async Task EnsureExistsAsync(CancellationToken ct = default)
        {
            var exists = await _client.Indices.ExistsAsync(IndexName, ct);
            if (exists.Exists)
                return;

            Properties props = new()
            {
                ["id"] = new KeywordProperty(),
                ["title"] = new TextProperty(),
                ["customerId"] = new KeywordProperty(),
                ["customerEmail"] = new KeywordProperty(),
                ["message"] = new TextProperty(),
                ["type"] = new KeywordProperty(),
                ["createdAt"] = new DateProperty()
            };

            var response = await _client.Indices.CreateAsync(IndexName, c =>
                c.Settings(s => s.NumberOfShards(1).NumberOfReplicas(1))
                .Mappings(m => m.Properties(props)),
            ct);

            if(!response.IsValidResponse)
                throw new Exception($"Failed to create index {IndexName}: {response.DebugInformation}");

        }


    }
}
