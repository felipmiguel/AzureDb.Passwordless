using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Concurrent;

namespace AzureDb.Passwordless.Core
{
    public class AuthenticationHelper
    {
        private const string OSSRDBMS_SCOPE = "https://ossrdbms-aad.database.windows.net/.default";
        private const string SYSTEM_KEY = "system";
        private static readonly TokenRequestContext requestContext = new TokenRequestContext(new string[] { OSSRDBMS_SCOPE });

        private static ConcurrentDictionary<string, AccessToken> accessTokens = new ConcurrentDictionary<string, AccessToken>();


        private static DefaultAzureCredential GetCredentials(string clientId)
        {

            if (string.IsNullOrEmpty(clientId))
            {
                return new DefaultAzureCredential();
            }
            else
            {
                return new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = (string)clientId,
                });
            }
        }

        public static string GetAccessToken(string clientId)
        {
            string key = GetKey(clientId);
            AccessToken currentAccessToken = accessTokens.GetOrAdd(key, GetCredentials(clientId).GetToken(requestContext));
            if (currentAccessToken.ExpiresOn < DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds(30)))
            {
                return currentAccessToken.Token;
            }
            else
            {
                var newToken = GetCredentials(clientId).GetToken(requestContext);
                accessTokens.TryUpdate(key, newToken, currentAccessToken);
                return newToken.Token;
            }
        }

        private static string GetKey(string clientId)
            => string.IsNullOrEmpty(clientId) ? clientId : SYSTEM_KEY;

    }
}
