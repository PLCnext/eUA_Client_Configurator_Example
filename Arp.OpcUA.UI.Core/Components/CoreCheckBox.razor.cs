// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreCheckBox : ComponentBase
    {
        [Parameter]
        public bool Checked { get; set; }
        [Parameter]
        public EventCallback<bool> CheckedChanged { get; set; }
        [Parameter]
        public string Label { get; set; }
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
        private Task OnCheckedChanged(bool value)
        {
            this.Checked = value;

            return CheckedChanged.InvokeAsync(this.Checked);
        }

    }
}
