using Azure.Identity;
using Microsoft.Azure.Data.Extensions.Npgsql;
using Microsoft.Azure.Data.Extensions.NpgsqlTests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sample.Repository;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Data.Extensions.NpgsqlTests
{
    [TestClass]
    public class PostgresConnectionTests
    {
        private static IConfiguration? configuration;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static async Task ValidateDataSourceAsync(NpgsqlDataSource dataSource)
        {
            Assert.IsNotNull(dataSource);
            using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
            using NpgsqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT now()";
            DateTime? serverDate = (DateTime?)await cmd.ExecuteScalarAsync();
            Assert.IsNotNull(serverDate);
        }

        [TestMethod]
        public async Task NoExtensionDefaultAzureCredential()
        {
            Assert.IsNotNull(configuration);
            TokenCredentialNpgsqlPasswordProvider passwordProvider = new TokenCredentialNpgsqlPasswordProvider(new DefaultAzureCredential());
            // Connection string does not contain password
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                            .UsePeriodicPasswordProvider(passwordProvider.PasswordProvider, TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(100))
                            .Build();
            await ValidateDataSourceAsync(dataSource);
        }

        [TestCategory("server-only")]
        [TestMethod]
        public async Task NoExtensionConstructorWithManagedIdentity()
        {
            Assert.IsNotNull(configuration);
            string? managedIdentityClientId = configuration.GetManagedIdentityClientId();
            Assert.IsNotNull(managedIdentityClientId);
            TokenCredentialNpgsqlPasswordProvider passwordProvider = new TokenCredentialNpgsqlPasswordProvider(new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClientId }));
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                            .UsePeriodicPasswordProvider(passwordProvider.PasswordProvider, TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(100))
                            .Build();
            await ValidateDataSourceAsync(dataSource);
        }

        [TestCategory("local-only")]
        [TestMethod]
        public async Task NoExtensionConstructorWithTokenCredential()
        {
            Assert.IsNotNull(configuration);
            AzureCliCredential credential = new AzureCliCredential();
            TokenCredentialNpgsqlPasswordProvider passwordProvider = new TokenCredentialNpgsqlPasswordProvider(credential);
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                            .UsePeriodicPasswordProvider(passwordProvider.PasswordProvider, TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(100))
                            .Build();
            await ValidateDataSourceAsync(dataSource);
        }
    }
}
