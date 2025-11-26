namespace DigitalGarden.Shared.Models.Data;

public class FamousQuote
{
    public required string Text { get; set; }
    public required string Author { get; set; }
    public string? Source { get; set; }
    public int? Year { get; set; }
    public bool IsFavourite { get; set; }
    public bool IsAttribution { get; set; }
}
