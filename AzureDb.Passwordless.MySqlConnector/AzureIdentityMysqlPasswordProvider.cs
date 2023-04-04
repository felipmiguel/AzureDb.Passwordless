using Azure.Core;
using AzureDb.Passwordless.Core;
using MySqlConnector;
using MySqlConnector.Authentication;

namespace AzureDb.Passwordless.MySqlConnector
{
    public class AzureIdentityMysqlPasswordProvider : AzureIdentityBaseAuthenticationPlugin
    {

        public AzureIdentityMysqlPasswordProvider()
            : base() { }

        public AzureIdentityMysqlPasswordProvider(string clientId)
            : base(clientId) { }

        public AzureIdentityMysqlPasswordProvider(TokenCredential credential)
            : base(credential) { }

        public string ProvidePassword(MySqlProvidePasswordContext context)
        {
            return GetAuthenticationToken();
        }

        public async ValueTask<string> ProvidePasswordAsync(MySqlProvidePasswordContext context)
        {
            return await GetAuthenticationTokenAsync();
        }
    }
}