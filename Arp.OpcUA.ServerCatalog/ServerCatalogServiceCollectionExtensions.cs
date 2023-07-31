// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Microsoft.Extensions.DependencyInjection;

namespace Arp.OpcUA.ServerCatalog
{
    public static class ServerCatalogServiceCollectionExtensions
    {
        public static IServiceCollection AddServerCatalog(this IServiceCollection services)
        {
            return services.AddSingleton<IServerCatalogService, ServerCatalogService>();
        }
    }
}
