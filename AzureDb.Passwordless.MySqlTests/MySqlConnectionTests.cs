using AzureDb.Passwordless.Core;
using AzureDb.Passwordless.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Sample.Repository;
using Azure.Identity;

namespace AzureDb.Passwordless.MysqlTests
{
    [TestClass]
    public class MySqlConnectionTests
    {
        static IConfiguration? configuration;
        private static IServiceProvider? serviceProvider;
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ConfigurationManager configurationManager = new ConfigurationManager();
            configuration = configurationManager
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                SslMode = MySqlSslMode.Required,
                DefaultAuthenticationPlugin = "mysql_clear_password",
                ConnectionTimeout = 120
            };
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options.UseMySQL(connectionStringBuilder.ConnectionString, options => options.UseAzureADAuthentication());
            });

            serviceProvider = services.BuildServiceProvider();
        }


        [TestMethod]
        public async Task AuthenticationPluginsInConnectionString()
        {
            Assert.IsNotNull(configuration);
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                AuthenticationPlugins = $"mysql_clear_password:{typeof(AzureIdentityMysqlAuthenticationPlugin).AssemblyQualifiedName?.Replace(',', '#')}",
                SslMode = MySqlSslMode.Required,
                DefaultAuthenticationPlugin = "mysql_clear_password",
                ConnectionTimeout = 120
            };

            AzureIdentityMysqlAuthenticationPlugin.RegisterAuthenticationPlugin();
            await DoConnectAsync(connectionStringBuilder);
        }

        [TestMethod]
        public async Task AuthenticationPluginsRegisteredByReflection()
        {
            Assert.IsNotNull(configuration);
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                SslMode = MySqlSslMode.Required,
                DefaultAuthenticationPlugin = "mysql_clear_password",
                ConnectionTimeout = 120
            };

            AzureIdentityMysqlAuthenticationPlugin.RegisterAuthenticationPlugin();
            string authPlugin = AzureIdentityMysqlAuthenticationPlugin.GetAuthenticationPlugin("mysql_clear_password");
            Assert.AreEqual(typeof(AzureIdentityMysqlAuthenticationPlugin).AssemblyQualifiedName, authPlugin);
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
            Assert.IsNotNull(configuration);
            TokenCredentialBaseAuthenticationProvider authenticationPlugin = new TokenCredentialBaseAuthenticationProvider(new DefaultAzureCredential());
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                Password = await authenticationPlugin.GetAuthenticationTokenAsync(),
                SslMode = MySqlSslMode.Required,
                DefaultAuthenticationPlugin = "mysql_clear_password",
                ConnectionTimeout = 120
            };
            await DoConnectAsync(connectionStringBuilder);
        }

        [TestMethod]
        public async Task FeedData()
        {
            Assert.IsNotNull(serviceProvider);
            await SeedData.InitializeAsync(serviceProvider);
        }

        [TestMethod]
        public async Task TestEntityFrameworkAad()
        {
            Assert.IsNotNull(serviceProvider);
            var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
            using var context = await contextFactory.CreateDbContextAsync();
            Assert.IsNotNull(context.Checklists);
            var result = await context.Checklists.ToListAsync();
            Assert.IsNotNull(result);

        }
    }
}