using Azure.Identity;
using AzureDb.Passwordless.MySqlConnector;
using AzureDb.Passwordless.MySqlConnectorTests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Sample.Repository;
using System;

namespace AzureDb.Passwordless.MySqlConnectorTests
{
    [TestClass]
    public class MySqlConnectionTests
    {
        static IConfiguration? configuration;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private static async Task ValidateConnectionAsync(MySqlConnection connection)
        {
            Assert.IsNotNull(connection);
            await connection.OpenAsync();
            ServerVersion version = ServerVersion.Parse(connection.ServerVersion);
            Assert.AreEqual(5, version.Version.Major);
            Assert.AreEqual(7, version.Version.Minor);
            MySqlCommand cmd = new MySqlCommand("SELECT now()", connection);
            DateTime? serverDate = (DateTime?)await cmd.ExecuteScalarAsync();
            Assert.IsNotNull(serverDate);
        }


        [TestMethod]
        public async Task ProviderDefaultConstructor()
        {
            Assert.IsNotNull(configuration);
            AzureIdentityMySqlPasswordProvider passwordProvider = new AzureIdentityMySqlPasswordProvider();
            using MySqlConnection connection = new MySqlConnection(configuration.GetConnectionString());
            connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
            await ValidateConnectionAsync(connection);            
        }

        [TestMethod]
        public async Task ProviderWithManagedIdentity()
        {
            Assert.IsNotNull(configuration);
            string? managedIdentityClientId = configuration.GetManagedIdentityClientId();
            Assert.IsNotNull(managedIdentityClientId);
            AzureIdentityMySqlPasswordProvider passwordProvider = new AzureIdentityMySqlPasswordProvider(managedIdentityClientId);
            using MySqlConnection connection = new MySqlConnection(configuration.GetConnectionString());
            connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
            await ValidateConnectionAsync(connection);
        }

        [TestMethod]
        public async Task ProviderWithTokenCredential()
        {
            Assert.IsNotNull(configuration);
            AzureCliCredential credential = new AzureCliCredential();
            AzureIdentityMySqlPasswordProvider passwordProvider = new AzureIdentityMySqlPasswordProvider(credential);
            using MySqlConnection connection = new MySqlConnection(configuration.GetConnectionString());
            connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
            await ValidateConnectionAsync(connection);
        }
    }
}