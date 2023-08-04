// Copyright PHOENIX CONTACT Electronics GmbH

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// BrowseFilter for OPC Variables.
    /// </summary>
    public class BrowseFilter
    {
        /// <summary>
        /// Gets or sets the data type of the variable.
        /// </summary>
        /// <value>The type of the data.</value>
        public DataTypeModel DataType { get; set; }
    }
}