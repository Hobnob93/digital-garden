using DigitalGarden.Shared.Components.Models;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components.Layout;

public partial class MouseOverCards
{
    [Parameter, EditorRequired]
    public SimpleCardData[] Cards { get; set; } = [];
}
