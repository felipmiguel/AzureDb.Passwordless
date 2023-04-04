using Azure.Core;
using AzureDb.Passwordless.Postgresql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Microsoft.EntityFrameworkCore;
public static class DbContextOptionsBuilderExtension
{
    public static NpgsqlDbContextOptionsBuilder UseAadAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder, string? clientId = null)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));
        AzureIdentityPostgresqlPasswordProvider passwordProvider = (clientId == null)
            ? new AzureIdentityPostgresqlPasswordProvider()
            : new AzureIdentityPostgresqlPasswordProvider(clientId);
        return optionsBuilder.UseAadAuthentication(passwordProvider);
    }

    public static NpgsqlDbContextOptionsBuilder UseAadAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder, TokenCredential credential)
    {
        AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider(credential);
        return optionsBuilder.UseAadAuthentication(passwordProvider);
    }

    public static NpgsqlDbContextOptionsBuilder UseAadAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder, AzureIdentityPostgresqlPasswordProvider passwordProvider)
    {
        return optionsBuilder.ProvidePasswordCallback(passwordProvider.ProvidePasswordCallback);
    }
}
