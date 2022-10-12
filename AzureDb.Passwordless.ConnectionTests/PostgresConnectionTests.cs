using AzureDb.Passwordless.Postgresql;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.ConnectionTests
{
    [TestClass]
    public class PostgresConnectionTests
    {
        [TestMethod]
        public void TestConnectionPasswordProvider()
        {
            ConfigurationManager configurationManager = new ConfigurationManager();
            IConfiguration configuration = configurationManager.AddJsonFile("appsettings.json").Build();

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

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
            {
                AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider();
                connection.ProvidePasswordCallback = passwordProvider.ProvidePasswordCallback;
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT now()", connection);
                DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
                Assert.IsNotNull(serverDate);
            }
        }
    }
}
