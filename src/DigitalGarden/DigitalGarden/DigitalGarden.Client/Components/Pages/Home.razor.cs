using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Client.Components.Pages;

public partial class Home
{
    [Inject]
    private ILifeDataProvider LifeDataProvider { get; set; } = default!;

    [PersistentState]
    public FamousQuote? QuoteOfTheDay { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (QuoteOfTheDay is not null)
            return;

        QuoteOfTheDay = await LifeDataProvider.GetQuoteOfTheDay();

        await InvokeAsync(StateHasChanged);
    }
}
