// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreSelectItem<T> : ComponentBase
    {
        [Parameter]
        public T Value { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

    }
}
