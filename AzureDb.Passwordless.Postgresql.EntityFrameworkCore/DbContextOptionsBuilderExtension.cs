using Azure.Core;
using AzureDb.Passwordless.Postgresql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides extension methods that simplifies the configuration to use Azure AD authentication to access to Azure Database for Postgresql with Npgsql Entity Framework
/// </summary>
public static class DbContextOptionsBuilderExtension
{
    /// <summary>
    /// Configures NpgsqlDbContextOptionsBuilder to use AAD authentication.
    /// If app hosting has more than one managed identity assigned, it is possible to specify which one should be used.
    /// </summary>
    /// <param name="optionsBuilder">NpgsqlDbContextOptionsBuilder to be configured</param>
    /// <param name="clientId">Optional client id of the managed identity to be used</param>
    /// <returns>NpgsqlDbContextOptionsBuilder configured with AAD authentication</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsBuilder"/> is null</exception>
    public static NpgsqlDbContextOptionsBuilder UseAadAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder, string? clientId = null)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));
        AzureIdentityPostgresqlPasswordProvider passwordProvider = (clientId == null)
            ? new AzureIdentityPostgresqlPasswordProvider()
            : new AzureIdentityPostgresqlPasswordProvider(clientId);
        return optionsBuilder.UseAadAuthentication(passwordProvider);
    }

    /// <summary>
    /// Configures NpgsqlDbContextOptionsBuilder to use AAD authentication with a given TokenCredential.
    /// </summary>
    /// <param name="optionsBuilder">NpgsqlDbContextOptionsBuilder to be configured</param>
    /// <param name="credential">TokenCredential that will be used to retrieve AAD access tokens</param>
    /// <returns>NpgsqlDbContextOptionsBuilder configured with AAD authentication</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsBuilder"/> or <paramref name="credential"/> are null</exception>
    public static NpgsqlDbContextOptionsBuilder UseAadAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder, TokenCredential credential)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));
        if (credential == null)
            throw new ArgumentNullException(nameof(credential));
        AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider(credential);
        return optionsBuilder.UseAadAuthentication(passwordProvider);
    }

    /// <summary>
    /// Configures NpgsqlDbContextOptionsBuilder to use AAD authentication with a given AzureIdentityPostgresqlPasswordProvider.
    /// </summary>
    /// <param name="optionsBuilder">NpgsqlDbContextOptionsBuilder to be configured</param>
    /// <param name="passwordProvider">Password provider that will be used to retrieve access tokens to access to Azure Database for Postgresql</param>
    /// <returns>NpgsqlDbContextOptionsBuilder configured with AAD authentication</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsBuilder"/> or <paramref name="passwordProvider"/> are null</exception>
    public static NpgsqlDbContextOptionsBuilder UseAadAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder, AzureIdentityPostgresqlPasswordProvider passwordProvider)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));
        if (passwordProvider == null)
            throw new ArgumentNullException(nameof(passwordProvider));
        return optionsBuilder.ProvidePasswordCallback(passwordProvider.ProvidePasswordCallback);
    }
}
