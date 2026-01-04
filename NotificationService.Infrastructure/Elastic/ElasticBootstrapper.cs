using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using NotificationService.Infrastructure.Elastic.Indexes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Elastic
{
    public class ElasticBootstrapper(ElasticsearchClient client)
    {
        private readonly ElasticsearchClient _client = client;



        public async Task EnsureIndicesExistAsync(CancellationToken ct = default)
        {
            NotificationIndexBuilder notificationIndex = new(_client);
            await notificationIndex.EnsureExistsAsync(ct);
        }


    }
}
