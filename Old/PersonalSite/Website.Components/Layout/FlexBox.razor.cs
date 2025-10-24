using Microsoft.AspNetCore.Components;
using Website.Components.Builders;

namespace Website.Components.Layout;

public partial class FlexBox : BaseComponent
{
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public bool IsColumn { get; set; }

    [Parameter]
    public bool FullHeight { get; set; }

    public string Classes => new ClassBuilder()
        .Add("d-flex")
        .Add("flex-column", condition: IsColumn)
        .Add("h-100pc", condition: FullHeight)
        .Add(Class!, condition: Class is not null)
        .Build();
}
