using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class TopTrackEntryConfiguration : IEntityTypeConfiguration<TopTrackEntryDto>
{
    public void Configure(EntityTypeBuilder<TopTrackEntryDto> builder)
    {
        builder.ToTable(Constants.TopTracksTable);

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.SnapshotId, e.Rank });

        builder.HasIndex(e => new { e.ArtistName, e.Name });
    }
}
