using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DigitalGarden.Shared.Components.Controls;

public partial class ContentCardLink
{
    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string Description { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string FontAwesomeIcon { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public EventCallback ClickedCallback { get; set; }

    private bool IsPulsing { get; set; }
    private bool IsFlipped { get; set; }

    private string PulseClass => IsPulsing && !IsFlipped ? "pulse-A" : IsPulsing && IsFlipped ? "pulse-B" : string.Empty;

    private async Task OnFocus(FocusEventArgs e)
    {
        await SetPulsingAsync();
    }

    private async Task OnMouseEnter(MouseEventArgs e)
    {
        await SetPulsingAsync();
    }

    private async Task SetPulsingAsync()
    {
        if (IsPulsing)
        {
            IsFlipped = !IsFlipped;
            await InvokeAsync(StateHasChanged);
        }

        IsPulsing = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnAnimationEnd(EventArgs e)
    {
        IsPulsing = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnClicked(MouseEventArgs e)
    {
        await ClickedCallback.InvokeAsync();
    }
}
