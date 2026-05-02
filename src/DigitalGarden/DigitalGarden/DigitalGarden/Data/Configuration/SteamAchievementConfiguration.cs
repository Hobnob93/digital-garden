using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class SteamAchievementConfiguration : IEntityTypeConfiguration<SteamAchievementDto>
{
    public void Configure(EntityTypeBuilder<SteamAchievementDto> builder)
    {
        builder.ToTable(Constants.GameAchievementsTable);

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.AppId);

        builder.HasIndex(e => e.Name);
    }
}
