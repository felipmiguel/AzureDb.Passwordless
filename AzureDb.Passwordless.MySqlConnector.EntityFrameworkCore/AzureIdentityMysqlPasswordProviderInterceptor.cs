using Azure.Core;
using AzureDb.Passwordless.MySqlConnector;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;
using System.Data.Common;

namespace AzureDb.Passwordless.MySqlConnector.EntityFrameworkCore
{
    /// <summary>
    /// DBConnectionInterceptor that assigns ProvidePasswordCallback to MySqlConnection just before opening.
    /// </summary>
    internal class AzureIdentityMysqlPasswordProviderInterceptor : DbConnectionInterceptor
    {
        private readonly AzureIdentityMysqlPasswordProvider _passwordProvider;

        /// <summary>
        /// Constructor that allow specify the client id of the managed identity to be used. If no clientId is provided it will use DefaultAzureCredential with default behavior.
        /// </summary>
        /// <param name="clientId">Optional client id of the managed identity to be used</param>
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

        /// <summary>
        /// Constructor that allows specify TokenCredential to be used to retrieve AAD access tokens
        /// </summary>
        /// <param name="credential">TokenCredential to be used to retrieve AAD access tokens</param>
        public AzureIdentityMysqlPasswordProviderInterceptor(TokenCredential credential)
        {
            _passwordProvider = new AzureIdentityMysqlPasswordProvider(credential);
        }

        /// <summary>
        /// Method that is called synchronously just before the physical connection is opened
        /// </summary>
        /// <param name="connection">DbConnection. It should be of type MySqlConnection</param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            var mysqlConnection = (MySqlConnection)connection;
            mysqlConnection.ProvidePasswordCallback = _passwordProvider.ProvidePassword;
            return result;
        }

        /// <summary>
        /// Method that is called asynchronously just before the physical connection is opened
        /// </summary>
        /// <param name="connection">DbConnection. It should be of type MySqlConnection</param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            var mysqlConnection = (MySqlConnection)connection;
            mysqlConnection.ProvidePasswordCallback = _passwordProvider.ProvidePassword;
            return ValueTask.FromResult(result);
        }
    }
}