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
    /// <summary>
    /// Authentication plugin that can be used to customize the authentication to MySql. See https://dev.mysql.com/doc/connector-net/en/connector-net-programming-authentication-user-plugin.html
    /// OAuth based authentication plugins are managed as mysql_clear_password plugins.
    /// IMPORTANT: This component is considered Experimental as it uses a non-recommended approach to register the authentication plugin in MySQL.Data.
    /// </summary>
    public class AzureIdentityMysqlAuthenticationPlugin : MySqlAuthenticationPlugin
    {
        private const string PLUGIN_NAME = "mysql_clear_password";
        private const string CLIENTID_PROPERTY_NAME = "azure.clientId";
        public override string PluginName => PLUGIN_NAME;

        private TokenCredentialBaseAuthenticationProvider _aadAuthenticationPlugin;
        private TokenCredentialBaseAuthenticationProvider AadAuthenticationPlugin
        {
            get
            {
                if (_aadAuthenticationPlugin == null)
                {
                    _aadAuthenticationPlugin = string.IsNullOrEmpty(ClientId) 
                        ? new TokenCredentialBaseAuthenticationProvider(new DefaultAzureCredential()) 
                        : new TokenCredentialBaseAuthenticationProvider(new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = ClientId }));
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

        /// <summary>
        /// Registers this class as the implementation for mysql_clear_password.
        /// It workarounds the fact that MySql.Data uses old System.Configuration libraries that no longer work on dotnet core.
        /// </summary>
        public static void RegisterAuthenticationPlugin()
        {
            Type authenticationPluginManagerType = Type.GetType("MySql.Data.MySqlClient.Authentication.AuthenticationPluginManager, MySql.Data");
            FieldInfo pluginsField = authenticationPluginManagerType.GetField("Plugins", BindingFlags.Static | BindingFlags.NonPublic);
            IDictionary plugins = pluginsField.GetValue(null) as IDictionary;
            object clearTextPasswordPlugin = plugins["mysql_clear_password"];
            clearTextPasswordPlugin.GetType().GetField("Type").SetValue(clearTextPasswordPlugin, typeof(AzureIdentityMysqlAuthenticationPlugin).AssemblyQualifiedName);
            plugins["mysql_clear_password"] = clearTextPasswordPlugin;
        }

        /// <summary>
        /// Gets current authentication plugin implementation type for a given plugin identifier.
        /// </summary>
        /// <param name="pluginId">Plugin identifier, for instance mysql_clear_password</param>
        /// <returns>Implementation type qualified name</returns>
        public static string GetAuthenticationPlugin(string pluginId)
        {
            Type authenticationPluginManagerType = Type.GetType("MySql.Data.MySqlClient.Authentication.AuthenticationPluginManager, MySql.Data");
            FieldInfo pluginsField = authenticationPluginManagerType.GetField("Plugins", BindingFlags.Static | BindingFlags.NonPublic);
            IDictionary plugins = pluginsField.GetValue(null) as IDictionary;
            object clearTextPasswordPlugin = plugins[pluginId];
            return clearTextPasswordPlugin.GetType().GetField("Type").GetValue(clearTextPasswordPlugin) as string;
        }

    }
}
