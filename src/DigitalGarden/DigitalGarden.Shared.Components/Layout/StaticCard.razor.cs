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

    [Parameter]
    public bool CenterBodyText { get; set; }

    [Parameter]
    public string? AppendClasses { get; set; }

    private string CardClasses => new ClassBuilder()
        .Add("card")
        .Add("sheen", condition: ShowSheen)
        .Add(AppendClasses!, condition: !string.IsNullOrWhiteSpace(AppendClasses))
        .Build();

    private string HeaderClasses => new ClassBuilder()
        .Add("header")
        .Add("pl-5")
        .Add("pr-5")
        .Add("pt-4")
        .Build();

    private string BodyClasses => new ClassBuilder()
        .Add("body")
        .Add("p-5")
        .Add("pt-4")
        .Add("center-text", condition: CenterBodyText)
        .Build();
}
