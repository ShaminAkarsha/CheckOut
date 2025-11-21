using IntegrationService.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrationService.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IntegrationDbContext db)
        {
            if (!await db.Adapters.AnyAsync())
            {
                await db.Adapters.AddRangeAsync(
                    new Adapter
                    {
                        AdapterName = "Bokun",
                        BaseUrl = "http://bokunadapterwebapi:8080/api",
                        IsActive = true
                    },
                    new Adapter
                    {
                        AdapterName = "Handycraft",
                        BaseUrl = "http://handycraftsadapterwebapi:8080/api",
                        IsActive = true
                    }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}
