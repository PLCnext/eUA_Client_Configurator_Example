// Copyright PHOENIX CONTACT Electronics GmbH

using System.IO;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// Service to configure an eUAClient.
    /// </summary>
    public interface IEuaClientConfiguratorService
    {
        /// <summary>
        /// Creates an eUAClient configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The configurator.</returns>
        IEuaClientConfigurator CreateConfiguration(string name);
        /// <summary>
        /// Gets an open eUAClient configuration.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The configurator.</returns>
        IEuaClientConfigurator GetConfiguration(string name);
        /// <summary>
        /// Closes the specified configurator.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        void CloseConfiguration(IEuaClientConfigurator configurator);
        /// <summary>
        /// Loads an eUAClient configuration.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The configurator.</returns>
        IEuaClientConfigurator LoadConfiguration(Stream stream);
    }
}