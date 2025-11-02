using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Controls;

public partial class Anchor
{
    [Parameter, EditorRequired]
    public string Url { get; set; }

    [Parameter, EditorRequired]
    public string Text { get; set; }
}
