using DigitalGarden.Shared.Extensions;
using System.Text.Json.Serialization;

namespace DigitalGarden.Data.Sync.Models.Base;

public abstract class BaseSyncSlugData
{
    [JsonInclude]
    protected string? Slug { get; set; }

    public string Name { get; set; } = string.Empty;

    public string UrlSlug => string.IsNullOrEmpty(Slug)
        ? Name.ToSlugString()
        : Slug;
}
