using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class RecentLifeLogsConfiguration : IEntityTypeConfiguration<LifeLogItemDto>
{
    public void Configure(EntityTypeBuilder<LifeLogItemDto> builder)
    {
        builder.ToTable(Constants.RecentLifeLogsTable);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(100);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.AddedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Slug)
            .IsUnique();
    }
}
