// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;

namespace Arp.OpcUA.Core.UAClient
{
    /// <summary>
    /// Information about a running server.
    /// </summary>
    public class ServerDescriptor
    {
        public ServerDescriptor(string name, string serverUrl)
        {
            Name = name;
            ServerUrl = serverUrl;
        }
        /// <summary>
        /// Given name of the user (no semantic meaning).
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Application URL to access the server.
        /// </summary>
        public string ServerUrl { get; }
        /// <summary>
        /// Empty string or user name if UserName/Password authentication is used.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password of the user (only if user is specified)
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Mode how to protect the communication.
        /// </summary>
        public MessageSecurityMode SecurityMode { get; set; }
        /// <summary>
        /// Cypher suite to use for the connection.
        /// </summary>
        public string SecurityPolicyUri { get; set; }
        /// <summary>
        /// Unique index that does not change during runtime of the Software.
        /// </summary>
        /// <remarks>
        /// 0 is used for ExpandedNodeIds that are not server specific (e.g. UA Information Model of Namespace 0).
        /// Could be replaced by ServerUri for persistence.
        /// </remarks>
        public int ServerIndex { get; set; }
        /// <summary>
        /// Status as exposed at the Server Object.
        /// </summary>
        public int ServerStatus { get; set; } = -1;
    }
}