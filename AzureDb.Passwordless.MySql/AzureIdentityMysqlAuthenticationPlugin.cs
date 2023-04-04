using Azure.Core;
using Azure.Identity;
using AzureDb.Passwordless.Core;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace AzureDb.Passwordless.MySql
{
    public class AzureIdentityMysqlAuthenticationPlugin : MySqlAuthenticationPlugin
    {
        private const string PLUGIN_NAME = "mysql_clear_password";
        private const string CLIENTID_PROPERTY_NAME = "azure.clientId";
        public override string PluginName => PLUGIN_NAME;

        private AzureIdentityBaseAuthenticationPlugin _aadAuthenticationPlugin;
        private AzureIdentityBaseAuthenticationPlugin AadAuthenticationPlugin
        {
            get
            {
                if (_aadAuthenticationPlugin == null)
                {
                    _aadAuthenticationPlugin = string.IsNullOrEmpty(ClientId) 
                        ? new AzureIdentityBaseAuthenticationPlugin() 
                        : new AzureIdentityBaseAuthenticationPlugin(ClientId);
                }
                return _aadAuthenticationPlugin;
            }
        }


        protected override byte[] MoreData(byte[] data)
        {
            if (Settings.SslMode != MySqlSslMode.Disabled)
            {
                byte[] passBytes = System.Text.Encoding.UTF8.GetBytes(AadAuthenticationPlugin.GetAuthenticationToken());
                return passBytes;
            }
            else
            {
                throw new Exception("Method not supported");
            }
        }

        protected override void SetAuthData(byte[] data)
        {
            base.SetAuthData(data);
        }



        //public override object GetPassword()
        //{
        //    if (_authData)
        //    {
        //        return System.Text.Encoding.UTF8.GetBytes(GetAccessToken());
        //    }
        //    else
        //    {
        //        return base.GetPassword();
        //    }            
        //}

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


        public static void RegisterAuthenticationPlugin()
        {
            Type authenticationPluginManagerType = Type.GetType("MySql.Data.MySqlClient.Authentication.AuthenticationPluginManager, MySql.Data");
            FieldInfo pluginsField = authenticationPluginManagerType.GetField("Plugins", BindingFlags.Static | BindingFlags.NonPublic);
            IDictionary plugins = pluginsField.GetValue(null) as IDictionary;
            object clearTextPasswordPlugin = plugins["mysql_clear_password"];
            clearTextPasswordPlugin.GetType().GetField("Type").SetValue(clearTextPasswordPlugin, typeof(AzureIdentityMysqlAuthenticationPlugin).AssemblyQualifiedName);
            plugins["mysql_clear_password"] = clearTextPasswordPlugin;
        }

        public static string GetAuthenticationPlugin(string key)
        {
            Type authenticationPluginManagerType = Type.GetType("MySql.Data.MySqlClient.Authentication.AuthenticationPluginManager, MySql.Data");
            FieldInfo pluginsField = authenticationPluginManagerType.GetField("Plugins", BindingFlags.Static | BindingFlags.NonPublic);
            IDictionary plugins = pluginsField.GetValue(null) as IDictionary;
            object clearTextPasswordPlugin = plugins[key];
            return clearTextPasswordPlugin.GetType().GetField("Type").GetValue(clearTextPasswordPlugin) as string;
        }

    }
}
