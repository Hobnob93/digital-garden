using DigitalGarden.Data.Dtos;
using DigitalGarden.Shared.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalGarden.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<BeaconCategoryDto> BeaconCategories => Set<BeaconCategoryDto>();
    public DbSet<BeaconDto> Beacons => Set<BeaconDto>();
    public DbSet<FamousQuoteDto> FamousQuotes => Set<FamousQuoteDto>();

    public DbSet<ContentCategory> ContentCategories => Set<ContentCategory>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<ContentCategory>(entity =>
        {
            entity.ToTable(Constants.CategoriesTable);

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.MaterialIcon)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(c => c.Slug)
                .IsUnique();
        });
    }
}
