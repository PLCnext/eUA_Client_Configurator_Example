// Copyright PHOENIX CONTACT Electronics GmbH

namespace Arp.OpcUA.UI.Core.Components
{
    public class CoreTreeStyle
    {
        public static readonly CoreTreeStyle Bootstrap = new CoreTreeStyle
        {
            //ExpandNodeIconClass = "far fa-plus-square uic-tree--cursor-pointer",
            ExpandNodeIconClass = "oi oi-chevron-right uic-tree--cursor-pointer",
            //CollapseNodeIconClass = "far fa-minus-square uic-tree--cursor-pointer",
            CollapseNodeIconClass = "oi oi-chevron-bottom uic-tree--cursor-pointer",
            NodeTitleClass = "p-1",
            NodeTitleSelectableClass = "text-body uic-tree--cursor-pointer",
            NodeTitleSelectedClass = "bg-primary text-white",
            NodeTitleDisabledClass = "text-black-50",
            NodeLoadingClass = "fas fa-spinner"
        };

        public string ExpandNodeIconClass { get; set; }
        public string CollapseNodeIconClass { get; set; }
        public string NodeTitleClass { get; set; }
        public string NodeTitleSelectableClass { get; set; }
        public string NodeTitleSelectedClass { get; set; }
        public string NodeTitleDisabledClass { get; set; }
        public string NodeLoadingClass { get; set; }
    }
}