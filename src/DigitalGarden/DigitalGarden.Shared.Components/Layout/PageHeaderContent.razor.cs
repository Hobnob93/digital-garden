using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Layout;

public partial class PageHeaderContent
{
    [Parameter, EditorRequired]
    public string PageTitle { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string HeaderText { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public RenderFragment? BodyFragment { get; set; }
}
