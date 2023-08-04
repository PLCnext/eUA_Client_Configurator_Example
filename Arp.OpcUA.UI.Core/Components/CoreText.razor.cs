// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreText
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
