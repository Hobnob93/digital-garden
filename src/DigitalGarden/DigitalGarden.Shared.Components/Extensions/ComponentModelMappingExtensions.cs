using DigitalGarden.Shared.Components.Models;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Models.Enums;

namespace DigitalGarden.Shared.Components.Extensions;

public static class ComponentModelMappingExtensions
{
    private const string DefaultFontAwesomeType = "fa-solid";
    private const string BrandsFontAwesomeType = "fa-brands";

    public static SimpleCardData ToSimpleCardData(this RecentLifeLog fileLog)
    {
        return new SimpleCardData
        (
            FontAwesomeIcon: FontAwesomeIconFromLifeLogItemType(fileLog.Type),
            fileLog.Title,
            fileLog.Description,
            FontAwesomeType: FontAwesomeTypeFromLifeLogItemType(fileLog.Type)
        );
    }

    private static string FontAwesomeIconFromLifeLogItemType(LifeLogItemType type)
    {
        return type switch
        {
            LifeLogItemType.Reading => "fa-book",
            LifeLogItemType.Developing => "fa-code",
            LifeLogItemType.UnityDeveloping => "fa-unity",
            LifeLogItemType.Writing => "fa-pen-fancy",
            LifeLogItemType.PlayingBoardGame => "fa-dice",
            LifeLogItemType.PlayingSteam => "fa-steam",
            LifeLogItemType.PlayingXbox => "fa-xbox",
            LifeLogItemType.WatchingFilm => "fa-film",
            LifeLogItemType.WatchingShow => "fa-tv",
            _ => "fa-circle-question"
        };
    }

    private static string FontAwesomeTypeFromLifeLogItemType(LifeLogItemType type)
    {
        return type switch
        {
            LifeLogItemType.UnityDeveloping => BrandsFontAwesomeType,
            LifeLogItemType.PlayingSteam => BrandsFontAwesomeType,
            LifeLogItemType.PlayingXbox => BrandsFontAwesomeType,
            _ => DefaultFontAwesomeType
        };
    }
}
