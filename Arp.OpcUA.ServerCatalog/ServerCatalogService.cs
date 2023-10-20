// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Arp.OpcUA.ServerCatalog.Required;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arp.OpcUA.ServerCatalog
{
    public class ServerCatalogService : IServerCatalogService
    {
        private readonly IUAClientService uaClientService;
        private readonly IServerRepositoryService serverRepository;
        private IList<ServerConnectionModel> servers;

        public ServerCatalogService(IServerRepositoryService serverRepository, IUAClientService uaClientService)
        {
            this.uaClientService = uaClientService;
            this.serverRepository = serverRepository;
        }
        public async Task StartAsync()
        {
            if (servers == null)
                servers = await serverRepository.GetAllServers();

            if (servers == null)
            {
                servers = new List<ServerConnectionModel>
                {
                    new ServerConnectionModel()
                    {
                        Enabled = true,
                        Name = "AXC F 2152",
                        Url = "opc.tcp://192.168.1.10:4840",
                        UserName = "admin",
                        SecurityMode = Opc.Ua.MessageSecurityMode.SignAndEncrypt
                    }
                };
            }

            UpdateServersOnUAClient();
        }

        private void UpdateServersOnUAClient()
        {
            uaClientService.ClearServers();
            foreach (var svr in servers.Where(svr => svr.Enabled))
                uaClientService.AddServer(svr);
        }

        public async Task<IList<ServerConnectionModel>> GetServersAsync()
        {
            servers ??= await serverRepository.GetAllServers();

            return servers ?? new List<ServerConnectionModel>();
        }

        public async Task AddServerAsync(ServerConnectionModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            servers ??= await serverRepository.GetAllServers();

            servers.Add(model);

            if (model.Enabled)
                uaClientService.AddServer(model);

            await UpdateRepository();
        }

        public async Task RemoveServerAsync(ServerConnectionModel model)
        {
            if (servers is null) throw new InvalidOperationException("not initialized");
            if (model is null) throw new ArgumentNullException(nameof(model));

            servers.Remove(model);
            try
            {
                uaClientService.RemoveServer(model.Url);
            }
            catch (InvalidOperationException)
            {
                // if not enabled or enabled was set before
            }
            await UpdateRepository();
        }
        public async Task UpdateServerAsync(ServerConnectionModel model)
        {
            if (servers is null) throw new InvalidOperationException("not initialized");
            if (model is null) throw new ArgumentNullException(nameof(model));

            if (uaClientService.FindServer(model.Url) == -1)
                UpdateServersOnUAClient(); // previous URL not known
            else if (model.Enabled)
                uaClientService.UpdateServer(model);
            else
                uaClientService.RemoveServer(model.Url);

            await UpdateRepository();
        }

        private async Task UpdateRepository()
        {
            await serverRepository.SetAllServers(servers);
        }

        public async Task<bool> TestConnectionAsync(ServerConnectionModel model)
        {
            var serverIndex = uaClientService.FindServer(model.Url);
            if (serverIndex == -1)
                return false;
            var server = await uaClientService.TestConnection(serverIndex);

            return server?.ServerStatus == 0;
        }
    }
}
