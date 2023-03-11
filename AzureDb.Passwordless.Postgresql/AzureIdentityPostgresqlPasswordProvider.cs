using AzureDb.Passwordless.Core;
using Npgsql;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace AzureDb.Passwordless.Postgresql
{
    public class AzureIdentityPostgresqlPasswordProvider
    {
        private readonly string clientId;

        public AzureIdentityPostgresqlPasswordProvider() : this(null)
        {

        }

        public AzureIdentityPostgresqlPasswordProvider(string clientId)
        {
            this.clientId = clientId;
        }
        public string ProvidePasswordCallback(string host, int port, string database, string username)
        {
            return AuthenticationHelper.GetAccessToken(clientId);
        }

        public ValueTask<string> PeriodicPasswordProvider(NpgsqlConnectionStringBuilder settings, CancellationToken cancellationToken=default)
        {
            return AuthenticationHelper.GetAccessTokenAsync(clientId, cancellationToken);
        }
    }
}
