using Elastic.Clients.Elasticsearch;
using AshishGeneralStore.Common;

namespace AshishGeneralStore.Data
{
    public class ElasticsearchService
    {
        private readonly ElasticsearchClient _client;

        public ElasticsearchService(IConfiguration configuration)
        {
            var elasticsearchUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";

            var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri))
               .DefaultIndex(Constants.Elasticsearch.ProductsIndex);

            _client = new ElasticsearchClient(settings);
        }

        public ElasticsearchClient Client => _client;
    }
}