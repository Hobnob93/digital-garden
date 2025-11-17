using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Controls;

public partial class AccordionOfItems<TItem>
{
    [Parameter, EditorRequired]
    public RenderFragment<TItem>? TabHeaderFragment { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment<TItem>? TabContentFragment { get; set; }

    [Parameter, EditorRequired]
    public ICollection<TItem> Items { get; set; } = [];

    [Parameter]
    public bool AllowMultipleActive { get; set; }

    private List<TItem> _selectedItems = [];

    private string GetItemActiveClass(TItem item)
    {
        if (_selectedItems.Contains(item))
            return "active";

        return string.Empty;
    }

    private void OnItemClicked(TItem item)
    {
        if (!AllowMultipleActive)
        {
            if (_selectedItems.Contains(item))
            {
                _selectedItems.Clear();
                return;
            }

            _selectedItems.Clear();
        }

        if (_selectedItems.Contains(item))
            _selectedItems.Remove(item);
        else
            _selectedItems.Add(item);

        StateHasChanged();
    }
}
