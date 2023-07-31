// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arp.OpcUA.Client
{
    public class UAClientService : IUAClientService, IDisposable
    {
        int indexCounter;
        readonly Dictionary<int, ServerDescriptor> servers = new();
        readonly Dictionary<int, ISession> sessions = new();
        private readonly ILogger<UAClientService> logger;
        object lockObject = new();

        public UAClientService(ILogger<UAClientService> logger)
        {
            this.logger = logger;
            Utils.SetLogger(logger);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                ClearServers();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public IEnumerable<ServerDescriptor> Servers => servers.Values;

        public void AddServer(ServerConnectionModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            logger.LogTrace("AddServer {Name}: {Url}", model.Name, model.Url);
            lock (lockObject)
            {
                var svr = new ServerDescriptor(model.Name, model.Url)
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    SecurityMode = model.SecurityMode,
                    SecurityPolicyUri = model.SecurityPolicyUri,
                    ServerIndex = GetNewIndex()
                };
                servers.Add(svr.ServerIndex, svr);
            }
        }
        public void RemoveServer(string url)
        {
            logger.LogTrace("RemoveServer {Url}", url);
            lock (lockObject)
            {
                var serverKey = servers.First(kv => kv.Value.ServerUrl == url).Key;
                servers.Remove(serverKey);
                FreeSession(serverKey);
            }
        }

        public void UpdateServer(ServerConnectionModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            logger.LogTrace("UpdateServer {Name}: {Url}", model.Name, model.Url);
            lock (lockObject)
            {
                var serverKey = servers.First(kv => kv.Value.ServerUrl == model.Url).Key;
                servers[serverKey] = new ServerDescriptor(model.Name, model.Url)
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    SecurityMode = model.SecurityMode,
                    SecurityPolicyUri = model.SecurityPolicyUri,
                    ServerIndex = serverKey
                };
                FreeSession(serverKey);
            }
        }
        int GetNewIndex()
        {
            return Interlocked.Increment(ref indexCounter);
        }

        public void ClearServers()
        {
            logger.LogTrace("ClearServers");
            lock (lockObject)
            {
                FreeAllSessions();
                servers.Clear();
            }
        }

        public void FreeAllSessions()
        {
            foreach (var kv in sessions)
                FreeSession(kv.Key);
        }

        public void FreeSession(int serverIndex)
        {
            logger.LogTrace("ClearSession {ServerIndex}", serverIndex);
            lock (lockObject)
            {
                if (sessions.TryGetValue(serverIndex, out var session) && session != null)
                {
                    session.Close(100);
                    session.Dispose();
                }
                sessions[serverIndex] = null;
            }
        }
        public async Task<ISession> GetSession(int serverIndex)
        {
            ServerDescriptor serverDesc = null;
            lock (lockObject)
            {
                if (!servers.TryGetValue(serverIndex, out serverDesc))
                {
                    throw new InvalidOperationException("Unknown server handle");
                }
                sessions.TryGetValue(serverIndex, out var session);

                if (false == session?.Connected)
                {
                    session.Dispose();
                    session = null;
                    sessions[serverIndex] = null;
                }
                if (session != null)
                    return session;
            }
            var s = await CreateSession(serverDesc);
            lock (lockObject)
            {
                sessions[serverIndex] = s;
            }
            return s;
        }

        private async Task<ISession> CreateSession(ServerDescriptor serverDesc)
        {
            logger.LogTrace("CreateSession {ServerName}", serverDesc.Name);
            OpcClient opc = null;
            if (string.IsNullOrWhiteSpace(serverDesc.UserName))
            {
                opc = new OpcClient(serverDesc.ServerUrl, true, 10000, serverDesc.SecurityMode, serverDesc.SecurityPolicyUri);
            }
            else
            {
                opc = new OpcClient(serverDesc.ServerUrl, true, 10000, serverDesc.SecurityMode, serverDesc.SecurityPolicyUri, serverDesc.UserName, serverDesc.Password);
            }

            return await opc.Connect();
        }

        public string GetServerName(int serverIndex)
        {
            lock (lockObject)
            {
                servers.TryGetValue(serverIndex, out var serverDesc);
                return serverDesc?.Name;
            }
        }
        public string GetServerUri(int serverIndex)
        {
            lock (lockObject)
            {
                servers.TryGetValue(serverIndex, out var serverDesc);
                return serverDesc?.ServerUrl;
            }
        }
        private ServerDescriptor GetServer(int serverIndex)
        {
            lock (lockObject)
            {
                ServerDescriptor server;
                servers.TryGetValue(serverIndex, out server);
                return server;
            }
        }

        public int FindServer(string url)
        {
            lock (lockObject)
            {
                var server = servers.Values.FirstOrDefault(svr => svr.ServerUrl == url);
                return server == null ? -1 : server.ServerIndex;
            }
        }

        async Task<T> WithSession<T>(int serverIndex, Func<ISession, Task<T>> action)
        {
            var session = await GetSession(serverIndex);
            try
            {
                return await action?.Invoke(session);
            }
            catch (ServiceResultException ex)
            {
                if (ex.Result.StatusCode == StatusCodes.BadSessionClosed
                    || ex.Result.StatusCode == StatusCodes.BadConnectionClosed
                    || ex.Result.StatusCode == StatusCodes.BadSessionIdInvalid
                    || ex.Result.StatusCode == StatusCodes.BadSecureChannelClosed)
                    FreeSession(serverIndex);
                throw;
            }
        }
        async Task WithSession(int serverIndex, Func<ISession, Task> action)
        {
            var session = await GetSession(serverIndex);
            try
            {
                await action?.Invoke(session);
            }
            catch (ServiceResultException ex)
            {
                if (ex.Result.StatusCode == StatusCodes.BadSessionClosed
                    || ex.Result.StatusCode == StatusCodes.BadConnectionClosed
                    || ex.Result.StatusCode == StatusCodes.BadSecureChannelClosed)
                    FreeSession(serverIndex);
                throw;
            }
        }
        public Task<object> ReadValue(int serverIndex, ExpandedNodeId nodeId)
        {
            return ReadAttribute(serverIndex, nodeId, Attributes.Value);
        }
        public Task<object> ReadValue(ServerDescriptor server, ExpandedNodeId nodeId)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            return ReadAttribute(server.ServerIndex, nodeId, Attributes.Value);
        }
        public async Task<NodeId> ReadDataType(int serverIndex, ExpandedNodeId nodeId)
        {
            return await ReadAttribute(serverIndex, nodeId, Attributes.DataType) as NodeId;
        }
        public async Task<NodeId> ReadDataType(ExpandedNodeId nodeId)
        {
            if (nodeId == null) throw new ArgumentNullException(nameof(nodeId));
            return await ReadAttribute(nodeId, Attributes.DataType) as NodeId;
        }
        public async Task<QualifiedName> ReadBrowseName(int serverIndex, ExpandedNodeId nodeId)
        {
            return await ReadAttribute(serverIndex, nodeId, Attributes.BrowseName) as QualifiedName;
        }
        public async Task<QualifiedName> ReadBrowseName(ExpandedNodeId nodeId)
        {
            if (nodeId == null) throw new ArgumentNullException(nameof(nodeId));
            return await ReadAttribute(nodeId, Attributes.BrowseName) as QualifiedName;
        }
        Task<object> ReadAttribute(ExpandedNodeId nodeId, uint attributeId)
        {
            return ReadAttribute((int)nodeId.ServerIndex, nodeId, attributeId);
        }
        Task<object> ReadAttribute(int serverIndex, ExpandedNodeId nodeId, uint attributeId)
        {
            return WithSession(serverIndex, (session) =>
            {
                ReadValueId nodeToRead = new ReadValueId();
                nodeToRead.NodeId = ExpandedNodeId.ToNodeId(nodeId, session.NamespaceUris);
                nodeToRead.AttributeId = attributeId;
                //nodeToRead.DataEncoding = m_encodingName;

                ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
                nodesToRead.Add(nodeToRead);

                // read the attributes.
                DataValueCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                session.Read(
                    null,
                    0,
                    TimestampsToReturn.Neither,
                    nodesToRead,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, nodesToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

                // check for error.
                if (StatusCode.IsBad(results[0].StatusCode))
                {
                    return null;
                }
                else
                {
                    return Task.FromResult(results[0].Value);
                }
            });
        }
        public Task<List<object>> ReadValues(int serverIndex, IList<NodeId> variableIds, IList<Type> expectedTypes)
        {
            logger.LogTrace("ReadValues {ServerName}, {VariableIds}", serverIndex, variableIds);
            return WithSession(serverIndex, (session) =>
            {
                List<ServiceResult> errors = null;
                List<object> results = null;
                session.ReadValues(variableIds, expectedTypes, out results, out errors);

                return Task.FromResult(results);
            });
        }
        public Task WriteValue(int serverIndex, NodeId nodeId, object value)
        {
            logger.LogTrace("WriteValue {ServerName}, {NodeId}", serverIndex, nodeId);
            return WithSession(serverIndex, (session) =>
            {
                WriteValue nodeToWrite = new WriteValue();
                nodeToWrite.NodeId = nodeId;
                nodeToWrite.AttributeId = Attributes.Value;
                nodeToWrite.Value = new DataValue();
                nodeToWrite.Value.WrappedValue = new Variant(value);

                WriteValueCollection nodesToWrite = new WriteValueCollection();
                nodesToWrite.Add(nodeToWrite);

                // read the attributes.
                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                ResponseHeader responseHeader = session.Write(
                    null,
                    nodesToWrite,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, nodesToWrite);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToWrite);

                // check for error.
                if (StatusCode.IsBad(results[0]))
                {
                    throw ServiceResultException.Create(results[0], 0, diagnosticInfos, responseHeader.StringTable);
                }
                return Task.CompletedTask;
            });
        }

        public Task<ReferenceDescriptionCollection> Browse(int serverIndex, BrowseDescription browseDescription, bool throwOnError)
        {
            if (browseDescription == null) throw new ArgumentNullException(nameof(browseDescription));
            logger.LogTrace("Browse {ServerName}, {NodeId}", serverIndex, browseDescription.NodeId);
            return WithSession(serverIndex, (session) =>
                Task.FromResult(ClientUtils.Browse(session, browseDescription, throwOnError)));
        }
        public Task<IList<BrowseReferenceDescription>> Browse(
            int serverIndex,
            ExpandedNodeId nodeId,
            BrowseDirection browseDirection,
            ExpandedNodeId referenceTypeId,
            bool includeSubtypes,
            uint nodeClassMask,
            uint resultMask,
            bool throwOnError)
        {
            logger.LogTrace("Browse {ServerName}, {NodeId}", serverIndex, nodeId);
            return WithSession(serverIndex, (session) =>
            {
                BrowseDescription browseDescription = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(nodeId, session.NamespaceUris),
                    BrowseDirection = browseDirection,
                    ReferenceTypeId = ExpandedNodeId.ToNodeId(referenceTypeId, session.NamespaceUris),
                    IncludeSubtypes = includeSubtypes,
                    NodeClassMask = nodeClassMask,
                    ResultMask = resultMask
                };
                var result = ClientUtils.Browse(session, browseDescription, throwOnError);
                IList<BrowseReferenceDescription> ret = new List<BrowseReferenceDescription>();
                foreach (var res in result)
                {
                    ret.Add(new BrowseReferenceDescription
                    {
                        ReferenceTypeId = NodeId.ToExpandedNodeId(res.ReferenceTypeId, session.NamespaceUris),
                        BrowseName = res.BrowseName,
                        DisplayName = res.DisplayName,
                        IsForward = res.IsForward,
                        NodeClass = res.NodeClass,
                        NodeId = new ExpandedNodeId(
                            identifier: res.NodeId.Identifier,
                            namespaceIndex: 0,
                            namespaceUri: session.NamespaceUris.GetString(res.NodeId.NamespaceIndex),
                            serverIndex: (uint)serverIndex),
                        TypeDefinition = res.TypeDefinition
                    });
                }
                return Task.FromResult(ret);
            });
        }

        public Task<Node> FetchNode(int serverIndex, ExpandedNodeId nodeId)
        {
            logger.LogTrace("FetchNode {ServerName}, {NodeId}", serverIndex, nodeId);
            return WithSession(serverIndex, (session) =>
                Task.FromResult(session.NodeCache.FetchNode(nodeId)));
        }

        public Task<IList<INode>> FindReferences(int serverIndex, ExpandedNodeId nodeId, NodeId referenceTypeId, bool isInverse, bool includeSubtypes) =>
            WithSession(serverIndex, (session) =>
               Task.FromResult(session.NodeCache.FindReferences(
                   nodeId,
                   referenceTypeId,
                   isInverse,
                   includeSubtypes)));

        public Task<List<NodeId>> TranslateBrowsePaths(int serverIndex, NodeId startNodeId, NamespaceTable namespacesUris, params string[] relativePaths)
        {
            logger.LogTrace("TranslateBrowsePaths {ServerName}, {NodeId}", serverIndex, startNodeId);
            return WithSession(serverIndex, (session) =>
                Task.FromResult(ClientUtils.TranslateBrowsePaths(session, startNodeId, namespacesUris, relativePaths)));
        }

        public Task<NamespaceTable> GetNamespaceUris(int serverIndex) =>
            WithSession(serverIndex, (session) =>
                Task.FromResult(session.NamespaceUris));

        public Task<IList<object>> Call(int serverIndex, NodeId objectId, NodeId methodId, params object[] args)
        {
            logger.LogTrace("Call {ServerName}, {ObjectId}, {MethodId}", serverIndex, objectId, methodId);
            return WithSession(serverIndex, (session) =>
                Task.FromResult(session.Call(objectId, methodId, args)));
        }
        public async Task<ServerDescriptor> TestConnection(int serverIndex)
        {
            try
            {
                var server = GetServer(serverIndex);
                var value = await ReadValue(server, new ExpandedNodeId(2259));
                server.ServerStatus = value is int ? (int)value : -1;
                return server;
            }
            catch (ServiceResultException) { return null; }
        }
    }
}
