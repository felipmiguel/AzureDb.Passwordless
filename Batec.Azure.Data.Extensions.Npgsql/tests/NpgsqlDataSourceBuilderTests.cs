﻿// Licensed under the MIT License.

using Batec.Core.TestFramework;
using Azure.Identity;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batec.Azure.Data.Extensions.Npgsql.Tests
{
    public class NpgsqlDataSourceBuilderTests : TestBase<NpgsqlTestEnvironment>
    {
        private static async Task ValidateDataSourceAsync(NpgsqlDataSource dataSource)
        {
            Assert.IsNotNull(dataSource);
            using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
            using NpgsqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT now()";
            DateTime? serverDate = (DateTime?)await cmd.ExecuteScalarAsync();
            Assert.IsNotNull(serverDate);
        }

        [Test]
        public async Task DataSourceBuilderExtensionDefault()
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(TestEnvironment.ConnectionString);
            NpgsqlDataSource dataSource = dataSourceBuilder
                .UseAzureADAuthentication(TestEnvironment.Credential)
                .Build();
            await ValidateDataSourceAsync(dataSource);
        }
    }
}
