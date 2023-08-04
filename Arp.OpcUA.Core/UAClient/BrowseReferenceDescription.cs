// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;

namespace Arp.OpcUA.Core.UAClient
{
    public class BrowseReferenceDescription
    {
        public ExpandedNodeId ReferenceTypeId { get; set; }
        public bool IsForward { get; set; }
        public ExpandedNodeId NodeId { get; set; }
        public QualifiedName BrowseName { get; set; }
        public LocalizedText DisplayName { get; set; }
        public NodeClass NodeClass { get; set; }
        public ExpandedNodeId TypeDefinition { get; set; }
    }
}
