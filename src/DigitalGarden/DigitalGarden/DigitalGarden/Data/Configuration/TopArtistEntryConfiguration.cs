using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class TopArtistEntryConfiguration : IEntityTypeConfiguration<TopArtistEntryDto>
{
    public void Configure(EntityTypeBuilder<TopArtistEntryDto> builder)
    {
        builder.ToTable(Constants.TopMusicArtistsTable);

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.SnapshotId, e.Rank });

        builder.HasIndex(e => e.Name);
    }
}
