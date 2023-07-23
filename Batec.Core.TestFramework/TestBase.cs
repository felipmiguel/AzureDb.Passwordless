using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Batec.Core.TestFramework
{
    /// <summary>
    /// Base test that executes tests using Azure resources
    /// </summary>
    /// <typeparam name="TEnvironment"></typeparam>
    public class TestBase<TEnvironment> where TEnvironment : TestEnvironment, new()
    {
        protected TestBase()
        {
            TestEnvironment = new TEnvironment();
        }
        /// <summary>
        /// Facilities to access to the test environment
        /// </summary>
        protected TEnvironment TestEnvironment { get; }

        /// <summary>
        /// Initialize the environment and waits until it is ready to execute the tests
        /// </summary>
        /// <returns></returns>
        [OneTimeSetUp]
        public async ValueTask WaitForEnvironment()
        {
            await TestEnvironment.WaitForEnvironmentAsync();
        }

        /// <summary>
        /// Destroys the environment and waits until it is completely done
        /// </summary>
        /// <returns></returns>
        [OneTimeTearDown]
        public async ValueTask WaitForEnvironmentShutdown()
        {
            await TestEnvironment.WaitForEnvironmentShutdown();
        }
    }
}
