// Copyright PHOENIX CONTACT Electronics GmbH


using Arp.OpcUA.Core.UAClient;
using System.Collections.Generic;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Model of the ClientConfiguration.
    /// </summary>
    public class ClientConfigurationModel
    {
        /// <summary>
        /// Gets or sets the name (only used internally).
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets the variable groups.
        /// </summary>
        /// <value>The variable groups.</value>
        public IList<ClientVariableGroupModel> VariableGroups { get; set; } = new List<ClientVariableGroupModel>();
        /// <summary>
        /// Gets the server connections.
        /// </summary>
        /// <value>The server connections.</value>
        public IList<ServerDescriptor> ServerConnections { get; set; } = new List<ServerDescriptor>();
    }
}