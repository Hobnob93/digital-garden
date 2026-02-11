using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Layout;

public partial class HeroCard
{
    [Parameter, EditorRequired]
    public RenderFragment? HeaderFragment { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment? BodyFragment { get; set; }
}
