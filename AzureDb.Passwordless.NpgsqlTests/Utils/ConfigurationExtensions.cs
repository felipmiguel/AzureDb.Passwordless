using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.PostgresqlTests.Utils
{
    static internal class ConfigurationExtensions
    {
        public static string GetConnectionString(this IConfiguration configuration)
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

        public static string? GetManagedIdentityClientId(this IConfiguration configuration)
        {
            return configuration?.GetSection("msi")?.Value;
        }
    }
}
