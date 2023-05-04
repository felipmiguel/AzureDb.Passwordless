using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Data.Extensions.Core;

namespace Microsoft.Azure.Data.Extensions.CoreTests
{
    [TestClass]
    public class AccessTokenTests
    {
        [TestMethod]
        public async Task CachingCreds()
        {
            TokenCredentialBaseAuthenticationProvider authenticationPlugin = new TokenCredentialBaseAuthenticationProvider(new DefaultAzureCredential());
            string[] tokens = await Task.WhenAll(authenticationPlugin.GetAuthenticationTokenAsync().AsTask(), authenticationPlugin.GetAuthenticationTokenAsync().AsTask());
            Assert.AreEqual(tokens[0], tokens[1]);
            string accessToken1 = await authenticationPlugin.GetAuthenticationTokenAsync();
            string accessToken2 = await authenticationPlugin.GetAuthenticationTokenAsync();
            Assert.AreEqual(accessToken1, accessToken2);
            Assert.AreEqual(accessToken1, tokens[1]);

            
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