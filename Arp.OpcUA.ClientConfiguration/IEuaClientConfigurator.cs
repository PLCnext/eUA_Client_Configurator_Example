// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Opc.Ua;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Configurator an eUAClient.
    /// </summary>
    public interface IEuaClientConfigurator
    {
        /// <summary>
        /// Gets the ClientConfiguration.
        /// </summary>
        /// <value>The configuration.</value>
        /// <remarks>The model should not be edited directly.</remarks>
        ClientConfigurationModel Configuration { get; }
        /// <summary>
        /// Loads the model from stream.
        /// </summary>
        /// <param name="stream">stream with data model</param>
        void SaveConfiguration(Stream stream);
        /// <summary>
        /// Exports the eUA Client configuration as XML for PLCnext.
        /// </summary>
        /// <param name="stream">Stream for XML configuration.</param>
        void ExportXmlConfiguration(Stream stream);
        /// <summary>
        /// Adds the specified server to the model.
        /// </summary>
        /// <param name="serverDescriptor">The server descriptor.</param>
        void AddServer(ServerDescriptor serverDescriptor);
        /// <summary>
        /// Removes the server from the model.
        /// </summary>
        /// <param name="serverDescriptor">The server descriptor to remove.</param>
        void RemoveServer(ServerDescriptor serverDescriptor);
        /// <summary>
        /// Adds a new group to the mode.
        /// </summary>
        /// <returns>The new group.</returns>
        ClientVariableGroupModel AddGroup();
        /// <summary>
        /// Removes the group from the model.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        void RemoveGroup(ClientVariableGroupModel group);
        /// <summary>
        /// Adds a variable mapping to the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>ClientVariableMappingModel.</returns>
        ClientVariableMappingModel AddMapping(ClientVariableGroupModel group);
        /// <summary>
        /// Sets the local variable to the specified mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="server">The server.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="displayPath">The display path.</param>
        /// <param name="dataType">Type of the data.</param>
        void SetLocalVariable(ClientVariableMappingModel mapping, string displayName, ServerDescriptor server, ExpandedNodeId nodeId, string displayPath, DataTypeModel dataType);
        /// <summary>
        /// Sets the remote variable to the mapping for browsing.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="server">The server.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="displayPath">The display path.</param>
        /// <param name="dataType">Type of the data.</param>
        void SetRemoteVariable(ClientVariableMappingModel mapping, string displayName, ServerDescriptor server, ExpandedNodeId nodeId, string displayPath, DataTypeModel dataType);
        /// <summary>
        /// Sets the remote variable to the mapping for offline editing.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="server">The server.</param>
        /// <param name="nodeIdNamespace">The node identifier namespace.</param>
        /// <param name="nodeIdentifier">The node identifier.</param>
        void SetRemoteVariable(ClientVariableMappingModel mapping, string displayName, ServerDescriptor server, string nodeIdNamespace, string nodeIdentifier);
        /// <summary>
        /// Removes the mapping from the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="mapping">The mapping.</param>
        void RemoveMapping(ClientVariableGroupModel group, ClientVariableMappingModel mapping);
        /// <summary>
        /// Browses the hierarchy of nodes in a remote OPC UA server.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;IList&lt;BrowseNode&gt;&gt;.</returns>
        Task<IList<BrowseNode>> BrowseExternalVariables(BrowseNode parent, BrowseFilter filter);
    }
}
