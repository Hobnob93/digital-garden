using DigitalGarden.Shared.Models.Enums;

namespace DigitalGarden.Shared.Models.Data;

public class RecentLifeLog
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public LifeLogItemType Type { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime AddedAtUtc { get; set; }

    public bool IsEntertainment => Type switch
    {
        LifeLogItemType.Reading => true,
        LifeLogItemType.WatchingFilm => true,
        LifeLogItemType.WatchingShow => true,
        _ => false
    };

    public bool IsGame => Type switch
    {
        LifeLogItemType.PlayingBoardGame => true,
        LifeLogItemType.PlayingXbox => true,
        LifeLogItemType.PlayingSteam => true,
        _ => false
    };
}
