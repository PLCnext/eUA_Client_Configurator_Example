// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.ClientConfiguration;
using Arp.OpcUA.Core;
using Arp.OpcUA.Core.UAClient;
using Arp.OpcUA.UI.Core.Components;
using Microsoft.AspNetCore.Components;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Arp.OpcUA.UI.ClientConfiguration.Components
{
    public partial class EuaClientConfigurationView : ComponentBase
    {

        [Inject]
        public IUAClientService uaClientService { get; set; }
        [Inject]
        public IEuaClientConfiguratorService configuratorService { get; set; }
        [Inject]
        public IBrowseForFileService browseForFileService { get; set; }

        UABrowserView localBrowser;
        UABrowserView remoteBrowser;

        CoreDialog localBrowserDlg;
        CoreDialog remoteBrowserDlg;
        CoreDialog remoteEditDlg;

        BrowseFilter localBrowseFilter = new BrowseFilter();
        BrowseFilter remoteBrowseFilter = new BrowseFilter();

        IEuaClientConfigurator configurator;
        ClientConfigurationModel configuration;

        ClientVariableGroupModel currentLocalGroup;
        ClientVariableMappingModel currentLocalMapping;

        ClientVariableGroupModel currentRemoteGroup;
        ClientVariableMappingModel currentRemoteMapping;

        public string editRemoteServer;
        public string editRemoteNodeIdNamespace;
        public string editRemoteNodeIdentifier;
        public string editRemoteName;

        int localServerIndex;

        public EuaClientConfigurationView() { }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (configuration == null)
            {
                configurator = configuratorService.GetConfiguration("default");
                if (configurator == null)
                    configurator = configuratorService.CreateConfiguration("default");
                configuration = configurator.Configuration;
            }
        }
        public void AddMappings(ClientVariableGroupModel group)
        {
            currentLocalGroup = group;
            currentLocalMapping = null;
            localBrowseFilter.DataType = null;
            localBrowserDlg.Show();
            Update();
        }
        public void SelectLocalVariable(ClientVariableGroupModel group, ClientVariableMappingModel mapping)
        {
            currentLocalGroup = group;
            currentLocalMapping = mapping;
            localBrowseFilter.DataType = mapping.RemoteVariable?.DataType;

            localBrowserDlg.Show();

            Update();
        }
        public void SelectRemoteVariable(ClientVariableGroupModel group, ClientVariableMappingModel mapping)
        {
            currentRemoteGroup = group;
            currentRemoteMapping = mapping;
            remoteBrowseFilter.DataType = mapping.LocalVariable.DataType;

            remoteBrowserDlg.Show();

            Update();
        }
        public void EditRemoteVariable(ClientVariableGroupModel group, ClientVariableMappingModel mapping)
        {
            string identifier = "";
            var id = mapping.RemoteVariable?.NodeId;
            if (id != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                NodeId.Format(stringBuilder, id.Identifier, id.IdType, 0);
                identifier = stringBuilder.ToString();
            }
            currentRemoteGroup = group;
            currentRemoteMapping = mapping;
            editRemoteName = mapping.RemoteVariable?.DisplayName;
            editRemoteNodeIdentifier = identifier;
            editRemoteNodeIdNamespace = mapping.RemoteVariable?.NodeId?.NamespaceUri;
            editRemoteServer = mapping.RemoteVariable?.Server?.Name;

            remoteEditDlg.Show();

            Update();
        }
        public void ApplyRemoteEditor(bool close)
        {
            if (!close)
                return;
            var server = uaClientService.Servers.Where(svr => svr.Name == editRemoteServer).FirstOrDefault();
            try
            {
                configurator.SetRemoteVariable(currentRemoteMapping, editRemoteName, server, editRemoteNodeIdNamespace, editRemoteNodeIdentifier);
            }
            catch (ArgumentException) { }
            Update();
        }
        public void ApplyLocalBrowser(bool close)
        {
            if (!close)
                return;
            if (currentLocalGroup != null && currentLocalMapping != null)
            {
                SetLocalNodeId();
            }
            else if (currentLocalGroup != null)
            {
                AddMappings();
            }
            Update();
        }

        private void AddMappings()
        {
            foreach (var node in localBrowser.SelectedNodes)
            {
                if (node.NodeClass == NodeClass.Variable)
                {
                    var mapping = configurator.AddMapping(currentLocalGroup);
                    configurator.SetLocalVariable(
                        mapping,
                        node.DisplayName,
                        uaClientService.Servers.Where(svr => svr.ServerIndex == node.ServerIndex).FirstOrDefault(),
                        node.NodeId,
                        node.DisplayPath,
                        node.DataType);

                }
            }
            Update();
        }

        private void SetLocalNodeId()
        {
            var node = localBrowser.SelectedNodes.FirstOrDefault();
            if (node == null) // nothing selected
                return;

            configurator.SetLocalVariable(
                currentLocalMapping,
                node.DisplayName,
                uaClientService.Servers.Where(svr => svr.ServerIndex == node.ServerIndex).FirstOrDefault(),
                node.NodeId,
                node.DisplayPath,
                node.DataType);

            Update();
        }

        public void ApplyRemoteBrowser(bool close)
        {
            if (!close)
                return;

            if (currentRemoteGroup != null && currentRemoteMapping != null)
            {
                var node = remoteBrowser.SelectedNodes.FirstOrDefault();
                if (node == null) // nothing selected
                    return;

                configurator.SetRemoteVariable(
                    currentRemoteMapping,
                    node.DisplayName,
                    uaClientService.Servers.Where(svr => svr.ServerIndex == node.ServerIndex).FirstOrDefault(),
                    node.NodeId,
                    node.DisplayPath,
                    node.DataType);

            }
            Update();
        }
        public void Update()
        {
            InvokeAsync(() => StateHasChanged());
        }

        public void AddGroup()
        {
            configurator.AddGroup();
            Update();
        }
        public void RemoveGroup(ClientVariableGroupModel group)
        {
            configurator.RemoveGroup(group);
            Update();
        }
        public void RemoveMapping(ClientVariableGroupModel group, ClientVariableMappingModel mapping)
        {
            configurator.RemoveMapping(group, mapping);
            Update();
        }
        public async void Export()
        {
            try
            {
                using var stream = await browseForFileService.OpenFileForWriteAsync("Select a configuration project", "eUAClient connection configuraiton|*.xml", FileMode.Create);
                if (stream == null)
                    return;

                configurator.ExportXmlConfiguration(stream);

                Update();
            }
            catch (IOException) { }

        }
        public async void Load()
        {
            try
            {
                using var stream = await browseForFileService.OpenFileForReadAsync("Select a configuration project", "eUAClient configuration|*.json", FileMode.Open);
                if (stream == null)
                    return;
                configurator = configuratorService.LoadConfiguration(stream);
                configuration = configurator.Configuration;

                Update();
            }
            catch (IOException) { }
        }
        public void Create()
        {
            try
            {
                configurator = configuratorService.CreateConfiguration("default");
                configuration = configurator.Configuration;
                Update();
            }
            catch (IOException) { }
        }
        public async void Save()
        {
            try
            {
                using var stream = await browseForFileService.OpenFileForWriteAsync("Select a configuration project", "eUAClient configuration|*.json", FileMode.Create);
                if (stream == null)
                    return;

                configurator.SaveConfiguration(stream);

                Update();
            }
            catch (IOException) { }
        }

        public List<string> Messages { get; } = new List<string>();
    }
}
