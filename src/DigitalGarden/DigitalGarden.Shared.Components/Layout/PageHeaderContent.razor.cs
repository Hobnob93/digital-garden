using DigitalGarden.Shared.Components.Extensions;
using DigitalGarden.Shared.Components.Models.Enums;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Layout;

public partial class PageHeaderContent
{
    public string HeaderColorClass => HeaderColor.GetCssClassFromTextColor();

    [Parameter, EditorRequired]
    public string PageTitle { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string HeaderText { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public RenderFragment? BodyFragment { get; set; }

    [Parameter]
    public TextColor HeaderColor { get; set; }
}
