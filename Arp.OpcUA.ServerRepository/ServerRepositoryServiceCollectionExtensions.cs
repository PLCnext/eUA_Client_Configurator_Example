// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.ServerCatalog.Required;
using Microsoft.Extensions.DependencyInjection;

namespace Arp.OpcUA.ServerRepository
{
    public static class ServerRepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddServerRepository(this IServiceCollection services)
        {
            return services.AddSingleton<IServerRepositoryService, JsonServerRepositoryService>();

        }
    }
}
