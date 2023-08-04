// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Editor for eUAClient configurations.
    /// Implements the <see cref="IEuaClientConfigurator" />
    /// </summary>
    /// <seealso cref="IEuaClientConfigurator" />
    public class EuaClientConfigurator : IEuaClientConfigurator
    {
        private readonly IUAClientService uaClientService;
        private readonly IEuaClientConfigFileEditor euaClientConfigFileEditor;

        /// <summary>
        /// Gets the ClientConfiguration.
        /// </summary>
        /// <value>The configuration.</value>
        /// <remarks>The model should not be edited directly.</remarks>
        public ClientConfigurationModel Configuration { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuaClientConfigurator" /> class.
        /// </summary>
        /// <param name="uaClientService">The UA Client service.</param>
        /// <param name="euaClientConfigFileEditor">The eUAClient configuration file editor.</param>
        /// <param name="name">The name of the configuration.</param>
        public EuaClientConfigurator(IUAClientService uaClientService, IEuaClientConfigFileEditor euaClientConfigFileEditor, string name)
        {
            this.uaClientService = uaClientService;
            this.euaClientConfigFileEditor = euaClientConfigFileEditor;
            Configuration = new ClientConfigurationModel { Name = name };
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EuaClientConfigurator" /> class.
        /// </summary>
        /// <param name="uaClientService">The UA Client service.</param>
        /// <param name="euaClientConfigFileEditor">The eUAClient configuration file editor.</param>
        /// <param name="model">The model.</param>
        public EuaClientConfigurator(IUAClientService uaClientService, IEuaClientConfigFileEditor euaClientConfigFileEditor, ClientConfigurationModel model)
        {
            this.uaClientService = uaClientService;
            this.euaClientConfigFileEditor = euaClientConfigFileEditor;
            Configuration = model;
        }
        /// <summary>
        /// Browses the hierarchy of nodes in a remote OPC UA server.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>IList&lt;BrowseNode&gt;.</returns>
        /// <exception cref="ArgumentNullException">parent</exception>
        /// <exception cref="ArgumentNullException">filter</exception>
        public async Task<IList<BrowseNode>> BrowseExternalVariables(BrowseNode parent, BrowseFilter filter)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            if (filter is null) throw new ArgumentNullException(nameof(filter));

            IList<BrowseNode> ret = new List<BrowseNode>();
            try
            {
                var refs = await uaClientService.Browse(
                    serverIndex: parent.ServerIndex,
                    nodeId: parent.NodeId,
                    browseDirection: BrowseDirection.Forward,
                    referenceTypeId: Opc.Ua.ReferenceTypeIds.HierarchicalReferences,
                    includeSubtypes: true,
                    nodeClassMask: 0,
                    resultMask: (uint)(BrowseResultMask.DisplayName | BrowseResultMask.NodeClass),
                    throwOnError: false);

                if (refs.Count == 0)
                    return ret;

                foreach (var r in refs)
                {
                    string displayName = r.DisplayName.Text;
                    var node = new BrowseNode(parent.ServerIndex, displayName)
                    {
                        NodeClass = r.NodeClass,
                        NodeId = r.NodeId,
                        DisplayPath = parent.DisplayPath + " | " + displayName
                    };
                    if (node.NodeClass == NodeClass.Variable)
                    {
                        var dataTypeId = await uaClientService.ReadDataType(node.NodeId);
                        var dataTypeName = await uaClientService.ReadBrowseName(parent.ServerIndex, dataTypeId);
                        node.DataType = new DataTypeModel
                        {
                            DisplayName = dataTypeName?.Name,
                            NodeId = dataTypeId
                        };
                    }
                    if (!MatchFilter(filter, node))
                        continue;
                    ret.Add(node);
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }
        private static bool MatchFilter(BrowseFilter filter, BrowseNode node)
        {
            if (filter == null)
                return true;
            switch (node.NodeClass)
            {
                case NodeClass.Object:
                    return true;
                case NodeClass.Variable:
                    if (filter.DataType?.NodeId == null)
                        return true;
                    if (filter.DataType.NodeId == node.DataType?.NodeId)
                        return true;
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Adds a new group to the mode.
        /// </summary>
        /// <returns>The new group.</returns>
        public ClientVariableGroupModel AddGroup()
        {
            var ret = new ClientVariableGroupModel();
            Configuration.VariableGroups.Add(ret);
            return ret;
        }

        /// <summary>
        /// Adds a variable mapping to the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>ClientVariableMappingModel.</returns>
        /// <exception cref="ArgumentNullException">group</exception>
        public ClientVariableMappingModel AddMapping(ClientVariableGroupModel group)
        {
            if (group is null) throw new ArgumentNullException(nameof(group));

            var mapping = new ClientVariableMappingModel();
            group.VariableMappings.Add(mapping);
            return mapping;
        }

        /// <summary>
        /// Sets the local variable to the specified mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="server">The server.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="displayPath">The display path.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        /// <exception cref="ArgumentNullException">server</exception>
        /// <exception cref="ArgumentNullException">nodeId</exception>
        /// <exception cref="ArgumentException">'{nameof(displayName)}' cannot be null or whitespace. - displayName</exception>
        public void SetLocalVariable(ClientVariableMappingModel mapping, string displayName, ServerDescriptor server, ExpandedNodeId nodeId, string displayPath, DataTypeModel dataType)
        {
            if (mapping is null) throw new ArgumentNullException(nameof(mapping));
            if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException($"'{nameof(displayName)}' cannot be null or whitespace.", nameof(displayName));
            if (server is null) throw new ArgumentNullException(nameof(server));
            if (nodeId is null) throw new ArgumentNullException(nameof(nodeId));

            mapping.LocalVariable = new VariableModel
            {
                NodeId = RemoveServerIndexFromNodeId(nodeId),
                Server = server,
                DataType = dataType,
                DisplayName = displayName,
                DisplayPath = displayPath
            };
        }

        /// <summary>
        /// Sets the remote variable to the mapping for browsing.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="server">The server.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="displayPath">The display path.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        /// <exception cref="ArgumentNullException">server</exception>
        /// <exception cref="ArgumentNullException">nodeId</exception>
        /// <exception cref="ArgumentException">'{nameof(displayName)}' cannot be null or whitespace. - displayName</exception>
        public void SetRemoteVariable(ClientVariableMappingModel mapping, string displayName, ServerDescriptor server, ExpandedNodeId nodeId, string displayPath, DataTypeModel dataType)
        {
            if (mapping is null) throw new ArgumentNullException(nameof(mapping));
            if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException($"'{nameof(displayName)}' cannot be null or whitespace.", nameof(displayName));
            if (server is null) throw new ArgumentNullException(nameof(server));
            if (nodeId is null) throw new ArgumentNullException(nameof(nodeId));

            mapping.RemoteVariable = new VariableModel
            {
                NodeId = RemoveServerIndexFromNodeId(nodeId),
                Server = server,
                DataType = dataType,
                DisplayName = displayName,
                DisplayPath = displayPath
            };
        }
        private static ExpandedNodeId RemoveServerIndexFromNodeId(ExpandedNodeId nodeId)
        {
            return new ExpandedNodeId(nodeId.Identifier, nodeId.NamespaceIndex, nodeId.NamespaceUri, 0);
        }

        /// <summary>
        /// Sets the remote variable to the mapping for offline editing.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="server">The server.</param>
        /// <param name="nodeIdNamespace">The node identifier namespace.</param>
        /// <param name="nodeIdentifier">The node identifier.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        /// <exception cref="ArgumentNullException">server</exception>
        /// <exception cref="ArgumentNullException">nodeIdNamespace</exception>
        /// <exception cref="ArgumentNullException">nodeIdentifier</exception>
        /// <exception cref="ArgumentException">'{nameof(displayName)}' cannot be null or whitespace. - displayName</exception>
        public void SetRemoteVariable(ClientVariableMappingModel mapping, string displayName, ServerDescriptor server, string nodeIdNamespace, string nodeIdentifier)
        {
            if (mapping is null) throw new ArgumentNullException(nameof(mapping));
            if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException($"'{nameof(displayName)}' cannot be null or whitespace.", nameof(displayName));
            if (server is null) throw new ArgumentNullException(nameof(server));
            if (nodeIdNamespace is null) throw new ArgumentNullException(nameof(nodeIdNamespace));
            if (nodeIdentifier is null) throw new ArgumentNullException(nameof(nodeIdentifier));

            var nodeId = new ExpandedNodeId(NodeId.Parse(nodeIdentifier), nodeIdNamespace);
            mapping.RemoteVariable = new VariableModel
            {
                NodeId = nodeId,
                Server = server,
                DataType = null,
                DisplayName = displayName,
                DisplayPath = null
            };
        }
        /// <summary>
        /// Adds the specified server to the model.
        /// </summary>
        /// <param name="serverDescriptor">The server descriptor.</param>
        /// <exception cref="ArgumentNullException">serverDescriptor</exception>
        public void AddServer(ServerDescriptor serverDescriptor)
        {
            if (serverDescriptor is null) throw new ArgumentNullException(nameof(serverDescriptor));

            Configuration.ServerConnections.Add(serverDescriptor);
        }

        /// <summary>
        /// Exports the eUA Client configuration as XML for PLCnext.
        /// </summary>
        /// <param name="stream">Stream for XML configuration.</param>
        /// <exception cref="ArgumentNullException">stream</exception>
        public void ExportXmlConfiguration(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            CopyServersFromOpcUAClient();

            eUAClientConfiguration fileConfig = CreateEuaClientConfiguration();

            euaClientConfigFileEditor.EncodeAsXml(stream, fileConfig);
        }

        private void CopyServersFromOpcUAClient()
        {
            Configuration.ServerConnections.Clear();
            foreach (var svr in uaClientService.Servers)
                AddServer(svr);
        }

        /// <summary>
        /// Removes the group from the model.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        /// <exception cref="ArgumentNullException">group</exception>
        public void RemoveGroup(ClientVariableGroupModel group)
        {
            if (group is null) throw new ArgumentNullException(nameof(group));

            Configuration.VariableGroups.Remove(group);
        }

        /// <summary>
        /// Removes the mapping from the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="mapping">The mapping.</param>
        /// <exception cref="ArgumentNullException">group</exception>
        /// <exception cref="ArgumentNullException">mapping</exception>
        public void RemoveMapping(ClientVariableGroupModel group, ClientVariableMappingModel mapping)
        {
            if (group is null) throw new ArgumentNullException(nameof(group));
            if (mapping is null) throw new ArgumentNullException(nameof(mapping));

            group.VariableMappings.Remove(mapping);
        }

        /// <summary>
        /// Removes the server from the model.
        /// </summary>
        /// <param name="serverDescriptor">The server descriptor to remove.</param>
        /// <exception cref="ArgumentNullException">serverDescriptor</exception>
        public void RemoveServer(ServerDescriptor serverDescriptor)
        {
            if (serverDescriptor is null) throw new ArgumentNullException(nameof(serverDescriptor));

            Configuration.ServerConnections.Remove(serverDescriptor);
        }

        /// <summary>
        /// Saves the model to stream.
        /// </summary>
        /// <param name="stream">Stream to store the model</param>
        /// <exception cref="ArgumentNullException">stream</exception>
        /// <exception cref="NotImplementedException"></exception>
        public void SaveConfiguration(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            // The corresponding Load method is at IEuaClientConfiguratorService

            CopyServersFromOpcUAClient();

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new ExpandedNodeIdJsonConverter());
            serializeOptions.WriteIndented = true;
            JsonSerializer.Serialize(stream, Configuration, serializeOptions);
        }

        private eUAClientConfiguration CreateEuaClientConfiguration()
        {
            var fileConfig = euaClientConfigFileEditor.CreateConfiguration(Configuration.Name);

            foreach (var svr in Configuration.ServerConnections)
                euaClientConfigFileEditor.AddServer(fileConfig, svr.ServerUrl, svr.SecurityMode, svr.SecurityPolicyUri, svr.UserName, svr.Password);

            foreach (var group in Configuration.VariableGroups)
            {
                var fileGroup = euaClientConfigFileEditor.AddGroup(fileConfig);

                fileGroup.CycleTime = group.CycleTime;
                fileGroup.GroupType = group.GroupType;

                foreach (var mapping in group.VariableMappings)
                {
                    if (mapping.LocalVariable?.NodeId == null)
                        continue;
                    var fileMapping = euaClientConfigFileEditor.AddMapping(fileConfig, fileGroup);
                    euaClientConfigFileEditor.SetLocalNodeId(fileConfig, fileMapping, mapping.LocalVariable.NodeId);

                    if (mapping.RemoteVariable?.NodeId == null)
                        continue;
                    var remoteServerUri = uaClientService.GetServerUri(mapping.RemoteVariable.Server.ServerIndex);
                    euaClientConfigFileEditor.SetRemoteNodeId(fileConfig, fileMapping, remoteServerUri, mapping.RemoteVariable.NodeId);
                }
            }

            return fileConfig;
        }
    }
}
