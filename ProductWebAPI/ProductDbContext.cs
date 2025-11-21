using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using ProductWebAPI.Models;

namespace ProductWebAPI
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> dbContextOptions) : base(dbContextOptions)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if( databaseCreator != null)
                {
                    // Create db if cannot connect
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    // Create Tables if not available
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }catch (Exception ex)
            {
                Console.WriteLine($"Database error occured {ex.Message}");
            }
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                // Configure JSON columns
                entity.Property(e => e.ProductGalleryImagesJson)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.AdditionalAttributesJson)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false);

                // Configure other properties if needed
                entity.Property(e => e.ProductPrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.SyncedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
