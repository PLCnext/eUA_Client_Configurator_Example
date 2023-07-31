// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Arp.OpcUA.ClientConfiguration
{
    /// <summary>
    /// eUAClient configurator. (Implementation of <see cref="IEuaClientConfiguratorService" />)
    /// </summary>
    public class EuaClientConfiguratorService : IEuaClientConfiguratorService
    {
        private readonly IEuaClientConfigFileEditor euaClientConfigFileEditor;
        private readonly IUAClientService uaClientService;

        List<EuaClientConfigurator> configurators = new List<EuaClientConfigurator>();

        /// <summary>
        /// Constructor for DI Container.
        /// </summary>
        /// <param name="euaClientConfigFileEditor">An eUAClient configuration file editor service.</param>
        /// <param name="uAClientService">An eUAClient service.</param>
        public EuaClientConfiguratorService(IEuaClientConfigFileEditor euaClientConfigFileEditor, IUAClientService uAClientService)
        {
            this.euaClientConfigFileEditor = euaClientConfigFileEditor;
            uaClientService = uAClientService;
        }

        public void CloseConfiguration(IEuaClientConfigurator configurator)
        {
            configurators.Remove(configurator as EuaClientConfigurator);
        }

        /// <summary>
        /// Creates or overwrites an eUAClient configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEuaClientConfigurator.</returns>
        /// <exception cref="ArgumentException">'{nameof(name)}' cannot be null or whitespace. - name</exception>
        public IEuaClientConfigurator CreateConfiguration(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            var configurator = GetConfiguration(name);
            if (configurator != null)
                CloseConfiguration(configurator);

            configurator = new EuaClientConfigurator(uaClientService, euaClientConfigFileEditor, name);
            configurators.Add(configurator as EuaClientConfigurator);

            return configurator;
        }
        /// <summary>
        /// Gets an open eUAClient configuration.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The configurator.</returns>
        public IEuaClientConfigurator GetConfiguration(string name)
        {
            return configurators.FirstOrDefault(c => c.Configuration.Name == name);
        }

        /// <summary>
        /// Loads an eUAClient configuration.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>IEuaClientConfigurator.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEuaClientConfigurator LoadConfiguration(Stream stream)
        {
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new ExpandedNodeIdJsonConverter());
            var config = JsonSerializer.Deserialize<ClientConfigurationModel>(stream, serializeOptions);

            var configurator = GetConfiguration(config.Name);
            if (configurator != null)
                CloseConfiguration(configurator);

            return new EuaClientConfigurator(uaClientService, euaClientConfigFileEditor, config);
        }

    }
}