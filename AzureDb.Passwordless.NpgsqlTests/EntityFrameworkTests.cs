using Azure.Identity;
using AzureDb.Passwordless.PostgresqlTests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sample.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.PostgresqlTests
{
    [TestClass]
    public class EntityFrameworkTests
    {
        private static IConfiguration? configuration;

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
            var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
            using var context = await contextFactory.CreateDbContextAsync();
            await SeedData.InitializeAsync(context);
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
                options.UseNpgsql(configuration.GetConnectionString(),
                    npgsqlOptions => npgsqlOptions.UseAzureADAuthentication(new DefaultAzureCredential()));
            });

            var serviceProvider = services.BuildServiceProvider();
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
                options.UseNpgsql(configuration.GetConnectionString(),
                    npgsqlOptions => npgsqlOptions.UseAzureADAuthentication(new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClientId })));
            });

            var serviceProvider = services.BuildServiceProvider();
            await ValidateDbContextAsync(serviceProvider);
        }

        [TestMethod]
        public async Task EFTokenCredential()
        {
            Assert.IsNotNull(configuration);
            AzureCliCredential tokenCredential = new AzureCliCredential();
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(),
                    npgsqlOptions => npgsqlOptions.UseAzureADAuthentication(tokenCredential));
            });

            var serviceProvider = services.BuildServiceProvider();
            await ValidateDbContextAsync(serviceProvider);
        }
    }
}
