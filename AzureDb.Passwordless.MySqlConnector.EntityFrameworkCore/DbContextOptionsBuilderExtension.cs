using Azure.Core;
using AzureDb.Passwordless.MySqlConnector;
using AzureDb.Passwordless.MySqlConnector.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtension
{
    public static DbContextOptionsBuilder UseAadAuthentication(this DbContextOptionsBuilder optionsBuilder, string? clientId = null)
    {
        // see: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1643
        return optionsBuilder.AddInterceptors(new AzureIdentityMysqlPasswordProviderInterceptor(clientId));
    }

    public static DbContextOptionsBuilder UseAadAuthentication(this DbContextOptionsBuilder optionsBuilder, TokenCredential credential)
    {
        // see: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1643
        return optionsBuilder.AddInterceptors(new AzureIdentityMysqlPasswordProviderInterceptor(credential));
    }
}