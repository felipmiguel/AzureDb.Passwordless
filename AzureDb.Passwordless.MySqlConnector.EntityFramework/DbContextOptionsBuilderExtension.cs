using AzureDb.Passwordless.MySqlConnector;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtension
{
    public static DbContextOptionsBuilder UseAadAuthentication(this DbContextOptionsBuilder optionsBuilder, string? clientId = null)
    {
        return optionsBuilder.AddInterceptors(new AzureIdentityMysqlPasswordProviderInterceptor(clientId));
    }
}