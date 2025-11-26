using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Layout;

public partial class WaitForInteractivity
{
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }
}