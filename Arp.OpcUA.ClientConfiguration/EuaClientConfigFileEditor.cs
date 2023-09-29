// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

/// <summary>
/// Editor for the PLCnext configuration file of the eUAClient.
/// </summary>
[DataContract(Namespace = Arp.OpcUA.ClientConfiguration.Namespaces.UAClientXsd)]
public class eUAClientConfigurationDocument { }

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Class EuaClientConfigFileEditor.
    /// Implements the <see cref="IEuaClientConfigFileEditor" />
    /// </summary>
    /// <seealso cref="IEuaClientConfigFileEditor" />
    public class EuaClientConfigFileEditor : IEuaClientConfigFileEditor
    {
        private const int DefaultCycleTime = 500;
        /// <summary>
        /// Encodes as XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        public void EncodeAsXml(Stream stream, eUAClientConfiguration configuration)
        {
            //TODO determine and remove unused namespaces
            ServiceMessageContext messageContext = new ServiceMessageContext
            {
                NamespaceUris = new NamespaceTable(configuration.NamespaceArray
                .Prepend(Opc.Ua.Namespaces.OpcUa))
            };
            var ext = new ExtensionObject(configuration);
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
            {
                var encoder = new XmlEncoder(typeof(eUAClientConfigurationDocument), writer, messageContext);
                encoder.WriteEncodeable(nameof(eUAClientConfiguration), configuration, typeof(eUAClientConfiguration));
            }
        }
        /// <summary>
        /// Decodes the XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>eUAClientConfiguration.</returns>
        public eUAClientConfiguration DecodeXml(Stream stream)
        {
            ServiceMessageContext messageContext = new ServiceMessageContext
            {
                NamespaceUris = new NamespaceTable(new[] { Opc.Ua.Namespaces.OpcUa })
            };
            using (var reader = XmlReader.Create(stream))
            {
                var decoder = new XmlDecoder(typeof(eUAClientConfigurationDocument), reader, messageContext);
                return decoder.ReadEncodeable(nameof(eUAClientConfiguration), typeof(eUAClientConfiguration)) as eUAClientConfiguration;
            }
        }
        /// <summary>
        /// Creates the configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>eUAClientConfiguration.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public eUAClientConfiguration CreateConfiguration(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return new eUAClientConfiguration
            {
                Name = name,
                ServerConnections = new eUAClientServerConnectionCollection(),
                VariableGroups = new eUAClientVariableGroupCollection()
            };
        }
        /// <summary>
        /// Adds a group to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>eUAClientVariableGroup.</returns>
        public eUAClientVariableGroup AddGroup(eUAClientConfiguration configuration)
        {
            var grp = new eUAClientVariableGroup
            {
                CycleTime = DefaultCycleTime,
                GroupType = eUAClientGroupType.Subscribe
            };
            configuration.VariableGroups.Add(grp);
            return grp;
        }
        /// <summary>
        /// Removes the group.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="index">The index.</param>
        public void RemoveGroup(eUAClientConfiguration configuration, int index)
        {
            configuration.VariableGroups.RemoveAt(index);
        }
        /// <summary>
        /// Removes a group from the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="group">The group.</param>
        public void RemoveGroup(eUAClientConfiguration configuration, eUAClientVariableGroup group)
        {
            configuration.VariableGroups.Remove(group);
        }
        /// <summary>
        /// Adds a mapping to the group.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="group">The group.</param>
        /// <returns>eUAClientNodeMapping.</returns>
        public eUAClientNodeMapping AddMapping(eUAClientConfiguration configuration, eUAClientVariableGroup group)
        {
            eUAClientNodeMapping mapping = new eUAClientNodeMapping
            {
                RemoteVariableDescriptor = new eUAClientRemoteVariableDescriptor()
            };
            group.NodeMappings.Add(mapping);
            return mapping;
        }
        /// <summary>
        /// Removes a mapping from the group.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="group">The group.</param>
        /// <param name="mapping">The mapping.</param>
        public void RemoveMapping(eUAClientConfiguration configuration, eUAClientVariableGroup group, eUAClientNodeMapping mapping)
        {
            group.NodeMappings.Remove(mapping);
        }
        /// <summary>
        /// Sets the local NodeId.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="expandedNodeId">The expanded node identifier.</param>
        public void SetLocalNodeId(eUAClientConfiguration configuration, eUAClientNodeMapping mapping, ExpandedNodeId expandedNodeId)
        {
            RegisterNamespace(configuration, expandedNodeId);
            mapping.LocalVariable = ExpandedNodeId.ToNodeId(expandedNodeId, GetNamespaceTable(configuration));
        }
        /// <summary>
        /// Sets the remote NodeId.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="expandedNodeId">The expanded node identifier.</param>
        public void SetRemoteNodeId(eUAClientConfiguration configuration, eUAClientNodeMapping mapping, string serverUri, ExpandedNodeId expandedNodeId)
        {
            if (configuration.ServerConnections.ToList().FirstOrDefault(sc => sc.Endpoint.EndpointUrl == serverUri) == null)
            {
                configuration.ServerConnections.Add(new eUAClientServerConnection
                {
                    Endpoint = new EndpointType
                    {
                        EndpointUrl = serverUri,
                    }
                });
            }
            RegisterNamespace(configuration, expandedNodeId);
            var nsTable = GetNamespaceTable(configuration);

            if (mapping.RemoteVariableDescriptor == null)
                mapping.RemoteVariableDescriptor = new eUAClientRemoteVariableDescriptor();
            mapping.RemoteVariableDescriptor.NodeId = ExpandedNodeId.ToNodeId(expandedNodeId, nsTable);
            var serverIndex = (short)configuration.ServerConnections.FindIndex(s => s.Endpoint.EndpointUrl == serverUri);
            mapping.RemoteVariableDescriptor.ServerIndex = (short)(serverIndex + 1);//0 is reserved for the local server
        }
        /// <summary>
        /// Add a server to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="securityMode">The mode how to protect messages.</param>
        /// <param name="securityPolicyUri">The URI of the security policy to use (null for "best available")</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public void AddServer(eUAClientConfiguration configuration, string serverUri, MessageSecurityMode securityMode, string securityPolicyUri, string userName, string password)
        {
            var serverConnection = configuration.ServerConnections.FirstOrDefault(s => s.Endpoint.EndpointUrl == serverUri);
            if (serverConnection == null)
            {
                serverConnection = new eUAClientServerConnection
                {
                    Endpoint = new EndpointType
                    {
                        EndpointUrl = serverUri,
                        SecurityMode = securityMode,
                        SecurityPolicyUri = securityPolicyUri,
                    }
                };
                configuration.ServerConnections.Add(serverConnection);
            }
            
            serverConnection.UserName = userName;
            serverConnection.Password = password;

            if (string.IsNullOrEmpty(userName))
                serverConnection.UserTokenType = UserTokenType.Anonymous;
            else
                serverConnection.UserTokenType = UserTokenType.UserName;
        }
        /// <summary>
        /// Removes a server from the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serverUri">The server URI.</param>
        public void RemoveServer(eUAClientConfiguration configuration, string serverUri)
        {
            var serverConnection = configuration.ServerConnections.FirstOrDefault(s => s.Endpoint.EndpointUrl == serverUri);
            if (serverConnection == null)
                return;
            configuration.ServerConnections.Remove(serverConnection);
        }
        private static void RegisterNamespace(eUAClientConfiguration configuration, ExpandedNodeId expandedNodeId)
        {
            if (expandedNodeId.NamespaceUri != null && !configuration.NamespaceArray.Contains(expandedNodeId.NamespaceUri))
                configuration.NamespaceArray.Add(expandedNodeId.NamespaceUri);
        }
        private static NamespaceTable GetNamespaceTable(eUAClientConfiguration configuration)
        {
            return new NamespaceTable(configuration.NamespaceArray.Prepend(Opc.Ua.Namespaces.OpcUa));
        }

    }
}
