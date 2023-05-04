using Azure.Identity;
using Microsoft.Azure.Data.Extensions.NpgsqlTests.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sample.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Data.Extensions.NpgsqlTests
{
    [TestClass]
    public class NpgsqlDataSourceExtensionTests
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
        public async Task DataSourceBuilderExtensionDefault()
        {
            Assert.IsNotNull(configuration);
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                .UseAzureADAuthentication(new DefaultAzureCredential())
                .Build();
            await ValidateDataSourceAsync(dataSource);
        }

        [TestCategory("server-only")]
        [TestMethod]
        public async Task DataSourceBuilderExtensionWithManagedIdentity()
        {
            Assert.IsNotNull(configuration);
            string? managedIdentityClientId = configuration.GetManagedIdentityClientId();
            Assert.IsNotNull(managedIdentityClientId);

            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                .UseAzureADAuthentication(new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClientId }))
                .Build();
            await ValidateDataSourceAsync(dataSource);
        }

        [TestCategory("local-only")]
        [TestMethod]
        public async Task DataSourceBuilderExtensionWithTokenCredential()
        {
            Assert.IsNotNull(configuration);
            AzureCliCredential tokenCredential = new AzureCliCredential();
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                .UseAzureADAuthentication(tokenCredential)
                .Build();
            await ValidateDataSourceAsync(dataSource);
        }
    }
}
