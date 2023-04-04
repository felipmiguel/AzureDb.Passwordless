using Azure;
using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.Core
{
    public static class DatabaseAuthenticationExtensions
    {
        static ConcurrentDictionary<TokenCredential, AccessToken> tokenCache = new ConcurrentDictionary<TokenCredential, AccessToken>();
        public static async ValueTask<string> GetOssDatabaseAuthenticationTokenAsync(this TokenCredential credential, CancellationToken cancellationToken = default)
        {
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }
            if (tokenCache.TryGetValue(credential, out AccessToken token))
            {
                if (token.ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(25))
                {
                    return token.Token;
                }
                else
                {
                    AccessToken newToken = await credential.GetTokenAsync(AuthenticationHelper.requestContext, cancellationToken);
                    tokenCache.TryUpdate(credential, newToken, token);
                    return newToken.Token;
                }
            }
            else
            {
                token = await credential.GetTokenAsync(AuthenticationHelper.requestContext, cancellationToken);
                tokenCache.TryAdd(credential, token);
                return token.Token;
            }
        }

        public static string GetOssDatabaseAuthenticationToken(this TokenCredential credential, CancellationToken cancellationToken = default)
        {
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }
            AccessToken currentAccessToken = tokenCache.GetOrAdd(credential, (c) => c.GetToken(AuthenticationHelper.requestContext, cancellationToken));
            if (currentAccessToken.ExpiresOn > DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30)))
            {
                return currentAccessToken.Token;
            }
            else
            {
                var newToken = credential.GetToken(AuthenticationHelper.requestContext, cancellationToken);
                tokenCache.TryUpdate(credential, newToken, currentAccessToken);
                return newToken.Token;
            }
        }
    }
}
