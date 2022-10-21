using Azure.Core;
using Azure.Identity;
using AzureDb.Passwordless.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDb.Passwordless.MysqlTests
{
    [TestClass]
    public class AccessTokenTests
    {
        [TestMethod]
        public void CachingCreds()
        {
            string accessToken1 = AuthenticationHelper.GetAccessToken(null);
            string accessToken2 = AuthenticationHelper.GetAccessToken(null);
            Assert.AreEqual(accessToken1, accessToken2);
        }

        [TestMethod]
        public async Task NoCachingCreds()
        {
            const string OSSRDBMS_SCOPE = "https://ossrdbms-aad.database.windows.net/.default";
            TokenRequestContext requestContext = new TokenRequestContext(new string[] { OSSRDBMS_SCOPE });
            DefaultAzureCredential credential = new DefaultAzureCredential();
            var token1 = await credential.GetTokenAsync(requestContext);

            var token2 = await credential.GetTokenAsync(requestContext);
            Assert.AreEqual(token1.Token, token2.Token);
        }
    }
}
