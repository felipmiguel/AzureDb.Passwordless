﻿
// Licensed under the MIT License.
using Azure.Core;
using Batec.Core.TestFramework;
using Azure.Identity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Batec.Azure.Data.Extensions.Common.Tests
{
    public class TokenCredentialBaseAuthenticationProviderTests : TestBase<ConfigurationTestEnvironment>
    {
        [Test]
        public async Task CachingCreds()
        {
            TokenCredentialBaseAuthenticationProvider authenticationPlugin = new TokenCredentialBaseAuthenticationProvider(TestEnvironment.Credential);
            string[] tokens = await Task.WhenAll(authenticationPlugin.GetAuthenticationTokenAsync().AsTask(), authenticationPlugin.GetAuthenticationTokenAsync().AsTask());
            Assert.AreEqual(tokens[0], tokens[1]);
            string accessToken1 = await authenticationPlugin.GetAuthenticationTokenAsync();
            string accessToken2 = await authenticationPlugin.GetAuthenticationTokenAsync();
            Assert.AreEqual(accessToken1, accessToken2);
            Assert.AreEqual(accessToken1, tokens[1]);
        }
    }
}
