namespace DigitalGarden.Data.Sync.Models;

public class SyncFamousQuoteData
{
    public string Text { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Source { get; set; }
    public int? Year { get; set; }
    public bool IsFavourite { get; set; }
    public bool IsAttribution { get; set; }
    public string Slug { get; set; } = string.Empty;
}
