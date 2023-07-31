// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreButton : ComponentBase
    {

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }

        [Parameter]
        public bool IsDefaultButton { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }
        public Task Disable()
        {
            IsDisabled = true;
            return InvokeAsync(StateHasChanged);
        }
        public Task Enable()
        {
            IsDisabled = false;
            return InvokeAsync(StateHasChanged);
        }
        public Task Click() => OnClick.InvokeAsync();
    }
}
