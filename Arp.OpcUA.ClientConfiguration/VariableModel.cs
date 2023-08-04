// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Opc.Ua;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Model of a Variable.
    /// </summary>
    public class VariableModel
    {
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public ServerDescriptor Server { get; set; }
        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        /// <value>The node identifier.</value>
        public ExpandedNodeId NodeId { get; set; }
        /// <summary>
        /// Gets or sets the datatype.
        /// </summary>
        /// <value>The datatype.</value>
        public DataTypeModel DataType { get; set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the display path.
        /// </summary>
        /// <value>The display path.</value>
        public string DisplayPath { get; set; }
    }
}