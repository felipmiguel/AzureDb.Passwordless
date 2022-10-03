using Azure.Core;
using Azure.Identity;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using System;
using System.Resources;

namespace AzureDb.Passwordless.MySql
{
    public class AzureIdentityMysqlAuthenticationPlugin : MySqlAuthenticationPlugin
    {
        private const string PLUGIN_NAME = "mysql_clear_password";
        private const string CLIENTID_PROPERTY_NAME = "azure.clientId";
        private const string OSSRDBMS_SCOPE = "https://ossrdbms-aad.database.windows.net/.default";
        private static readonly TokenRequestContext requestContext = new TokenRequestContext(new string[] { OSSRDBMS_SCOPE });
        // TODO: MAKE THIS CACHEABLE
        private AccessToken? currentAccessToken;
        public override string PluginName => PLUGIN_NAME;

        private DefaultAzureCredential credentials;

        private DefaultAzureCredential Credentials
        {
            get
            {
                if (credentials == null)
                {
                    if (this.Settings.TryGetValue(CLIENTID_PROPERTY_NAME, out object clientId))
                    {
                        credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                        {
                            ManagedIdentityClientId = (string)clientId,
                        });
                    }
                    else
                    {
                        credentials = new DefaultAzureCredential();
                    }
                }
                return credentials;
            }
        }

        private string GetAccessToken()
        {
            if (this.currentAccessToken == null
                || this.currentAccessToken?.ExpiresOn < DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds(30)))
            {
                currentAccessToken = Credentials.GetToken(requestContext);
            }
            return currentAccessToken?.Token;
        }

        protected override byte[] MoreData(byte[] data)
        {
            if ((Settings.SslMode != MySqlSslMode.Disabled && Settings.ConnectionProtocol != MySqlConnectionProtocol.UnixSocket)
                || (Settings.ConnectionProtocol == MySqlConnectionProtocol.UnixSocket))
            {
                return System.Text.Encoding.UTF8.GetBytes(GetAccessToken());
            }
            else
            {
                throw new Exception("Method not supported");
            }
        }



        public override object GetPassword()
        {
            return GetAccessToken();
        }


    }
}
