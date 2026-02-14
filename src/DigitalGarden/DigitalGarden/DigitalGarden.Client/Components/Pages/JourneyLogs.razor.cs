using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Client.Components.Pages;

public partial class JourneyLogs
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private void GoToNow()
    {
        Navigation.NavigateTo("/journey-logs/now");
    }

    private void GoToProfessionalProfile()
    {
        Navigation.NavigateTo("/journey-logs/mission-log");
    }

    private void GoToWedding()
    {
        Navigation.NavigateTo("/journey-logs/09-09-2025");
    }
}
