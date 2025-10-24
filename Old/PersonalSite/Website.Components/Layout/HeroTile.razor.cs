using Microsoft.AspNetCore.Components;
using Website.Components.Builders;

namespace Website.Components.Layout;

public partial class HeroTile : BaseComponent
{
    [Parameter, EditorRequired]
    public string Char { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    [Parameter]
    public string ImageType { get; set; } = "png";

    private string ImagePath => $"/images/home/{Name}.{ImageType}";
    private string ImageAlt => $"{Name} hero image";

    public string Classes => new ClassBuilder()
        .Add("hero-tile")
        .Add(Class!, condition: Class is not null)
        .Build();
}
