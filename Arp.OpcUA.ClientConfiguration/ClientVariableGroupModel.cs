// Copyright PHOENIX CONTACT Electronics GmbH


using System.Collections.Generic;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Model of the ClientVariableGroup.
    /// </summary>
    public class ClientVariableGroupModel
    {
        /// <summary>
        /// Gets or sets the type of the group.
        /// </summary>
        /// <value>The type of the group.</value>
        public eUAClientGroupType GroupType { get; set; }
        /// <summary>
        /// CycleTime in ms.
        /// </summary>
        /// <value>The cycle time.</value>
        public int CycleTime { get; set; } = 500;
        /// <summary>
        /// Gets the variable mappings.
        /// </summary>
        /// <value>The variable mappings.</value>
        public IList<ClientVariableMappingModel> VariableMappings { get; set; } = new List<ClientVariableMappingModel>();
    }
}