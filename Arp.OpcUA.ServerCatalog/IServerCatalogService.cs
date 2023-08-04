// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arp.OpcUA.ServerCatalog
{
    public interface IServerCatalogService
    {
        /// <summary>
        /// Must be called once to get server descriptions from repository and announce them to the client.
        /// </summary>
        /// <returns></returns>
        Task StartAsync();
        /// <summary>
        /// Returns a list of all know server descriptions.
        /// </summary>
        Task<IList<ServerConnectionModel>> GetServersAsync();
        /// <summary>
        /// Adds a server description to the catalog and also to the client if it is enabled.
        /// </summary>
        Task AddServerAsync(ServerConnectionModel model);
        /// <summary>
        /// Removes a server description from the catalog and also from the client if available.
        /// </summary>
        Task RemoveServerAsync(ServerConnectionModel model);
        /// <summary>
        /// Applies an update of the specified server description.
        /// </summary>
        Task UpdateServerAsync(ServerConnectionModel model);
        /// <summary>
        /// Establishes a connection to test if the communication parameters work.
        /// </summary>
        /// <returns>True if success.</returns>
        Task<bool> TestConnectionAsync(ServerConnectionModel model);
    }
}
