using Azure.Identity;
using AzureDb.Passwordless.Postgresql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sample.Repository;

namespace AzureDb.Passwordless.PostgresqlTests
{
    [TestClass]
    public class PostgresConnectionTests
    {
        private static IConfiguration? configuration;
        private static IServiceProvider? serviceProvider;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                //NpgsqlDataSourceBuilder sourceBuilder = new NpgsqlDataSourceBuilder();
                //sourceBuilder.UsePeriodicPasswordProvider()
                //options.UseNpgsql()
                options.UseNpgsql(GetConnectionString(), options => options.UseAadAuthentication());
            });

            serviceProvider = services.BuildServiceProvider();
        }

        private static string GetConnectionString()
        {
            Assert.IsNotNull(configuration);
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = configuration.GetSection("postgresql:host").Value,
                Database = configuration.GetSection("postgresql:database").Value,
                Username = configuration.GetSection("postgresql:user").Value,
                Port = 5432,
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                Timeout = 30
            };
            return connectionStringBuilder.ConnectionString;
        }

        [TestMethod]
        public async Task TestConnectionPasswordProviderExplicit()
        {
            AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider();
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                .UsePeriodicPasswordProvider(passwordProvider.PeriodicPasswordProvider, TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(100))
                .Build();
            using NpgsqlCommand cmd = dataSource.CreateCommand("SELECT now()");
            DateTime? serverDate = (DateTime?)await cmd.ExecuteScalarAsync();
            Assert.IsNotNull(serverDate);
        }

        [TestMethod]
        public async Task TestConnectionAadAuthentication()
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(GetConnectionString());
            NpgsqlDataSource dataSource = dataSourceBuilder
                .UseAadAuthentication()
                .Build();
            using NpgsqlCommand cmd = dataSource.CreateCommand("SELECT now()");
            DateTime? serverDate = (DateTime?) await cmd.ExecuteScalarAsync();
            Assert.IsNotNull(serverDate);
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
