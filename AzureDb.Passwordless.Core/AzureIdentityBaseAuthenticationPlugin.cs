using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.Core
{
    public class AzureIdentityBaseAuthenticationPlugin
    {
        private const string OSSRDBMS_SCOPE = "https://ossrdbms-aad.database.windows.net/.default";        
        private static readonly TokenRequestContext requestContext = new TokenRequestContext(new string[] { OSSRDBMS_SCOPE });

        private readonly TokenCredential credential;
        private AccessToken? accessToken;

        public AzureIdentityBaseAuthenticationPlugin()
            : this(new DefaultAzureCredential())
        { }

        public AzureIdentityBaseAuthenticationPlugin(string clientId)
            : this(new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId }))
        { }

        public AzureIdentityBaseAuthenticationPlugin(TokenCredential credential)
        {
            this.credential = credential;
        }

        public async ValueTask<string> GetAuthenticationTokenAsync(CancellationToken cancellationToken = default)
        {
            if (accessToken?.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(20))
            {
                return accessToken?.Token;
            }
            else
            {
                accessToken = await credential.GetTokenAsync(requestContext, cancellationToken);
                return accessToken?.Token;
            }
        }

        public string GetAuthenticationToken(CancellationToken cancellationToken = default)
        {
            if (accessToken?.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(20))
            {
                return accessToken?.Token;
            }
            else
            {
                accessToken = credential.GetToken(requestContext, cancellationToken);
                return accessToken?.Token;
            }
        }
    }
}
