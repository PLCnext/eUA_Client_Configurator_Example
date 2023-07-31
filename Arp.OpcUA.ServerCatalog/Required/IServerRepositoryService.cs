// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arp.OpcUA.ServerCatalog.Required
{
    public interface IServerRepositoryService
    {
        Task SetAllServers(IList<ServerConnectionModel> servers);
        Task<IList<ServerConnectionModel>> GetAllServers();
    }
}