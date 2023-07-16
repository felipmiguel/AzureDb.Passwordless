using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;

namespace Bili.Core.TestFramework
{
    /// <summary>
    /// Base test environment that can be extended to include more context properties
    /// </summary>
    public class TestEnvironment
    {
        /// <summary>
        /// TokenCredential of the test
        /// </summary>
        public TokenCredential Credential => null;

        internal Task WaitForEnvironmentAsync()
        {
            throw new NotImplementedException();
        }

        internal Task WaitForEnvironmentShutdown()
        {
            throw new NotImplementedException();
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
