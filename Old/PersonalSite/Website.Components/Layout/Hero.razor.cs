using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Website.Components.Builders;

namespace Website.Components.Layout;

public partial class Hero : BaseComponent
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    public string Classes => new ClassBuilder()
        .Add("hero")
        .Add(Class!, condition: Class is not null)
        .Build();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("setupHeroTiles");
        }
    }
}
