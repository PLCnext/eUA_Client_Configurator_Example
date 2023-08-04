// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core.UAClient;
using Arp.OpcUA.ServerCatalog;
using Arp.OpcUA.UI.Core.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arp.OpcUA.UI.ServerCatalog.Components
{
    public partial class ApplicationsView
    {
        bool isNewItem;
        private ServerConnectionModel selectedItem;
        CoreDialog statusDialog;
        string statusText;

        public ServerConnectionModel SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; EditItem = selectedItem?.Clone(); }
        }
        public ServerConnectionModel EditItem { get; set; }
        public string SecurityPolicyUriWithBestAvailable
        {
            get { return EditItem.SecurityPolicyUri == null ? "best" : EditItem.SecurityPolicyUri; }
            set { EditItem.SecurityPolicyUri = (value == "best") ? null : value; }
        }
        [Inject]
        public IServerCatalogService ServerCatalog { get; set; }
        public IList<ServerConnectionModel> Servers { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                Servers = await ServerCatalog.GetServersAsync();
                await InvokeAsync(StateHasChanged);
            }
        }
        public Task Apply()
        {
            Task task;
            if (isNewItem)
            {
                isNewItem = false;
                task = ServerCatalog.AddServerAsync(EditItem);
                SelectedItem = null;
                return task;
            }
            if (EditItem != null && SelectedItem != null)
            {
                SelectedItem.Url = EditItem.Url;
                SelectedItem.Name = EditItem.Name;
                SelectedItem.UserName = EditItem.UserName;
                SelectedItem.Password = EditItem.Password;
                SelectedItem.Enabled = EditItem.Enabled;
                SelectedItem.SecurityMode = EditItem.SecurityMode;
                SelectedItem.SecurityPolicyUri = EditItem.SecurityPolicyUri;
            }
            task = ServerCatalog.UpdateServerAsync(SelectedItem);
            SelectedItem = null;
            return task;
        }
        public Task CheckedChanged(ServerConnectionModel model, bool isChecked)
        {
            model.Enabled = isChecked;
            return ServerCatalog.UpdateServerAsync(model);
        }
        public Task Cancel()
        {
            SelectedItem = null;
            isNewItem = false;
            return Task.CompletedTask;
        }
        public Task AddNew()
        {
            SelectedItem = new ServerConnectionModel();
            isNewItem = true;
            return Task.CompletedTask;
        }
        public async Task Remove(ServerConnectionModel item)
        {
            if (SelectedItem == item)
            {
                SelectedItem = null;
            }

            await ServerCatalog.RemoveServerAsync(item);
        }

        public async Task TestConnection(ServerConnectionModel item)
        {
            statusText = $"Connecting...";
            statusDialog.Show();
            await Task.Yield();
            if (await ServerCatalog.TestConnectionAsync(item))
                statusText = $"Successfully connected to {item.Url}.";
            else
                statusText = $"Cannot connect to {item.Url}.";
        }
    }
}
