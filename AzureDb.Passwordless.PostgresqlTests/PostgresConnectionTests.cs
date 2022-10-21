using AzureDb.Passwordless.Postgresql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sample.Repository;

namespace AzureDb.Passwordless.MysqlTests
{
    [TestClass]
    public class PostgresConnectionTests
    {
        private static IConfiguration configuration;
        private static IServiceProvider serviceProvider;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options.UseNpgsql(GetConnectionString(), options => options.UseAadAuthentication());
            });

            serviceProvider = services.BuildServiceProvider();
        }

        private static string GetConnectionString()
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = configuration.GetSection("postgresql:host").Value,
                Database = configuration.GetSection("postgresql:database").Value,
                Username = configuration.GetSection("postgresql:user").Value,
                Port = 5432,
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                Timeout = 30,
            };
            return connectionStringBuilder.ConnectionString;
        }

        [TestMethod]
        public void TestConnectionPasswordProvider()
        {


            AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider();
            using NpgsqlConnection connection = new NpgsqlConnection
            {
                ConnectionString = GetConnectionString(),
                ProvidePasswordCallback = passwordProvider.ProvidePasswordCallback
            };
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT now()", connection);
            DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
            Assert.IsNotNull(serverDate);

        }

        [TestMethod]
        public void FeedData()
        {
            SeedData.Initialize(serviceProvider);
        }

        [TestMethod]
        public void TestEntityFrameworkAad()
        {
            var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ChecklistContext>>();
            using var context = contextFactory.CreateDbContext();
            var result = context.Checklists?.ToList();
            Assert.IsNotNull(result);

        }
    }
}
