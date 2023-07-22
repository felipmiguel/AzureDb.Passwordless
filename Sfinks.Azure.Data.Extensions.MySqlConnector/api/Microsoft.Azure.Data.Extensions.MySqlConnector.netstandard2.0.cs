namespace Sfinks.Azure.Data.Extensions.Common
{
    public partial class TokenCredentialBaseAuthenticationProvider
    {
        public TokenCredentialBaseAuthenticationProvider(Azure.Core.TokenCredential credential) { }
        public string GetAuthenticationToken(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public System.Threading.Tasks.ValueTask<string> GetAuthenticationTokenAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
}
namespace Sfinks.Azure.Data.Extensions.MySqlConnector
{
    public partial class TokenCredentialMysqlPasswordProvider : Sfinks.Azure.Data.Extensions.Common.TokenCredentialBaseAuthenticationProvider
    {
        public TokenCredentialMysqlPasswordProvider(Azure.Core.TokenCredential credential) : base (default(Azure.Core.TokenCredential)) { }
        public string ProvidePassword(MySqlConnector.MySqlProvidePasswordContext context) { throw null; }
        public System.Threading.Tasks.ValueTask<string> ProvidePasswordAsync(MySqlConnector.MySqlProvidePasswordContext context) { throw null; }
    }
}
