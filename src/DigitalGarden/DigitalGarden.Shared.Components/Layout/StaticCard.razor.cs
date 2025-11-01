using DigitalGarden.Shared.Components.Builders;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Layout;

public partial class StaticCard
{
    [Parameter, EditorRequired]
    public RenderFragment? HeaderFragment { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment? BodyFragment { get; set; }

    [Parameter]
    public bool HideHorizontalRules { get; set; }

    [Parameter]
    public bool ShowSheen { get; set; }

    private string CardClasses => new ClassBuilder()
        .Add("card")
        .Add("sheen", condition: ShowSheen)
        .Add("m-4")
        .Build();
}
