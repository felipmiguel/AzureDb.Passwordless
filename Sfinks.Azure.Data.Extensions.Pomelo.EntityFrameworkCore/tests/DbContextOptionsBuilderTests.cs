// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Sfinks.Core.TestFramework;
using Sfinks.Azure.Data.Extensions.MySqlConnector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Sample.Repository;
using Sample.Repository.Model;
using Sample.Repository.Tests;
using System;
using System.Threading.Tasks;

namespace Sfinks.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.Tests
{
    public class DbContextOptionsBuilderTests : LiveTestBase<MySqlTestEnvironment>
    {
        private static readonly ServerVersion serverVersion = ServerVersion.Parse("5.7", ServerType.MySql);

        [Test]
        public async Task EFDefault()
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options
                    .UseMySql(TestEnvironment.ConnectionString, serverVersion)
                    .UseAzureADAuthentication(TestEnvironment.Credential);
            });

            var serviceProvider = services.BuildServiceProvider();
            await ChecklistContextValidator.ValidateChecklistAsync(serviceProvider);            
        }
    }
}
