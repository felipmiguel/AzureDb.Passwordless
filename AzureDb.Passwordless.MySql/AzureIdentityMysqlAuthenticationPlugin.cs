using Azure.Core;
using Azure.Identity;
using AzureDb.Passwordless.Core;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace AzureDb.Passwordless.MySql
{
    public class AzureIdentityMysqlAuthenticationPlugin : MySqlAuthenticationPlugin
    {
        private const string PLUGIN_NAME = "mysql_clear_password";
        private const string CLIENTID_PROPERTY_NAME = "azure.clientId";
        public override string PluginName => PLUGIN_NAME;


        //protected override byte[] MoreData(byte[] data)
        //{
        //    if (Settings.SslMode != MySqlSslMode.Disabled)
        //    {
        //        byte[] passBytes = System.Text.Encoding.UTF8.GetBytes(GetAccessToken());
        //        return passBytes;
        //    }
        //    else
        //    {
        //        throw new Exception("Method not supported");
        //    }
        //}

        public override object GetPassword()
        {
            return System.Text.Encoding.UTF8.GetBytes(GetAccessToken());
        }

        private string ClientId
        {
            get
            {
                if (Settings != null && Settings.TryGetValue(CLIENTID_PROPERTY_NAME, out object clientId))
                {
                    return clientId as string;
                }
                return string.Empty;
            }
        }



        //public override object GetPassword()
        //{
        //    return GetAccessToken();
        //}

        private string GetAccessToken()
        {
            return AuthenticationHelper.GetAccessToken(ClientId);
        }

    }
}
