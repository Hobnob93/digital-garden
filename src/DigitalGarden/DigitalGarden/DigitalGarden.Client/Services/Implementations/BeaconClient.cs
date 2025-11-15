using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;

namespace DigitalGarden.Client.Services.Implementations;

public class BeaconClient : BaseClient, IBeaconProvider
{
    public BeaconClient(IHttpClientFactory httpFactory)
        : base(httpFactory)
    {
        
    }

    public async Task<ICollection<BeaconCategoryItems>> GetAllItems()
    {
        var apiResult = await Get<ICollection<BeaconCategoryItems>>("Beacons/GetAllWithinCategories");
        return apiResult.AsExpectedType;
    }
}
