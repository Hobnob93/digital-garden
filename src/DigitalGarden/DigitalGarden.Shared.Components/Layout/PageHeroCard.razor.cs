using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Layout;

public partial class PageHeroCard
{
    [Parameter, EditorRequired]
    public RenderFragment? HeaderFragment { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment? BodyFragment { get; set; }
}
