using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Repository;

namespace AzureDb.Passwordless.MSSqlTests
{
    [TestClass]
    public class MSSqlConnectionTests
    {
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
            using SqlConnection connection = new SqlConnection("Server=tcp:mssql-passwordless.database.windows.net;Database=checklist;Authentication=Active Directory Default;TrustServerCertificate=True");
            await connection.OpenAsync();
            SqlCommand cmd = new SqlCommand("SELECT GETDATE()", connection);
            DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
            Assert.IsNotNull(serverDate);
        }

        [TestMethod]
        public async Task OpenMSSqlConnectionWithToken()
        {
            string RDBMS_SCOPE = "https://database.windows.net/.default";
            TokenRequestContext requestContext = new TokenRequestContext(new string[] { RDBMS_SCOPE });
            DefaultAzureCredential creds = new DefaultAzureCredential();
            AccessToken accessToken = await creds.GetTokenAsync(requestContext);
            using SqlConnection connection = new SqlConnection($"Server=tcp:mssql-passwordless.database.windows.net;Database=checklist;TrustServerCertificate=True;");
            connection.AccessToken = accessToken.Token;
            await connection.OpenAsync();
            SqlCommand cmd = new SqlCommand("SELECT GETDATE()", connection);
            DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
            Assert.IsNotNull(serverDate);
        }

        [TestMethod]
        public async Task OpenMSSqlConnectionEntityFrameworkCore()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<ChecklistContext>(options =>
            options.UseSqlServer("Server=tcp:mssql-passwordless.database.windows.net;Database=checklist;Authentication=Active Directory Default;TrustServerCertificate=True"));

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            using var context = serviceProvider.GetRequiredService<ChecklistContext>();
            var checklists = await context.Checklists.ToListAsync();
            Assert.IsNotNull(checklists);
        }
    }
}