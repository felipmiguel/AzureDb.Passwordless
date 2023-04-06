using AzureDb.Passwordless.Core;
using Npgsql;
using System.Threading.Tasks;
using System.Threading;
using System;
using Azure.Core;
using Azure.Identity;

namespace AzureDb.Passwordless.Postgresql
{
    /// <summary>
    /// Provides implementations for Npgsql delegates to get passwords that can be used with Azure Database for Postgresql
    /// Passwords provided are access tokens issued by Azure AD.
    /// </summary>
    public class AzureIdentityPostgresqlPasswordProvider : AzureIdentityBaseAuthenticationProvider
    {
        /// <summary>
        /// Default constructor. DefaultAzureCredential will be used to get AAD tokens as passwords.
        /// </summary>
        public AzureIdentityPostgresqlPasswordProvider() : base()
        {

        }

        /// <summary>
        /// Allow specify a managed identity by providing its client id. The managed identity will be used to get AAD tokens as passwords.
        /// This options allow select specific managed identity when the app hosting has more than one managed identity assigned
        /// </summary>
        /// <param name="clientId">client id of the managed identity to be used</param>
        public AzureIdentityPostgresqlPasswordProvider(string clientId): base(clientId)
        {
        }

        /// <summary>
        /// Token credential provided by the caller that will be used to retrieve AAD access tokens.
        /// </summary>
        /// <param name="credential">TokenCredential to use to retrieve AAD access tokens</param>
        public AzureIdentityPostgresqlPasswordProvider(TokenCredential credential):base(credential)
        { }

        /// <summary>
        /// Method that implements NpgsqlDbContextOptionsBuilder.ProvidePasswordCallback delegate signature.
        /// It can used in Entity Framework DbContext configuration
        /// </summary>
        /// <param name="host">Just part of the delegate signature. It is ignored</param>
        /// <param name="port">Just part of the delegate signature. It is ignored</param>
        /// <param name="database">Just part of the delegate signature. It is ignored</param>
        /// <param name="username">Just part of the delegate signature. It is ignored</param>
        /// <returns></returns>
        public string ProvidePasswordCallback(string host, int port, string database, string username)
        {
            return GetAuthenticationToken();
        }

        /// <summary>
        /// Method that implements NpgsqlDataSourceBuilder.UsePeriodicPasswordProvider delegate signature.
        /// <see href="https://www.npgsql.org/doc/security.html?tabs=tabid-1#auth-token-rotation-and-dynamic-password"/>
        /// </summary>
        /// <param name="settings">ConnectionString settings</param>
        /// <param name="cancellationToken">token to propagate cancellation</param>
        /// <returns>AAD issued access token that can be used as password for Azure Database for Postgresql</returns>
        public ValueTask<string> PeriodicPasswordProvider(NpgsqlConnectionStringBuilder settings, CancellationToken cancellationToken=default)
        {
            return GetAuthenticationTokenAsync(cancellationToken);
        }
    }
}
