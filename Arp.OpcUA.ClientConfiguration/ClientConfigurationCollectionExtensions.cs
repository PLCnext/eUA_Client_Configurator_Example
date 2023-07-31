// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Microsoft.Extensions.DependencyInjection;

namespace Arp.OpcUA.ClientConfiguration
{
    public static class ClientConfigurationCollectionExtensions
    {
        public static IServiceCollection AddClientConfiguration(this IServiceCollection services)
        {
            return services
                .AddSingleton<IEuaClientConfiguratorService, EuaClientConfiguratorService>()
                .AddSingleton<IEuaClientConfigFileEditor, EuaClientConfigFileEditor>();
        }
    }
}
