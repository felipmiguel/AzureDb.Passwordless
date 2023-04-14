using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace AzureDb.Passwordless.MySqlConnectorTests.Utils
{
    static internal class ConfigurationExtensions
    {
        public static string GetConnectionString(this IConfiguration configuration)
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

        public static string? GetManagedIdentityClientId(this IConfiguration configuration)
        {
            return configuration?.GetSection("msi")?.Value;
        }
    }
}
