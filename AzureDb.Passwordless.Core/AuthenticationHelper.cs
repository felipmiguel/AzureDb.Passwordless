using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.Core
{
    public class AuthenticationHelper
    {
        private const string OSSRDBMS_SCOPE = "https://ossrdbms-aad.database.windows.net/.default";
        private const string SYSTEM_KEY = "system";
        internal static readonly TokenRequestContext requestContext = new TokenRequestContext(new string[] { OSSRDBMS_SCOPE });

        private static ConcurrentDictionary<string, AccessToken> accessTokens = new ConcurrentDictionary<string, AccessToken>();


        private static DefaultAzureCredential GetCredentials(string clientId)
        {
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions
            {
                ExcludeVisualStudioCredential = true,
                ExcludeVisualStudioCodeCredential = true
            };
            if (!string.IsNullOrEmpty(clientId))
            {
                options.ManagedIdentityClientId = clientId;

            }
            return new DefaultAzureCredential(options);
        }

        public static string GetAccessToken(string clientId)
        {
            string key = GetKey(clientId);
            AccessToken currentAccessToken = accessTokens.GetOrAdd(key, (k) => GetCredentials(clientId).GetToken(requestContext));
            if (currentAccessToken.ExpiresOn > DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(30)))
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

        public static async ValueTask<string> GetAccessTokenAsync(string clientId, CancellationToken cancellationToken=default)
        {
            string key = GetKey(clientId);
            AccessToken currentAccessToken;
            if (!accessTokens.TryGetValue(key, out currentAccessToken))
            {
                currentAccessToken = await GetCredentials(clientId).GetTokenAsync(requestContext, cancellationToken);
                if (!accessTokens.TryAdd(key, currentAccessToken))
                {
                    accessTokens.TryGetValue(key, out currentAccessToken);
                }
            }
            return currentAccessToken.Token;
        }

        private static string GetKey(string clientId)
            => string.IsNullOrEmpty(clientId) ? SYSTEM_KEY : clientId;

    }
}
