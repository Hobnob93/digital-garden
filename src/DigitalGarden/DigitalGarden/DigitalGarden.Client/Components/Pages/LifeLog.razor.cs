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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (LifeLogItems is null)
            {
                var items = await LifeDataProvider.GetRecentLifeLogsAsync();
                LifeLogItems = items.OrderByDescending(i => i.AddedAtUtc).ToArray();

                SetLogCollections();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private void SetLogCollections()
    {
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

    private SimpleCardData[] MusicCards = [
        new SimpleCardData("fa-8", "Jay Smith", "209 plays"),
        new SimpleCardData("fa-7", "Hinder", "220 plays"),
        new SimpleCardData("fa-6", "Fall Out Boy", "263 plays"),
        new SimpleCardData("fa-5", "Citizen Soldier", "302 plays"),
        new SimpleCardData("fa-4", "Five Finger Death Punch", "381 plays"),
        new SimpleCardData("fa-3", "Blue October", "469 plays"),
        new SimpleCardData("fa-2", "Shinedown", "488 plays"),
        new SimpleCardData("fa-1", "Nickelback", "1,186 plays"),
    ];
}
