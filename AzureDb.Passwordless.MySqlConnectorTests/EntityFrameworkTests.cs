using Azure.Identity;
using AzureDb.Passwordless.MySqlConnectorTests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.MySqlConnectorTests
{
    [TestClass]
    public class EntityFrameworkTests
    {
        private static IConfiguration? configuration;
        private static readonly ServerVersion serverVersion = ServerVersion.Parse("5.7", Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static async Task ValidateDbContextAsync(ServiceProvider serviceProvider)
        {
            Assert.IsNotNull(serviceProvider);
            await SeedData.InitializeAsync(serviceProvider);
            var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
            using var context = await contextFactory.CreateDbContextAsync();
            Assert.IsNotNull(context.Checklists);
            var result = await context.Checklists.ToListAsync();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task EFDefault()
        {
            Assert.IsNotNull(configuration);
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options
                    .UseMySql(configuration.GetConnectionString(), serverVersion)
                    .UseAzureADAuthentication(new DefaultAzureCredential());
            });

            using var serviceProvider = services.BuildServiceProvider();
            await ValidateDbContextAsync(serviceProvider);
        }

        [TestMethod]
        public async Task EFManageIdentity()
        {
            Assert.IsNotNull(configuration);
            string? managedIdentityClientId = configuration.GetManagedIdentityClientId();
            Assert.IsNotNull(managedIdentityClientId);
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options
                    .UseMySql(configuration.GetConnectionString(), serverVersion)
                    .UseAzureADAuthentication(new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClientId }));
            });

            using var serviceProvider = services.BuildServiceProvider();
            await ValidateDbContextAsync(serviceProvider);
        }

        [TestMethod]
        public async Task EFTokenCredential()
        {
            Assert.IsNotNull(configuration);
            AzureCliCredential tokenCredential = new AzureCliCredential();
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>((Action<DbContextOptionsBuilder>)(options =>
            {
                options
                    .UseMySql(configuration.GetConnectionString(), serverVersion)
                    .UseAzureADAuthentication(tokenCredential);
            }));

            using var serviceProvider = services.BuildServiceProvider();
            await ValidateDbContextAsync(serviceProvider);
        }
    }
}
