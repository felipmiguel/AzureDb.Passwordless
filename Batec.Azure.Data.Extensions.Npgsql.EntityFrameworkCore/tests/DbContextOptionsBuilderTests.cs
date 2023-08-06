// Licensed under the MIT License.

using Batec.Core.TestFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Sample.Repository;
using Sample.Repository.Model;
using Sample.Repository.Tests;
using System;
using System.Threading.Tasks;

namespace Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.Tests
{
    public class DbContextOptionsBuilderTests : TestBase<NpgsqlTestEnvironment>
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
        }
    }
}
