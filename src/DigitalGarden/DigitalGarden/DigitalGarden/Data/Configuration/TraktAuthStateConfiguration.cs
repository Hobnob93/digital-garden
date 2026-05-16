using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class TraktAuthStateConfiguration : IEntityTypeConfiguration<TraktAuthStateDto>
{
    public void Configure(EntityTypeBuilder<TraktAuthStateDto> builder)
    {
        builder.ToTable(Constants.TraktAuthTable);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.AccessToken)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.RefreshToken)
            .IsRequired()
            .HasMaxLength(100);
    }
}
