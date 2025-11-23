using DigitalGarden.Data.Dtos;
using DigitalGarden.Data.Sync.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public class SyncFamousQuotesContent : ISyncContent
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SyncBeaconsContent> _logger;

    public SyncFamousQuotesContent(ApplicationDbContext dbContext, ILogger<SyncBeaconsContent> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SynchronizeAsync(JsonSerializerOptions serializerOptions, string contentRootPath, CancellationToken cancellationToken)
    {
        var beaconsFilePath = $"{contentRootPath}/famous-quotes.json";
        if (!File.Exists(beaconsFilePath))
        {
            _logger.LogError("JSON file in {ContentClass} not found.", nameof(SyncFamousQuotesContent));
            return;
        }

        var json = await File.ReadAllTextAsync(beaconsFilePath, cancellationToken);
        var quotesData = JsonSerializer.Deserialize<SyncFamousQuoteData[]>(json, serializerOptions);

        if (quotesData is null)
        {
            _logger.LogError("Parsing of JSON in {ContentClass} failed.", nameof(SyncFamousQuotesContent));
            return;
        }

        await SyncFamousQuotesAsync(quotesData, cancellationToken);
    }

    private async Task SyncFamousQuotesAsync(SyncFamousQuoteData[] quotesToSync, CancellationToken cancellationToken)
    {
        var itemSlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var quoteToSync in quotesToSync)
        {
            if (itemSlugs.Contains(quoteToSync.Slug))
                throw new InvalidOperationException($"Duplicate quote slug '{quoteToSync.Slug}'");

            itemSlugs.Add(quoteToSync.Slug);

            var quoteItem = await _dbContext.FamousQuotes
                .FirstOrDefaultAsync(bc => EF.Functions.ILike(bc.Slug, quoteToSync.Slug), cancellationToken);

            if (quoteItem is null)
            {
                quoteItem = new FamousQuoteDto
                {
                    Text = quoteToSync.Text,
                    Author = quoteToSync.Author,
                    IsAttribution = quoteToSync.IsAttribution,
                    IsFavourite = quoteToSync.IsFavourite,
                    Source = quoteToSync.Source,
                    Year = quoteToSync.Year,
                    Slug = quoteToSync.Slug,
                    AddedAtUtc = DateTime.UtcNow
                };

                await _dbContext.FamousQuotes.AddAsync(quoteItem, cancellationToken);
            }
            else
            {
                quoteItem.Text = quoteToSync.Text;
                quoteItem.Author = quoteToSync.Author;
                quoteItem.IsFavourite = quoteToSync.IsFavourite;
                quoteItem.IsAttribution = quoteToSync.IsAttribution;
                quoteItem.Source = quoteToSync.Source;
                quoteItem.Year = quoteToSync.Year;
            }
        }

        var deletionTargets = await _dbContext.FamousQuotes
            .Where(dto => !itemSlugs.Contains(dto.Slug))
            .ToArrayAsync(cancellationToken);

        if (deletionTargets.Length > 0)
            _dbContext.FamousQuotes.RemoveRange(deletionTargets);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
