// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Bili.Core.TestFramework;
using Npgsql;

namespace Microsoft.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.Tests
{
    public class NpgsqlTestEnvironment : TestEnvironment
    {
        private string FQDN => GetVariable("POSTGRESQL_FQDN");
        private string Database => GetVariable("POSTGRESQL_DATABASE");

        private string User => GetVariable("POSTGRESQL_SERVER_ADMIN");

        public string ConnectionString
        {
            get
            {
                NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
                {
                    Host = FQDN,
                    Database = Database,
                    Username = User,
                    Port = 5432,
                    SslMode = SslMode.Require,
                    TrustServerCertificate = true,
                    Timeout = 30
                };
                return connectionStringBuilder.ConnectionString;
            }
        }
    }
}
