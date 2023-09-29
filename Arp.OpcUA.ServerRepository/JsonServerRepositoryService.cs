// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Arp.OpcUA.ServerCatalog.Required;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arp.OpcUA.ServerRepository
{
    public class JsonServerRepositoryService : IServerRepositoryService
    {
        private string ServerConnectionsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "eUAClientConfigurator");
        private string ServerConnectionsFile = "ServerConnections.json";
        IList<ServerConnectionModel> servers;
        private readonly ILogger<JsonServerRepositoryService> logger;

        public JsonServerRepositoryService(ILogger<JsonServerRepositoryService> logger)
        {
            this.logger = logger;
        }

        public Task<IList<ServerConnectionModel>> GetAllServers()
        {
            if (servers == null)
                servers = ReadServerConnectionsFile();
            return Task.FromResult(servers);
        }

        private IList<ServerConnectionModel> ReadServerConnectionsFile()
        {
            try
            {
                return Deserialize(File.ReadAllText(Path.Combine(ServerConnectionsFolder, ServerConnectionsFile)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading ServerConnections.json");
                return null;
            }
        }
        public IList<ServerConnectionModel> Deserialize(string json)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };
                return JsonSerializer.Deserialize<IList<ServerConnectionModel>>(json, options);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Error parsing server list json");
                return null;
            }
        }
        public Task SetAllServers(IList<ServerConnectionModel> servers)
        {
            this.servers = servers;
            WriteServerConnectionsFile(servers);
            return Task.CompletedTask;
        }
        private void WriteServerConnectionsFile(IList<ServerConnectionModel> servers)
        {
            try
            {
                if (!Directory.Exists(ServerConnectionsFolder)) Directory.CreateDirectory(ServerConnectionsFolder);

                File.WriteAllText(Path.Combine(ServerConnectionsFolder, ServerConnectionsFile), Serialize(servers));
            }
            catch (IOException ex)
            {
                logger.LogError(ex, $"Error writing {Path.Combine(ServerConnectionsFolder, ServerConnectionsFile)}");
            }
        }
        public string Serialize(IList<ServerConnectionModel> servers)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            try
            {
                return JsonSerializer.Serialize(servers, options);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Error serializing server list");
                return null;
            }
        }
    }
}



