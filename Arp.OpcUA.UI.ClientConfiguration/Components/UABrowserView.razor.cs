// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.ClientConfiguration;
using Arp.OpcUA.Core.UAClient;
using Microsoft.AspNetCore.Components;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ObjectIds = Opc.Ua.ObjectIds;

namespace Arp.OpcUA.UI.ClientConfiguration.Components
{
    public partial class UABrowserView
    {
        [Inject]
        public IUAClientService UAClientService { get; set; }

        int server;
        [Parameter]
        public int Server
        {
            get => server;
            set
            {
                if (server == value)
                    return;
                server = value;
                Browse();
            }
        }

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public HashSet<BrowseNode> SelectedNodes { get; set; } = new HashSet<BrowseNode>();

        [Parameter]
        public BrowseFilter BrowseFilter { get; set; }

        [Parameter]
        public bool ShowServerSelector { get; set; }

        [Parameter]
        public Func<BrowseNode, BrowseFilter, Task<IList<BrowseNode>>> BrowseFunction { get; set; }

        public IList<ServerDescriptor> Servers { get; } = new List<ServerDescriptor>();

        public bool CheckboxClicked(BrowseNode node, bool value)
        {
            if (value)
                SelectedNodes.Add(node);
            else
                SelectedNodes.Remove(node);
            return !value;
        }
        public void Update()
        {
            InvokeAsync(() => StateHasChanged());
        }
        bool firstCall = true;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (firstCall)
            {
                firstCall = false;
                foreach (var server in UAClientService.Servers)
                {
                    var svrDescriptor = await UAClientService.TestConnection(server.ServerIndex);
                    if (svrDescriptor?.ServerStatus != 0)
                        continue;
                    Servers.Add(svrDescriptor);
                }

                Server = Servers.FirstOrDefault()?.ServerIndex ?? 0;
                SelectedNodes.Clear();
                BrowseNodes.Clear();
                await Browse(BrowseNodes, GetObjectsNode());
            }
        }
        private BrowseNode GetObjectsNode()
        {
            return new BrowseNode(Server, "Objects")
            {
                NodeId = ObjectIds.ObjectsFolder,
                NodeClass = NodeClass.Object,
                DisplayPath = "Objects"
            };
        }
        public Task Browse()
        {
            SelectedNodes.Clear();
            BrowseNodes.Clear();
            return Browse(BrowseNodes, GetObjectsNode());
        }
        private async Task Browse(IList<BrowseNode> parent, BrowseNode node)
        {
            if (BrowseFunction == null) throw new InvalidOperationException("no BrowseFunction specified");

            var nodes = await BrowseFunction(node, BrowseFilter);
            foreach (var nd in nodes)
                parent.Add(nd);

            Update();
        }

        public List<BrowseNode> BrowseNodes { get; set; } = new List<BrowseNode>();

        public List<string> Messages { get; } = new List<string>();

        public async Task OnNodeExpand(BrowseNode node)
        {
            if (node.Childs != null)
                return;
            node.Childs = new List<BrowseNode>();
            await Browse(node.Childs, node);
        }

        public bool HasChilds(BrowseNode node)
        {
            return node.Childs == null || node.Childs.FirstOrDefault() != null;
        }
    }
}
