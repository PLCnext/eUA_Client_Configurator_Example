// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreDialog
    {
        private bool show;
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<bool> OnClose { get; set; }

        public void Show()
        {
            show = true;
        }
        public async Task Close()
        {
            show = false;
            await OnClose.InvokeAsync(true);
        }
        public async Task Cancel()
        {
            show = false;
            await OnClose.InvokeAsync(false);
        }
    }
}
