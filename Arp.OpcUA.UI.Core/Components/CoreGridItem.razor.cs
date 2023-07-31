// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreGridItem
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int xs { get; set; }

        [Parameter]
        public int sm { get; set; }
    }
}
