namespace DigitalGarden.Shared.Models.Options;

public class DailyIngestOptions
{
    public const string SectionName = "DailyIngest";

    public double MinimumDelayMinutes { get; set; } = 1d;
    public double DelayInDays { get; set; } = 1d;
}
