using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;

namespace Bili.Core.TestFramework
{
    /// <summary>
    /// Base test environment that can be extended to include more context properties
    /// </summary>
    public class TestEnvironment
    {
        private Lazy<TokenCredential> _credential = new Lazy<TokenCredential>(() => new DefaultAzureCredential(), true);
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
        /// Throws when variable is not found.
        /// </summary>
        protected string GetVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
