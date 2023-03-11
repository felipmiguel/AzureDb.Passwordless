using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Repository;

namespace AzureDb.Passwordless.MSSqlTests
{
    [TestClass]
    public class MSSqlConnectionTests
    {
        private static IConfiguration? configuration;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }


        [TestMethod]
        public async Task GetMsSqlToken()
        {
            string RDBMS_SCOPE = "https://database.windows.net/.default";
            TokenRequestContext requestContext = new TokenRequestContext(new string[] { RDBMS_SCOPE });
            DefaultAzureCredential creds = new DefaultAzureCredential();
            AccessToken accessToken = await creds.GetTokenAsync(requestContext);
            Assert.IsNotNull(accessToken);
            Assert.IsNotNull(accessToken.Token);
            Assert.IsTrue(accessToken.ExpiresOn > DateTime.UtcNow);
        }

        [TestMethod]
        public async Task OpenMSSqlConnection()
        {
            Assert.IsNotNull(configuration);
            string connectionString = configuration.GetSection("mssqlconnstring").Value;
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            SqlCommand cmd = new SqlCommand("SELECT GETDATE()", connection);
            DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
            Assert.IsNotNull(serverDate);
        }

        [TestMethod]
        public async Task OpenMSSqlConnectionWithToken()
        {
            Assert.IsNotNull(configuration);
            string connectionString = configuration.GetSection("mssqlconnstringNoAuth").Value;
            string RDBMS_SCOPE = "https://database.windows.net/.default";
            TokenRequestContext requestContext = new TokenRequestContext(new string[] { RDBMS_SCOPE });
            DefaultAzureCredential creds = new DefaultAzureCredential();
            AccessToken accessToken = await creds.GetTokenAsync(requestContext);
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken.Token;
            await connection.OpenAsync();
            SqlCommand cmd = new SqlCommand("SELECT GETDATE()", connection);
            DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
            Assert.IsNotNull(serverDate);
        }

        [TestMethod]
        public async Task OpenMSSqlConnectionEntityFrameworkCore()
        {
            Assert.IsNotNull(configuration);
            string connectionString = configuration.GetSection("mssqlconnstring").Value;
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<ChecklistContext>(options => options.UseSqlServer(connectionString));

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            using var context = serviceProvider.GetRequiredService<ChecklistContext>();
            Assert.IsNotNull(context);
            Assert.IsNotNull(context.Checklists);
            var checklists = await context.Checklists.ToListAsync();
            Assert.IsNotNull(checklists);
        }

        [TestMethod]
        public async Task OpenMSSqlConnectionEntityWithConnectringStringBuilder()
        {
            Assert.IsNotNull(configuration);
            string server = configuration.GetSection("server").Value;
            string database = configuration.GetSection("database").Value;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault;
            builder.TrustServerCertificate = true;
            using SqlConnection connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();
            SqlCommand cmd = new SqlCommand("SELECT GETDATE()", connection);
            DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
            Assert.IsNotNull(serverDate);
        }
    }
}