// Licensed under the MIT License.

using Batec.Core.TestFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Sample.Repository;
using Sample.Repository.Tests;
using System.Threading.Tasks;

namespace Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.Tests
{
    public class DbContextOptionsBuilderTests : TestBase<MySqlTestEnvironment>
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
            await ChecklistContextValidator.ValidateChecklistAsync(serviceProvider);            
        }
    }
}
