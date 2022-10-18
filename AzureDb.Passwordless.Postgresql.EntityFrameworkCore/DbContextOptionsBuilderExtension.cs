using System.Data.Common;
using AzureDb.Passwordless.Postgresql;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore;
public static class DbContextOptionsBuilderExtension
{
    private static NpgsqlDbContextOptionsBuilder EnhanceDbContextOptionsBuilder(this NpgsqlDbContextOptionsBuilder optionsBuilder, string? clientId = null)
    {
        AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider(clientId);
        return optionsBuilder.ProvidePasswordCallback(passwordProvider.ProvidePasswordCallback);
    }

    public static DbContextOptionsBuilder UseAadAuthentication(this DbContextOptionsBuilder optionsBuilder, string? clientId=null)
    {
        AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider(clientId);

        
        var extension = (NpgsqlOptionsExtension)GetOrCreateExtension(optionsBuilder).WithProvidePasswordCallback(passwordProvider.ProvidePasswordCallback);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        
        return optionsBuilder;
    }

    private static NpgsqlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>() is { } existing
            ? new NpgsqlOptionsExtension(existing)
            : new NpgsqlOptionsExtension();

    // public static DbContextOptionsBuilder UseNpgsqlWithAadAuth(this DbContextOptionsBuilder optionsBuilder, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    // {
    //     return optionsBuilder.UseNpgsql(options =>
    //     {
    //         if (npgsqlOptionsAction != null)
    //         {
    //             npgsqlOptionsAction(options);
    //         }
    //         options.EnhanceDbContextOptionsBuilder();
    //     });
    // }

    // public static DbContextOptionsBuilder UseNpgsqlWithAadAuth(this DbContextOptionsBuilder optionsBuilder, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    // {
    //     return optionsBuilder.UseNpgsql(options =>
    //     {
    //         if (npgsqlOptionsAction != null)
    //         {
    //             npgsqlOptionsAction(options);
    //         }
    //         options.EnhanceDbContextOptionsBuilder();
    //     }).u;
    // }
    // public static DbContextOptionsBuilder UseNpgsqlWithAadAuth(this DbContextOptionsBuilder optionsBuilder, string connectionString, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    // {
    //     return optionsBuilder.UseNpgsql(connectionString, options =>
    //     {
    //         if (npgsqlOptionsAction != null)
    //         {
    //             npgsqlOptionsAction(options);
    //         }
    //         options.EnhanceDbContextOptionsBuilder();
    //     });
    // }
    // public static DbContextOptionsBuilder UseNpgsqlWithAadAuth(this DbContextOptionsBuilder optionsBuilder, DbConnection connection, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    // {
    //     return optionsBuilder.UseNpgsql(connection, options =>
    //     {
    //         if (npgsqlOptionsAction != null)
    //         {
    //             npgsqlOptionsAction(options);
    //         }
    //         options.EnhanceDbContextOptionsBuilder();
    //     });
    // }
    // public static DbContextOptionsBuilder<TContext> UseNpgsqlWithAadAuth<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null) where TContext : DbContext
    // {
    //     return optionsBuilder.UseNpgsql<TContext>(options =>
    //     {
    //         if (npgsqlOptionsAction != null)
    //         {
    //             npgsqlOptionsAction(options);
    //         }
    //         options.EnhanceDbContextOptionsBuilder();
    //     });
    // }
    // public static DbContextOptionsBuilder<TContext> UseNpgsqlWithAadAuth<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, string connectionString, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null) where TContext : DbContext
    // {
    //     return optionsBuilder.UseNpgsql<TContext>(connectionString, options =>
    //     {
    //         if (npgsqlOptionsAction != null)
    //         {
    //             npgsqlOptionsAction(options);
    //         }
    //         options.EnhanceDbContextOptionsBuilder();
    //     });
    // }
    // public static DbContextOptionsBuilder<TContext> UseNpgsqlWithAadAuth<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, DbConnection connection, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null) where TContext : DbContext
    // {
    //     return optionsBuilder.UseNpgsql<TContext>(connection, options =>
    //     {
    //         if (npgsqlOptionsAction != null)
    //         {
    //             npgsqlOptionsAction(options);
    //         }
    //         options.EnhanceDbContextOptionsBuilder();
    //     });
    // }
}
