using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class BeaconCategoryConfiguration : IEntityTypeConfiguration<BeaconCategoryDto>
{
    public void Configure(EntityTypeBuilder<BeaconCategoryDto> builder)
    {
        builder.ToTable(Constants.BeaconCategoriesTable);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.HasIndex(e => e.Slug)
            .IsUnique();
    }
}
