using Azure.Core;
using AzureDb.Passwordless.MySqlConnector;
using AzureDb.Passwordless.MySqlConnector.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides extension methods that simplifies the configuration to use Azure AD authentication to access to Azure Database for MySQL when using Entity Framework with Pomelo
/// </summary>
public static class DbContextOptionsBuilderExtension
{
    /// <summary>
    /// Configures DbContext to use AAD issued tokens as passwords to access to Azure Database for MySQL.
    /// </summary>
    /// <param name="optionsBuilder">options builder to configure</param>
    /// <param name="clientId">Optional client id of the managed identity to use in case the app hosting has more than one assigned</param>
    /// <returns>DbContextOptionsBuilder configured with AAD authentication</returns>
    public static DbContextOptionsBuilder UseAadAuthentication(this DbContextOptionsBuilder optionsBuilder, string? clientId = null)
    {
        // see: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1643
        return optionsBuilder.AddInterceptors(new AzureIdentityMysqlPasswordProviderInterceptor(clientId));
    }

    /// <summary>
    /// Configures DbContext to use AAD issued tokens as passwords to access to Azure Database for MySQL, providing the TokenCredential to be used to retrieve access tokens
    /// </summary>
    /// <param name="optionsBuilder">DbContextOptionsBuilder to configure with AAD authentication</param>
    /// <param name="credential">TokenCredential provided by the call to retrieve AAD tokens</param>
    /// <returns>DbContextOptionsBuilder configured with AAD authentication</returns>
    public static DbContextOptionsBuilder UseAadAuthentication(this DbContextOptionsBuilder optionsBuilder, TokenCredential credential)
    {
        // see: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1643
        return optionsBuilder.AddInterceptors(new AzureIdentityMysqlPasswordProviderInterceptor(credential));
    }
}