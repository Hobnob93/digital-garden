using DigitalGarden.Data.Dtos;
using DigitalGarden.Shared.Models.Data;

namespace DigitalGarden.Extensions;

public static class MappingExtensions
{
    public static BeaconCategory ToDomain(this BeaconCategoryDto dto)
    {
        return new BeaconCategory
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static BeaconCategoryItems ToDomainWithItems(this BeaconCategoryDto dto)
    {
        return new BeaconCategoryItems
        {
            Name = dto.Name,
            Description = dto.Description,
            Beacons = dto.Beacons
                .Select(b => b.ToDomain())
                .ToArray()
        };
    }

    public static Beacon ToDomain(this BeaconDto dto)
    {
        return new Beacon
        {
            Title = dto.Title,
            Description = dto.Description,
            Url = dto.Url
        };
    }

    public static FamousQuote ToDomain(this FamousQuoteDto dto)
    {
        return new FamousQuote
        {
            Text = dto.Text,
            Author = dto.Author ?? "Unknown",
            Source = dto.Source,
            Year = dto.Year,
            IsFavourite = dto.IsFavourite,
            IsAttribution = dto.IsAttribution
        };
    }
}
