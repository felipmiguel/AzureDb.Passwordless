using Azure.Core;
using AzureDb.Passwordless.Postgresql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Npgsql
{
    public static class NpgsqlDataSourceBuilderExtensions
    {
        public static NpgsqlDataSourceBuilder UseAadAuthentication(this NpgsqlDataSourceBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseAadAuthentication(new AzureIdentityPostgresqlPasswordProvider());
        }

        public static NpgsqlDataSourceBuilder UseAadAuthentication(this NpgsqlDataSourceBuilder builder, string managedIdentityClientId)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseAadAuthentication(new AzureIdentityPostgresqlPasswordProvider(managedIdentityClientId));
        }

        public static NpgsqlDataSourceBuilder UseAadAuthentication(this NpgsqlDataSourceBuilder builder, TokenCredential credential)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseAadAuthentication(new AzureIdentityPostgresqlPasswordProvider(credential));
        }

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