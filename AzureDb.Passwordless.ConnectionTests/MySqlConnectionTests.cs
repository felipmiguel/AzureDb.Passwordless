using AzureDb.Passwordless.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.ConnectionTests
{
    [TestClass]
    public class MySqlConnectionTests
    {
        public readonly IConfiguration _configuration;
        //public MySqlConnectionTests()
        //{
        //    ConfigurationManager manager = new ConfigurationManager();

        //    _configuration = new ConfigurationBuilder()
        //        .AddJsonFile("appsettings.json")
        //        .Build();
        //    manager.AddConfiguration(_configuration);
        //    var s= manager.GetSection("MySQL");
        //}
        [TestMethod]
        public async Task DefaultCredentials()
        {
            string fullName = typeof(AzureIdentityMysqlAuthenticationPlugin).FullName;
            
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
            connectionStringBuilder.DefaultAuthenticationPlugin = "mysql_clear_password";
            connectionStringBuilder.Server = "[PUT YOUR SERVER]";
            connectionStringBuilder.UserID = "[PUT YOUR USER";
            connectionStringBuilder.Database = "checklist";
            connectionStringBuilder.SslMode = MySqlSslMode.Required;
            MySqlConnection connection = new MySqlConnection(connectionStringBuilder.GetConnectionString(false));
            await connection.OpenAsync();
            await connection.CloseAsync();
        }
    }
}