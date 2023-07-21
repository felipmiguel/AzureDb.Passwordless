using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Sample.Repository.Tests;

public class ChecklistContextValidator
{
    public async Task ValidateChecklist(ServiceProvider serviceProvider)
    {
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
            using var dbContext = await contextFactory.CreateDbContextAsync();
            await dbContext.Database.OpenConnectionAsync();
            var chk = await dbContext.Checklists.AddAsync(new Checklist
            {
                Date = DateTime.Now,
                Description = "Test sample item",
                Name = "Test Item"
            });
            await dbContext.SaveChangesAsync();

            var chk2 = await dbContext.Checklists.FindAsync(chk.Entity.ID);
            Assert.IsNotNull(chk2);
            dbContext.Checklists.Remove(chk2);
            await dbContext.SaveChangesAsync();
    }
}