using AzureDb.Passwordless.MySql;
using MySql.EntityFrameworkCore.Infrastructure;
using System;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides extension method to MySQLDbContextOptionsBuilder to simplify the usage of AAD authentication to MySql
    /// </summary>
    public static class DbContextOptionsBuilderExtension
    {
        /// <summary>
        /// Configures the context to use AAD authentication to MySql.
        /// </summary>
        /// <param name="optionsBuilder">builder to be configured with AAD authentication</param>
        /// <param name="clientId">If provided, indicates the managed identity to be used</param>
        /// <returns>MySQLDbContextOptionsBuilder configured with AAD authentication</returns>
        public static MySQLDbContextOptionsBuilder UseAzureADAuthentication(this MySQLDbContextOptionsBuilder optionsBuilder, string? clientId = null)
        {
            AzureIdentityMysqlAuthenticationPlugin.RegisterAuthenticationPlugin();
            return optionsBuilder;
        }
    }
}
