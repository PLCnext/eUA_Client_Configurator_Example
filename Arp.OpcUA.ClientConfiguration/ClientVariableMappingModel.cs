// Copyright PHOENIX CONTACT Electronics GmbH

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Model of the ClientVariableMapping.
    /// </summary>
    public class ClientVariableMappingModel
    {
        /// <summary>
        /// Gets or sets the local variable.
        /// </summary>
        /// <value>The local variable.</value>
        public VariableModel LocalVariable { get; set; }
        /// <summary>
        /// Gets or sets the remote variable.
        /// </summary>
        /// <value>The remote variable.</value>
        public VariableModel RemoteVariable { get; set; }
        /// <summary>
        /// Gets the datatype.
        /// </summary>
        /// <value>The datatype.</value>
        public DataTypeModel DataType => LocalVariable?.DataType ?? RemoteVariable?.DataType;
    }
}