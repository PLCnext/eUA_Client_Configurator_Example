// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;

namespace Arp.OpcUA.Core.UAClient
{
    /// <summary>
    /// Connection information for persistence.
    /// </summary>
    public class ServerConnectionModel
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public MessageSecurityMode SecurityMode { get; set; } = MessageSecurityMode.None;
        public string SecurityPolicyUri { get; set; }

        public ServerConnectionModel Clone()
        {
            return new ServerConnectionModel
            {
                Enabled = Enabled,
                Name = Name,
                Url = Url,
                UserName = UserName,
                Password = Password,
                SecurityMode = SecurityMode,
                SecurityPolicyUri = SecurityPolicyUri,
            };
        }
    }
}