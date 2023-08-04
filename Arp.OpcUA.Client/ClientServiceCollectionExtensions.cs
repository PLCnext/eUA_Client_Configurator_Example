// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Microsoft.Extensions.DependencyInjection;

namespace Arp.OpcUA.Client
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddUAClient(this IServiceCollection services)
        {
            return services.AddSingleton<IUAClientService, UAClientService>();
        }
    }
}
