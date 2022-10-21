using System.Data.Common;
using AzureDb.Passwordless.Postgresql;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore;
public static class DbContextOptionsBuilderExtension
{
    public static NpgsqlDbContextOptionsBuilder UseAadAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder, string? clientId=null)
    {
        AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider(clientId);
        return optionsBuilder.ProvidePasswordCallback(passwordProvider.ProvidePasswordCallback);
    }    
}
