// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreTable<T>
    {
        [Parameter]
        public IEnumerable<T> Items { get; set; } = new List<T>();
        [Parameter]
        public RenderFragment Columns { get; set; }
        [Parameter]
        public T SelectedItem { get; set; }
        [Parameter]
        public EventCallback<T> SelectedItemChanged { get; set; }

        private List<CoreTableColumn<T>> columns = new();
        private List<CoreEditableTableColumn<T>> editableColumns = new();

        /// Add a child component (will be done by the child itself)
        public void AddColumn(CoreTableColumn<T> column)
        {
            columns.Add(column);
            StateHasChanged();
        }
        public void AddEditableColumn(CoreEditableTableColumn<T> column)
        {
            editableColumns.Add(column);
            StateHasChanged();
        }

        MudTable<T> mudTable;

        private async void RowClickEvent(TableRowClickEventArgs<T> tableRowClickEventArgs)
        {
            if (tableRowClickEventArgs.Item?.Equals(SelectedItem) == true)
                SelectedItem = default(T);
            else
                SelectedItem = tableRowClickEventArgs.Item;
            await SelectedItemChanged.InvokeAsync(this.SelectedItem);
        }

        private string SelectedRowClassFunc(T element, int rowNumber)
        {
            if (SelectedItem?.Equals(element) == true)
                return "selected";
            else
                return string.Empty;
        }

    }
}
