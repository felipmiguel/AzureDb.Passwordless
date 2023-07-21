// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Bili.Core.TestFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Sample.Repository;
using Sample.Repository.Model;
using Sample.Repository.Tests;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.Tests
{
    public class DbContextOptionsBuilderTests : LiveTestBase<NpgsqlTestEnvironment>
    {
        [Test]
        public async Task EFDefault()
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options.UseNpgsql(TestEnvironment.ConnectionString,
                    npgsqlOptions => npgsqlOptions.UseAzureADAuthentication(TestEnvironment.Credential));
            });

            var serviceProvider = services.BuildServiceProvider();
            await ChecklistContextValidator.ValidateChecklistAsync(serviceProvider);
            //var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
            //using var dbContext = await contextFactory.CreateDbContextAsync();
            //await dbContext.Database.OpenConnectionAsync();
            //var chk = await dbContext.Checklists.AddAsync(new Checklist
            //{
            //    Date = DateTime.UtcNow,
            //    Description = "Test sample item",
            //    Name = "Test Item"
            //});
            //await dbContext.SaveChangesAsync();

            //var chk2 = await dbContext.Checklists.FindAsync(chk.Entity.ID);
            //Assert.IsNotNull(chk2);
            //dbContext.Checklists.Remove(chk2);
            //await dbContext.SaveChangesAsync();
        }
    }
}
