using DigitalGarden.Shared.Models.Data;

namespace DigitalGarden.Shared.Services.Interfaces;

public interface IBeaconProvider
{
    Task<ICollection<BeaconCategoryItems>> GetAllItems();
}
