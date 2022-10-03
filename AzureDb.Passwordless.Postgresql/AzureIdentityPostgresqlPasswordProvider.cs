using AzureDb.Passwordless.Core;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
