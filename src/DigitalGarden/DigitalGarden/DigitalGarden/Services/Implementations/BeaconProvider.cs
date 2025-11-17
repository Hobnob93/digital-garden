using DigitalGarden.Data;
using DigitalGarden.Extensions;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGarden.Services.Implementations;

public class BeaconProvider : IBeaconProvider
{
    private readonly ApplicationDbContext _dbContext;

    public BeaconProvider(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<BeaconCategoryItems>> GetAllItems()
    {
        var data = await _dbContext.BeaconCategories
            .Include(bc => bc.Beacons)
            .ToListAsync();

        return data
            .Select(d => d.FromDtoToItems())
            .ToArray();
    }
}
