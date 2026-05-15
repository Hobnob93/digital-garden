using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class TraktMovieConfiguration : IEntityTypeConfiguration<TraktMovieDto>
{
    public void Configure(EntityTypeBuilder<TraktMovieDto> builder)
    {
        builder.ToTable(Constants.TraktMoviesTable);

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
