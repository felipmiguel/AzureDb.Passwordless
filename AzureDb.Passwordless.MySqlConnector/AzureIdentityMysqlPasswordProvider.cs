using AzureDb.Passwordless.Core;
using MySqlConnector;
using MySqlConnector.Authentication;

namespace AzureDb.Passwordless.MySqlConnector
{
    public class AzureIdentityMysqlPasswordProvider
    {
        private readonly string? clientId;

        public AzureIdentityMysqlPasswordProvider() : this(null)
        {
        }

        public AzureIdentityMysqlPasswordProvider(string? clientId)
        {
            this.clientId = clientId;
        }
        public string ProvidePassword(MySqlProvidePasswordContext context)
        {
            return AuthenticationHelper.GetAccessToken(clientId);
        }
    }
}