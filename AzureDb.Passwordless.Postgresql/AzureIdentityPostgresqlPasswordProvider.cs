using AzureDb.Passwordless.Core;
using Npgsql;
using System.Threading.Tasks;
using System.Threading;
using System;
using Azure.Core;
using Azure.Identity;

namespace AzureDb.Passwordless.Postgresql
{
    public class AzureIdentityPostgresqlPasswordProvider : AzureIdentityBaseAuthenticationPlugin
    {
        public AzureIdentityPostgresqlPasswordProvider() : base()
        {

        }

        public AzureIdentityPostgresqlPasswordProvider(string clientId): base(clientId)
        {
        }

        public AzureIdentityPostgresqlPasswordProvider(TokenCredential credential):base(credential)
        { }

        public string ProvidePasswordCallback(string host, int port, string database, string username)
        {
            return GetAuthenticationToken();
            //return AuthenticationHelper.GetAccessToken(clientId);
        }

        public ValueTask<string> PeriodicPasswordProvider(NpgsqlConnectionStringBuilder settings, CancellationToken cancellationToken=default)
        {
            return GetAuthenticationTokenAsync(cancellationToken);
        }
    }
}
