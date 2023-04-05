using AzureDb.Passwordless.MySqlConnector;
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
        static IServiceProvider? serviceProvider;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                ServerVersion serverVersion = ServerVersion.Parse("5.7", Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql);
                options
                    .UseMySql(GetConnectionString(), serverVersion)
                    .UseAadAuthentication();
            });

            serviceProvider = services.BuildServiceProvider();
        }


        [TestMethod]
        public async Task TestConnectionPasswordProvider()
        {
            AzureIdentityMysqlPasswordProvider passwordProvider = new AzureIdentityMysqlPasswordProvider();
            using MySqlConnection connection = new MySqlConnection(GetConnectionString());
            connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
            await connection.OpenAsync();
            MySqlCommand cmd = new MySqlCommand("SELECT now()", connection);
            DateTime? serverDate = (DateTime?) await cmd.ExecuteScalarAsync();
            Assert.IsNotNull(serverDate);
        }

        [TestMethod]
        public async Task CheckServerVersion()
        {
            AzureIdentityMysqlPasswordProvider passwordProvider = new AzureIdentityMysqlPasswordProvider();
            using MySqlConnection connection = new MySqlConnection(GetConnectionString());
            connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
            await connection.OpenAsync();
            ServerVersion version = ServerVersion.Parse(connection.ServerVersion);
            Assert.AreEqual(5, version.Version.Major);
            Assert.AreEqual(7, version.Version.Minor);            
        }

        [TestMethod]
        public async Task FeedData()
        {
            Assert.IsNotNull(serviceProvider);
            await SeedData.InitializeAsync(serviceProvider);
        }

        private static string? GetConnectionString()
        {
            Assert.IsNotNull(configuration);
            MySqlConnectionStringBuilder connStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Port = 3306,
                SslMode = MySqlSslMode.Required,
                AllowPublicKeyRetrieval = true,
                ConnectionTimeout = 30,

            };
            return connStringBuilder.ConnectionString;
        }
    }
}