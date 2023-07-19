// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Bili.Core.TestFramework;
using Microsoft.Azure.Data.Extensions.MySqlConnector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Sample.Repository;
using Sample.Repository.Model;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.Tests
{
    public class DbContextOptionsBuilderTests : LiveTestBase<MySqlTestEnvironment>
    {
        private static readonly ServerVersion serverVersion = ServerVersion.Parse("5.7", ServerType.MySql);

        [Test]
        public async Task EFDefault()
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options
                    .UseMySql(TestEnvironment.ConnectionString, serverVersion)
                    .UseAzureADAuthentication(TestEnvironment.Credential);
            });

            var serviceProvider = services.BuildServiceProvider();
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
}
