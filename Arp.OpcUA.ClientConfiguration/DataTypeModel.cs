// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Model of the DataType.
    /// </summary>
    public class DataTypeModel
    {
        /// <summary>
        /// Gets or sets the NodeId.
        /// </summary>
        /// <value>The node identifier.</value>
        public ExpandedNodeId NodeId { get; set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
    }
}