// Copyright PHOENIX CONTACT Electronics GmbH


using Opc.Ua;
using System.Collections.Generic;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Node of the address space browser.
    /// </summary>
    public class BrowseNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowseNode" /> class.
        /// </summary>
        /// <param name="serverIndex">Index of the server.</param>
        /// <param name="displayName">The display name of the node.</param>
        public BrowseNode(int serverIndex, string displayName)
        {
            ServerIndex = serverIndex;
            DisplayName = displayName;
        }
        /// <summary>
        /// Gets the index of the server.
        /// </summary>
        /// <value>The index of the server.</value>
        public int ServerIndex { get; }
        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        /// <value>The node identifier.</value>
        public ExpandedNodeId NodeId { get; set; }
        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; }
        /// <summary>
        /// Gets or sets the display path.
        /// </summary>
        /// <value>The display path.</value>
        public string DisplayPath { get; set; } = "";
        /// <summary>
        /// Gets or sets the childs.
        /// </summary>
        /// <value>The childs.</value>
        public IList<BrowseNode> Childs { get; set; }
        /// <summary>
        /// Gets or sets the node class.
        /// </summary>
        /// <value>The node class.</value>
        public NodeClass NodeClass { get; set; }
        /// <summary>
        /// Gets or sets the datatype.
        /// </summary>
        /// <value>The datatype.</value>
        public DataTypeModel DataType { get; set; }
    }
}
