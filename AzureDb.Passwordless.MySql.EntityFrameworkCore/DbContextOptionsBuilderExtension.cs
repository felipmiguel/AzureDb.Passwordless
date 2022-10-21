using AzureDb.Passwordless.MySql;
using MySql.EntityFrameworkCore.Infrastructure;
using System;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension
    {
        public static MySQLDbContextOptionsBuilder UseAadAuthentication(this MySQLDbContextOptionsBuilder optionsBuilder, string? clientId = null)
        {
            AzureIdentityMysqlAuthenticationPlugin.RegisterAuthenticationPlugin();
            return optionsBuilder;
        }
    }
}
