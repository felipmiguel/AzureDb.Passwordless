using Azure.Core;
using AzureDb.Passwordless.Core;
using MySqlConnector;
using MySqlConnector.Authentication;
using System.Threading.Tasks;

namespace AzureDb.Passwordless.MySqlConnector
{
    /// <summary>
    /// Provides methods that implements MySqlConnection ProvidePasswordCallback. 
    /// <see href="https://mysqlconnector.net/api/mysqlconnector/mysqlconnection/providepasswordcallback/">MysqlConnection.ProvidePasswordCallback</see>
    /// </summary>
    public class AzureIdentityMysqlPasswordProvider : AzureIdentityBaseAuthenticationProvider
    {
        /// <summary>
        /// It will use <see href="https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential">DefaultAzureCredential</see> to get tokens to be used as password for MySQL in Azure
        /// </summary>
        public AzureIdentityMysqlPasswordProvider()
            : base() { }

        /// <summary>
        /// Specify the managed identity client id to be used to retrieve tokens to be used as password.
        /// </summary>
        /// <param name="clientId">client id of the managed identity to be used</param>
        public AzureIdentityMysqlPasswordProvider(string clientId)
            : base(clientId) { }

        /// <summary>
        /// Let the caller specify the TokenCredential to be used to retrieve access tokens.
        /// </summary>
        /// <param name="credential">TokenCredential to be used to retrieve access tokens</param>
        public AzureIdentityMysqlPasswordProvider(TokenCredential credential)
            : base(credential) { }

        /// <summary>
        /// ProvidePasswordCallback delegate implementation that can used to retrieve Azure Active Directory access tokens that can be used as password in Azure Database for MySql.
        /// <see href="https://mysqlconnector.net/api/mysqlconnector/mysqlconnection/providepasswordcallback/">MysqlConnection.ProvidePasswordCallback</see>
        /// </summary>
        /// <param name="context">Context information that could be used to retrieve the password. It is part of the delegate signature. This component does not use it</param>
        /// <returns>AAD access token that can be used as password to authentitcate with Azure Database for MySQL</returns>
        public string ProvidePassword(MySqlProvidePasswordContext context)
        {
            return GetAuthenticationToken();
        }

        /// <summary>
        /// If MySqlConnector provided async implementation for ProvidePasswordCallback delegate
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<string> ProvidePasswordAsync(MySqlProvidePasswordContext context)
        {
            return await GetAuthenticationTokenAsync();
        }
    }
}