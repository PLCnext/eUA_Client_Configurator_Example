// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Arp.OpcUA.UI.Core
{
    public static class UICoreServiceCollectionExtensions
    {
        public static IServiceCollection AddUICore(this IServiceCollection services)
        {
            return services.AddMudServices();
        }
    }
}
