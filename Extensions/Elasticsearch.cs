using Elasticsearch.Net;
using Nest;
using System.Runtime.CompilerServices;

namespace Procergs.ShoppingList.Service.Extensions
{
    public static class Elasticsearch
    {           
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            Uri uri1 = new Uri(configuration["Elasticsearch:Uri1"]);
            Uri[] urls = new Uri[] { uri1 };

            var index = configuration["Elasticsearch:Index"];
            var user = configuration["Elasticsearch:User"];
            var password = configuration["Elasticsearch:Password"];

            var connectionPool = new StaticConnectionPool(urls);

            var connectionSettings = new ConnectionSettings(connectionPool);

            connectionSettings.DisablePing().BasicAuthentication(user, password);

            connectionSettings.EnableDebugMode();
            connectionSettings.DisableDirectStreaming(true);

            var client = new ElasticClient(connectionSettings);

            services.AddSingleton<IElasticClient>(client);
        } 
    }
}
