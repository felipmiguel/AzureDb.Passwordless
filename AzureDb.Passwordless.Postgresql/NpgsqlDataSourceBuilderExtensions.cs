using Azure.Core;
using AzureDb.Passwordless.Postgresql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Npgsql
{
    /// <summary>
    /// NpgsqlDataSourceBuilder extensions that simplify the configuration to use AAD authentication when connecting to Azure Database for Postgresql
    /// </summary>
    public static class NpgsqlDataSourceBuilderExtensions
    {
        /// <summary>
        /// Configures NpgsqlDataSourceBuilder to use AAD authentication to connect to Azure Database for Postgresql.
        /// Use DefaultAzureCredential to get access tokens
        /// </summary>
        /// <param name="builder">NpgsqlDataSourceBuilder to be configured with AAD authentication</param>
        /// <returns>NpgsqlDataSourceBuilder configured with AAD authentication</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null</exception>
        public static NpgsqlDataSourceBuilder UseAadAuthentication(this NpgsqlDataSourceBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseAadAuthentication(new AzureIdentityPostgresqlPasswordProvider());
        }

        /// <summary>
        /// Configures NpgsqlDataSourceBuilder to use AAD authentication to connect to Azure Database for Postgresql using a specific managed identity.
        /// It allows to specify a managed identity when the app hosting has more than one managed identity assigned
        /// </summary>
        /// <param name="builder">NpgsqlDataSourceBuilder to be configured with AAD authentication</param>
        /// <param name="managedIdentityClientId">client id of the managed identity to be used</param>
        /// <returns>NpgsqlDataSourceBuilder configured with AAD authentication</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="managedIdentityClientId"/> are null</exception>
        public static NpgsqlDataSourceBuilder UseAadAuthentication(this NpgsqlDataSourceBuilder builder, string managedIdentityClientId)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (managedIdentityClientId == null)
                throw new ArgumentNullException(nameof(managedIdentityClientId));
            return builder.UseAadAuthentication(new AzureIdentityPostgresqlPasswordProvider(managedIdentityClientId));
        }

        /// <summary>
        /// Configures NpgsqlDataSourceBuilder to use AAD authentication to connect to Azure Database for Postgresql using the provided TokenCredential
        /// </summary>
        /// <param name="builder">NpgsqlDataSourceBuilder to be configured with AAD authentication</param>
        /// <param name="credential">TokenCredential that will be used to retrieve AAD access tokens</param>
        /// <returns>NpgsqlDataSourceBuilder configured with AAD authentication</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="credential"/> are null</exception>
        public static NpgsqlDataSourceBuilder UseAadAuthentication(this NpgsqlDataSourceBuilder builder, TokenCredential credential)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));
            return builder.UseAadAuthentication(new AzureIdentityPostgresqlPasswordProvider(credential));
        }

        /// <summary>
        /// Configures NpgsqlDataSourceBuilder to use AAD authentication to connect to Azure Database for Postgresql using an AzureIdentityPostgresqlPasswordProvider
        /// </summary>
        /// <param name="builder">NpgsqlDataSourceBuilder to be configured with AAD authentication</param>
        /// <param name="passwordProvider">PasswordProvider to be used to retrieve AAD access tokens</param>
        /// <returns>NpgsqlDataSourceBuilder configured with AAD authentication</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="credential"/> are null</exception>
        public static NpgsqlDataSourceBuilder UseAadAuthentication(this NpgsqlDataSourceBuilder builder, AzureIdentityPostgresqlPasswordProvider passwordProvider)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (passwordProvider == null)
                throw new ArgumentNullException(nameof(passwordProvider));
            builder.UsePeriodicPasswordProvider(passwordProvider.PeriodicPasswordProvider, TimeSpan.FromMinutes(29), TimeSpan.FromSeconds(5));
            return builder;
        }
    }
}