using DigitalGarden.Shared.Components.Models;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Client.Components.Pages;

public partial class LifeLog
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

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

    private SimpleCardData[] RightNowCards = [
        new SimpleCardData("fa-book", "Mistborn (Book 1)", "Brandon Sanderson"),
        new SimpleCardData("fa-code", "This Website", "Takes so long..."),
        new SimpleCardData("fa-unity", "Cavern Critters", "Games dev project", FontAwesomeType: "fa-brands"),
        new SimpleCardData("fa-pen-fancy", "Comissioner's Conscience", "Paxium Chronicles novella")
    ];

    private SimpleCardData[] EntertainmentCards = [
        new SimpleCardData("fa-film", "Captain Phillips"),
        new SimpleCardData("fa-book", "1984", "George Orwell"),
        new SimpleCardData("fa-tv", "The Traitors", "Season 4"),
        new SimpleCardData("fa-tv", "Stranger Things", "Season 5"),
        new SimpleCardData("fa-film", "The Hunger Games: Catching Fire")
    ];

    private SimpleCardData[] GameCards = [
        new SimpleCardData("fa-steam", "Baldur's Gate 3", FontAwesomeType: "fa-brands"),
        new SimpleCardData("fa-dice", "Hues & Cues"),
        new SimpleCardData("fa-dice", "Sequence"),
        new SimpleCardData("fa-xbox", "Halo 4", FontAwesomeType: "fa-brands"),
        new SimpleCardData("fa-steam", "Mass Effect: LE", "100% Achievements!", FontAwesomeType: "fa-brands"),
        new SimpleCardData("fa-steam", "Hades II", "100% Achievements!", FontAwesomeType: "fa-brands")
    ];

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
