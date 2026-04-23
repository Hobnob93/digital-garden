using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class LastFmSnapshotConfiguration : IEntityTypeConfiguration<LastFmSnapshotDto>
{
    public void Configure(EntityTypeBuilder<LastFmSnapshotDto> builder)
    {
        builder.ToTable(Constants.MusicSnapshotsTable);

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.CapturedAtUtc);
    }
}
