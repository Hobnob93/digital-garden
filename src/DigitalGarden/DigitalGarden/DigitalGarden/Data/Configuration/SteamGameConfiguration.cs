using DigitalGarden.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalGarden.Data.Configuration;

public class SteamGameConfiguration : IEntityTypeConfiguration<SteamGameDto>
{
    public void Configure(EntityTypeBuilder<SteamGameDto> builder)
    {
        builder.ToTable(Constants.GamesTable);

        builder.Property(e => e.AppId)
            .ValueGeneratedNever();

        builder.HasKey(e => e.AppId);
    }
}
