// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;
using System.Collections.Generic;

namespace Arp.OpcUA.UI.ClientConfiguration.Components
{
    public class UABrowseNode
    {
        public string Text { get; set; }
        public NodeId NodeId { get; set; }
        public IEnumerable<UABrowseNode> Childs { get; set; }
    }
}
