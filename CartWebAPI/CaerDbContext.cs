using CartWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace CartWebAPI
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> DbContextOptions) : base(DbContextOptions)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An Database Error occured msg:{ex.Message}");
            }
        }

        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Cart entity (which maps to CartItems table)
            modelBuilder.Entity<CartItem>(entity =>
            {
                // Create unique index on UserId and ProductId combination
                entity.HasIndex(e => new { e.UserId, e.ProductId })
                    .IsUnique()
                    .HasDatabaseName("IX_CartItems_UserId_ProductId");

                // Configure properties
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
