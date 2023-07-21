using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Repository.Model;

namespace Sample.Repository.Tests;

public class ChecklistContextValidator
{
    public static async Task ValidateChecklistAsync(ServiceProvider serviceProvider)
    {
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
        using var dbContext = await contextFactory.CreateDbContextAsync();
        //await dbContext.Database.OpenConnectionAsync();
        if (dbContext.Checklists != null)
        {
            var chk = await dbContext.Checklists.AddAsync(new Checklist
            {
                Date = DateTime.UtcNow,
                Description = "Test sample item",
                Name = "Test Item"
            });
            await dbContext.SaveChangesAsync();

            var chk2 = await dbContext.Checklists.FindAsync(chk.Entity.ID);
            Assert.IsNotNull(chk2);
            if (chk2 != null)
            {
                dbContext.Checklists.Remove(chk2);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}