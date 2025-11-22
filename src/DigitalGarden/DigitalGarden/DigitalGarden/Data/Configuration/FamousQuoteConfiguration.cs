using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class FamousQuoteConfiguration : IEntityTypeConfiguration<FamousQuoteDto>
{
    public void Configure(EntityTypeBuilder<FamousQuoteDto> builder)
    {
        builder.ToTable(Constants.QuotesTable);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Author)
            .HasMaxLength(100);

        builder.Property(e => e.Source)
            .HasMaxLength(200);

        builder.Property(e => e.AddedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Slug)
            .IsUnique();
    }
}
