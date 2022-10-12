using AzureDb.Passwordless.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDb.Passwordless.ConnectionTests
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
    }
}
