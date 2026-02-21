using DigitalGarden.Shared.Components.Extensions;
using DigitalGarden.Shared.Components.Models;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Client.Components.Pages;

public partial class LifeLog
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ILifeDataProvider LifeDataProvider { get; set; } = default!;

    [PersistentState]
    public RecentLifeLog[]? LifeLogItems { get; set; }

    [PersistentState]
    public SimpleCardData[]? RightNowCards { get; set; }

    [PersistentState]
    public SimpleCardData[]? EntertainmentCards { get; set; }

    [PersistentState]
    public SimpleCardData[]? GameCards { get; set; }

    [PersistentState]
    public SimpleCardData[]? MusicCards { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (LifeLogItems is null)
            {
                await SetLogCollectionsAsync();
                await SetTopArtistsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task SetLogCollectionsAsync()
    {
        var items = await LifeDataProvider.GetRecentLifeLogsAsync();
        LifeLogItems = items.OrderByDescending(i => i.AddedAtUtc).ToArray();

        RightNowCards = LifeLogItems
            ?.Where(i => i.IsCurrent)
            ?.Select(i => i.ToSimpleCardData())
            ?.ToArray();

        EntertainmentCards = LifeLogItems
            ?.Where(i => !i.IsCurrent && i.IsEntertainment)
            ?.Select(i => i.ToSimpleCardData())
            ?.ToArray();

        GameCards = LifeLogItems
            ?.Where(i => !i.IsCurrent && i.IsGame)
            ?.Select(i => i.ToSimpleCardData())
            ?.ToArray();
    }

    private async Task SetTopArtistsAsync()
    {
        var topArtistsResponse = await LifeDataProvider.GetLastFmTopArtists();

        MusicCards = topArtistsResponse.TopArtists.Artists
            .OrderByDescending(a => a.PlayCount)
            .Select((a, i) => a.ToSimpleCardData(i + 1))
            .ToArray();
    }

    private void GoToProfessionalProfile()
    {
        Navigation.NavigateTo("/life-log/mission");
    }

    private void GoToWedding()
    {
        Navigation.NavigateTo("/life-log/09-09-2025");
    }

    private void GoToHolidays()
    {
        Navigation.NavigateTo("/life-log/journeys");
    }
}
