using Microsoft.Azure.Data.Extensions.MySql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Data.Extensions.MySqlTests
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void ForceAuthenticationPluginByReflection()
        {
            Type? authenticationPluginManagerType = Type.GetType("MySql.Data.MySqlClient.Authentication.AuthenticationPluginManager, MySql.Data");
            FieldInfo? pluginsField = authenticationPluginManagerType?.GetField("Plugins", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(pluginsField);
            IDictionary? plugins = pluginsField?.GetValue(null) as IDictionary;
            object? clearTextPasswordPlugin = plugins?["mysql_clear_password"];
            clearTextPasswordPlugin?.GetType().GetField("Type")?.SetValue(clearTextPasswordPlugin, typeof(AzureIdentityMysqlAuthenticationPlugin).AssemblyQualifiedName);
        }
    }
}
