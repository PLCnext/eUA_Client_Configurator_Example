﻿// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreTextBox<T> : ComponentBase
    {
        [Parameter]
        public T Value { get; set; }
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }
        [Parameter]
        public string Label { get; set; }
        [Parameter]
        public string Placeholder { get; set; }
        [Parameter]
        public bool IsDisabled { get; set; }
        [Parameter]
        public bool IsPassword { get; set; }
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
        private Task OnValueChanged(T value)
        {
            this.Value = value;

            return ValueChanged.InvokeAsync(this.Value);
        }

    }
}
