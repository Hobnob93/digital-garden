using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Client.Components.Pages;

public partial class Beacons
{
    [Inject]
    public IBeaconProvider BeaconProvider { get; set; } = default!;

    [PersistentState]
    public ICollection<BeaconCategoryItems>? CategoryItems { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (CategoryItems is null)
            {
                CategoryItems = await BeaconProvider.GetAllItems();
                await InvokeAsync(StateHasChanged);
            }            
        }
    }
}