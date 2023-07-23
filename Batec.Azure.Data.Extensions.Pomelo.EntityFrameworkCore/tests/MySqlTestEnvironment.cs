﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Batec.Core.TestFramework;
using MySqlConnector;

namespace Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.Tests
{
    public class MySqlTestEnvironment : TestEnvironment
    {
        private string FQDN => GetVariable("MYSQL_FQDN");
        private string Database => GetVariable("MYSQL_DATABASE");
        private string User => GetVariable("MYSQL_SERVER_ADMIN");

        public string ConnectionString
        {
            get
            {
                MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
                {
                    Server = FQDN,
                    UserID = User,
                    Database = Database,
                    Port = 3306,
                    SslMode = MySqlSslMode.Required,
                    AllowPublicKeyRetrieval = true,
                    ConnectionTimeout = 30
                };
                return connectionStringBuilder.ConnectionString;
            }
        }
    }
}
