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

    private SimpleCardData[] EntertainmentCards = [
        new SimpleCardData("fa-suitcase", "Test 1", "Test desc")
    ];
}
