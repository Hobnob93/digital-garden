using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class DailyIngestSnapshotConfiguration : IEntityTypeConfiguration<DailyIngestSnapshotDto>
{
    public void Configure(EntityTypeBuilder<DailyIngestSnapshotDto> builder)
    {
        builder.ToTable(Constants.DailyIngestSnapshotsTable);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CapturedAtUtc)
            .IsRequired();
    }
}
