using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Client.Components.Pages;

public partial class Beacons
{
    [Inject]
    public IBeaconProvider BeaconProvider { get; set; } = default!;

    private ICollection<BeaconCategoryItems>? CategoryItems { get; set; }

    protected override async Task OnInitializedAsync()
    {
        CategoryItems = await BeaconProvider.GetAllItems();
    }
}