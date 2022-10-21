using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample.Repository
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            IDbContextFactory<ChecklistContext> contextFactory= serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
            using (var context = contextFactory.CreateDbContext())
            {
                if (context == null || context.Checklists == null)
                {
                    throw new ArgumentNullException("Null Checklists");
                }

                // Look for any checklist.
                if (context.Checklists.Any())
                {
                    return;   // DB has been seeded
                }

                context.Checklists.AddRange(
                    new Checklist
                    {
                        Name = "Checklist 1",
                        Date = DateTime.UtcNow,
                        Description = "Checklist 1 Description",
                        CheckItems = new List<CheckItem>
                        {
                            new CheckItem { Description = "CheckItem 1"},
                            new CheckItem { Description = "CheckItem 3"},
                            new CheckItem { Description = "CheckItem 4"},
                            new CheckItem { Description = "CheckItem 5"},
                        }
                    },
                    new Checklist
                    {
                        Name = "Checklist 2",
                        Date = DateTime.UtcNow,
                        Description = "Checklist 2 Description",
                        CheckItems = new List<CheckItem>
                        {
                            new CheckItem { Description = "CheckItem 1"},
                            new CheckItem { Description = "CheckItem 3"},
                            new CheckItem { Description = "CheckItem 4"},
                            new CheckItem { Description = "CheckItem 5"},
                        }
                    }
                );
                context.SaveChanges();
            }
        }
    }
}

