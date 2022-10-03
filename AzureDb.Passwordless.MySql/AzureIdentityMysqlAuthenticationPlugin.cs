using Azure.Core;
using Azure.Identity;
using AzureDb.Passwordless.Core;
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
        public override string PluginName => PLUGIN_NAME;

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

        private string ClientId
            => Settings.TryGetValue(CLIENTID_PROPERTY_NAME, out object clientId)
                ? clientId as string
                : string.Empty;



        public override object GetPassword()
        {
            return GetAccessToken();
        }

        private string GetAccessToken() => AuthenticationHelper.GetAccessToken(ClientId);

    }
}
