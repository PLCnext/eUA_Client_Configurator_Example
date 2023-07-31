// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arp.OpcUA.Core.UAClient
{
    public interface IUAClientService : IDisposable
    {
        IEnumerable<ServerDescriptor> Servers { get; }
        void AddServer(ServerConnectionModel model);
        void RemoveServer(string url);
        void UpdateServer(ServerConnectionModel model);
        void ClearServers();
        string GetServerName(int serverIndex);
        string GetServerUri(int serverIndex);
        /// <summary>
        /// Finds a server in the Servers collection.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>ServerIndex. -1 if not found.</returns>
        int FindServer(string url);
        Task<ReferenceDescriptionCollection> Browse(int serverIndex, BrowseDescription browseDescription, bool throwOnError);
        Task<IList<BrowseReferenceDescription>> Browse(int serverIndex, ExpandedNodeId nodeId, BrowseDirection browseDirection, ExpandedNodeId referenceTypeId, bool includeSubtypes, uint nodeClassMask, uint resultMask, bool throwOnError);
        Task<Node> FetchNode(int serverIndex, ExpandedNodeId nodeId);
        Task<List<NodeId>> TranslateBrowsePaths(int serverIndex, NodeId startNodeId, NamespaceTable namespacesUris, params string[] relativePaths);
        Task<NamespaceTable> GetNamespaceUris(int serverIndex);
        Task<object> ReadValue(int serverIndex, ExpandedNodeId nodeId);
        Task<NodeId> ReadDataType(ExpandedNodeId nodeId);
        Task<NodeId> ReadDataType(int serverIndex, ExpandedNodeId nodeId);
        Task<QualifiedName> ReadBrowseName(ExpandedNodeId nodeId);
        Task<QualifiedName> ReadBrowseName(int serverIndex, ExpandedNodeId nodeId);
        Task<List<object>> ReadValues(int serverIndex, IList<NodeId> variableIds, IList<Type> expectedTypes);
        Task WriteValue(int serverIndex, NodeId nodeId, object value);
        Task<IList<object>> Call(int serverIndex, NodeId objectId, NodeId methodId, params object[] args);
        Task<ServerDescriptor> TestConnection(int serverIndex);
    }
}