using AzureDb.Passwordless.Postgresql;
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
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder();
            connectionStringBuilder.Host = "[PUT YOUR HOST]";
            connectionStringBuilder.Port = 5432;
            connectionStringBuilder.Database = "checklist";
            connectionStringBuilder.Username = "[PUT YOUR USER]";
            connectionStringBuilder.SslMode = SslMode.Require;
            connectionStringBuilder.TrustServerCertificate = true;
            connectionStringBuilder.Timeout = 30;
            NpgsqlConnection connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
            AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider();
            connection.ProvidePasswordCallback = passwordProvider.ProvidePasswordCallback;
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT now()", connection);
            DateTime? serverDate = (DateTime?)cmd.ExecuteScalar();
            Assert.IsNotNull(serverDate);
            connection.Close();
        }
    }
}
