using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class BeaconConfiguration : IEntityTypeConfiguration<BeaconDto>
{
    public void Configure(EntityTypeBuilder<BeaconDto> builder)
    {
        builder.ToTable(Constants.BeaconsTable);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.AddedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Beacons)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.CategoryId);

        builder.HasIndex(e => e.Slug)
            .IsUnique();
    }
}
