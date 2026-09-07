using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using NUnit.Framework;

namespace Batec.Core.TestFramework
{
    /// <summary>
    /// Base test environment that can be extended to include more context properties
    /// </summary>
    public class TestEnvironment
    {
        private Lazy<TokenCredential> _credential = new Lazy<TokenCredential>(() =>
            new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = true, ExcludeAzurePowerShellCredential = true, ExcludeVisualStudioCodeCredential = true, ExcludeInteractiveBrowserCredential = true, ExcludeVisualStudioCredential = true }),
            true);

        /// <summary>
        /// TokenCredential of the test
        /// </summary>
        public TokenCredential Credential => _credential.Value;

        internal Task WaitForEnvironmentAsync()
        {
            return Task.CompletedTask;
        }

        internal Task WaitForEnvironmentShutdown()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns an environment variable value.
        /// Skips integration tests when variable is not found.
        /// </summary>
        protected string GetVariable(string name)
        {
            string variable = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(variable))
            {
                Assert.Ignore($"Skipping integration test because required environment variable '{name}' is missing.");
            }

            return variable;
        }
    }
}
