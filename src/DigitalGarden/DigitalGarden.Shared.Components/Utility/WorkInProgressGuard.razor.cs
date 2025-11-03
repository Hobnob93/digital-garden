using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace DigitalGarden.Shared.Components.Utility;

public partial class WorkInProgressGuard
{
    [Inject]
    private ISiteConfigurationProvider ConfigurationProvider { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool _siteIsWip;

    protected override async Task OnInitializedAsync()
    {
        var siteFlags = await ConfigurationProvider.GetSiteFlagOptionsAsync();
        _siteIsWip = siteFlags.IsWip;
        
        if (_siteIsWip)
        {
            if (Navigation.Uri.EndsWith("/gift-list", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        
            if (!Navigation.Uri.EndsWith("/wip", StringComparison.OrdinalIgnoreCase))
            {
                Navigation.NavigateTo("/wip", replace: true, forceLoad: true);
            }
        }
    }

    private void OnNavigationChanging(LocationChangingContext context)
    {
        if (context.TargetLocation.EndsWith("/gift-list", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        if (_siteIsWip && !context.TargetLocation.EndsWith("/wip", StringComparison.OrdinalIgnoreCase))
        {
            context.PreventNavigation();
            Navigation.NavigateTo("/wip", replace: true, forceLoad: true);
        }
    }
}
