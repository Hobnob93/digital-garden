using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class TraktShowConfiguration : IEntityTypeConfiguration<TraktShowDto>
{
    public void Configure(EntityTypeBuilder<TraktShowDto> builder)
    {
        builder.ToTable(Constants.TraktShowsTable);

        builder.Property(e => e.TraktId)
            .ValueGeneratedNever();

        builder.HasKey(e => e.TraktId);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(e => e.ImdbId)
            .IsRequired()
            .HasMaxLength(20);
    }
}
