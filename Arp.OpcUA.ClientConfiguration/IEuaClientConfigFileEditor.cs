// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;
using System.IO;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Editor for the PLCnext configuration file of the eUAClient.
    /// </summary>
    public interface IEuaClientConfigFileEditor
    {
        /// <summary>
        /// Creates the configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>eUAClientConfiguration.</returns>
        eUAClientConfiguration CreateConfiguration(string name);
        /// <summary>
        /// Encodes as XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        void EncodeAsXml(Stream stream, eUAClientConfiguration configuration);
        /// <summary>
        /// Decodes the XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>eUAClientConfiguration.</returns>
        eUAClientConfiguration DecodeXml(Stream stream);
        /// <summary>
        /// Add a server to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="securityMode">The mode how to protect messages.</param>
        /// <param name="securityPolicyUri">The URI of the security policy to use (null for "best available")</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        void AddServer(eUAClientConfiguration configuration, string serverUri, MessageSecurityMode securityMode, string securityPolicyUri, string userName, string password);
        /// <summary>
        /// Removes a server from the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serverUri">The server URI.</param>
        void RemoveServer(eUAClientConfiguration configuration, string serverUri);
        /// <summary>
        /// Adds a group to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>eUAClientVariableGroup.</returns>
        eUAClientVariableGroup AddGroup(eUAClientConfiguration configuration);
        /// <summary>
        /// Removes a group from the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="group">The group.</param>
        void RemoveGroup(eUAClientConfiguration configuration, eUAClientVariableGroup group);
        /// <summary>
        /// Adds a mapping to the group.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="group">The group.</param>
        /// <returns>eUAClientNodeMapping.</returns>
        eUAClientNodeMapping AddMapping(eUAClientConfiguration configuration, eUAClientVariableGroup group);
        /// <summary>
        /// Removes a mapping from the group.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="group">The group.</param>
        /// <param name="mapping">The mapping.</param>
        void RemoveMapping(eUAClientConfiguration configuration, eUAClientVariableGroup group, eUAClientNodeMapping mapping);
        /// <summary>
        /// Sets the local NodeId.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="expandedNodeId">The expanded node identifier.</param>
        void SetLocalNodeId(eUAClientConfiguration configuration, eUAClientNodeMapping mapping, ExpandedNodeId expandedNodeId);
        /// <summary>
        /// Sets the remote NodeId.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="expandedNodeId">The expanded node identifier.</param>
        void SetRemoteNodeId(eUAClientConfiguration configuration, eUAClientNodeMapping mapping, string serverUri, ExpandedNodeId expandedNodeId);
    }
}