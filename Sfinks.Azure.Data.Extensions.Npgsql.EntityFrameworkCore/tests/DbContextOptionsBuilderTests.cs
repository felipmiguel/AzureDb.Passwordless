// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Sfinks.Core.TestFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Sample.Repository;
using Sample.Repository.Model;
using Sample.Repository.Tests;
using System;
using System.Threading.Tasks;

namespace Sfinks.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.Tests
{
    public class DbContextOptionsBuilderTests : LiveTestBase<NpgsqlTestEnvironment>
    {
        [Test]
        public async Task EFDefault()
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<ChecklistContext>(options =>
            {
                options.UseNpgsql(TestEnvironment.ConnectionString,
                    npgsqlOptions => npgsqlOptions.UseAzureADAuthentication(TestEnvironment.Credential));
            });

            var serviceProvider = services.BuildServiceProvider();
            await ChecklistContextValidator.ValidateChecklistAsync(serviceProvider);
        }
    }
}
