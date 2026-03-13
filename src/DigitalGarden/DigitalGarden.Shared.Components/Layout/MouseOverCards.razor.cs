using DigitalGarden.Shared.Components.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DigitalGarden.Shared.Components.Layout;

public partial class MouseOverCards
{
    [Inject]
    private IJSRuntime JsInterop { get; set; } = default!;

    [Parameter, EditorRequired]
    public SimpleCardData[]? Cards { get; set; } = [];

    [Parameter]
    public bool DisableSpinner { get; set; }

    private ElementReference _mouseOverCardsRoot;
    private bool _initializedMouseOver;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!string.IsNullOrWhiteSpace(_mouseOverCardsRoot.Id) && !_initializedMouseOver)
        {
            await JsInterop.InvokeVoidAsync("initializeMouseOverCards", _mouseOverCardsRoot);
            _initializedMouseOver = true;
        }
    }
}
