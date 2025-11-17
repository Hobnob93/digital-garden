using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace DigitalGarden.Shared.Components.Layout;

public partial class TitleWithLink
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter, EditorRequired]
    public string Title { get; set; }

    [Parameter, EditorRequired]
    public string Url { get; set; }

    private string Host => new Uri(Url).Host;
    private string FaviconAddress => $"https://www.google.com/s2/favicons?domain={Host}";

    private async Task OnTitleClicked(MouseEventArgs args)
    {
        await JsRuntime.InvokeVoidAsync("open", Url, "_blank");
    }
}
