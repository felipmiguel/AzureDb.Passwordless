using AzureDb.Passwordless.Core;
using AzureDb.Passwordless.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.ConnectionTests
{
    [TestClass]
    public class MySqlConnectionTests
    {
        static IConfiguration configuration;
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ConfigurationManager configurationManager = new ConfigurationManager();
            configuration = configurationManager
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [TestMethod]
        public async Task DefaultCredentials()
        {
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                AuthenticationPlugins = $"mysql_clear_password:{typeof(AzureIdentityMysqlAuthenticationPlugin).AssemblyQualifiedName.Replace(',', '#')}",
                SslMode = MySqlSslMode.Required,
                DefaultAuthenticationPlugin = "mysql_clear_password",
                ConnectionTimeout = 120
            };


            //MySqlConfiguration.Settings.AuthenticationPlugins.First(p => p.Name == "mysql_clear_password").Type = fullName;
            await DoConnectAsync(connectionStringBuilder);
        }

        private static async Task DoConnectAsync(MySqlConnectionStringBuilder connectionStringBuilder)
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT now()";
                var serverTime = await cmd.ExecuteScalarAsync();
                Assert.IsNotNull(serverTime);
                Assert.IsInstanceOfType(serverTime, typeof(DateTime));
            }
        }

        [TestMethod]
        public async Task TokenAsPassword()
        {
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                Password = AuthenticationHelper.GetAccessToken(null),
                SslMode = MySqlSslMode.Required,
                DefaultAuthenticationPlugin = "mysql_clear_password",
                ConnectionTimeout = 120
            };
            await DoConnectAsync(connectionStringBuilder);
        }
    }
}