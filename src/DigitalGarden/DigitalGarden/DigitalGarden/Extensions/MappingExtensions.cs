using DigitalGarden.Data.Dtos;
using DigitalGarden.Shared.Models.Data;

namespace DigitalGarden.Extensions;

public static class MappingExtensions
{
    public static BeaconCategory FromDtoToCategory(this BeaconCategoryDto dto)
    {
        return new BeaconCategory
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static BeaconCategoryItems FromDtoToItems(this BeaconCategoryDto dto)
    {
        return new BeaconCategoryItems
        {
            Name = dto.Name,
            Description = dto.Description,
            Beacons = dto.Beacons.Select(b => b.FromDto()).ToArray()
        };
    }

    public static Beacon FromDto(this BeaconDto dto)
    {
        return new Beacon
        {
            Title = dto.Title,
            Description = dto.Description,
            Url = dto.Url
        };
    }
}
