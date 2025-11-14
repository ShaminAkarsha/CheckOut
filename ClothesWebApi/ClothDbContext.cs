using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClothesWebApi
{
    public class ClothDbContext : DbContext
    {
        public ClothDbContext(DbContextOptions<ClothDbContext> dbContextOptions) : base(dbContextOptions)
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
        public DbSet<Models.Clothe> Clothes { get; set; }
    }
}
