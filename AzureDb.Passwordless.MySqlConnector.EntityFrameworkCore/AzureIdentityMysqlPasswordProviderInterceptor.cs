using Azure.Core;
using AzureDb.Passwordless.MySqlConnector;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;
using System.Data.Common;

namespace AzureDb.Passwordless.MySqlConnector.EntityFrameworkCore
{
    internal class AzureIdentityMysqlPasswordProviderInterceptor : DbConnectionInterceptor
    {
        private readonly AzureIdentityMysqlPasswordProvider _passwordProvider;

        public AzureIdentityMysqlPasswordProviderInterceptor(string? clientId = null)
        {
            if (clientId == null) 
            {
                _passwordProvider = new AzureIdentityMysqlPasswordProvider();
            }
            else
            {
                _passwordProvider = new AzureIdentityMysqlPasswordProvider(clientId);
            }
        }

        public AzureIdentityMysqlPasswordProviderInterceptor(TokenCredential credential)
        {
            _passwordProvider = new AzureIdentityMysqlPasswordProvider(credential);
        }

        public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            var mysqlConnection = (MySqlConnection)connection;
            mysqlConnection.ProvidePasswordCallback = _passwordProvider.ProvidePassword;
            return result;
        }

        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            var mysqlConnection = (MySqlConnection)connection;
            mysqlConnection.ProvidePasswordCallback = _passwordProvider.ProvidePassword;
            return ValueTask.FromResult(result);
        }
    }
}