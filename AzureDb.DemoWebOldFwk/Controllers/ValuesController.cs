using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;
using MySql.Data.MySqlClient;

namespace AzureDb.DemoWebOldFwk.Controllers
{
    public class ValuesController : ApiController
    {
        private string ConnectionString
        {
            get
            {
                MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
                //connectionStringBuilder["authenticationPlugins"] = "AzureDb.Passwordless.MySql.AzureIdentityMysqlAuthenticationPlugin";
                connectionStringBuilder.DefaultAuthenticationPlugin = "mysql_clear_password";
                //connectionStringBuilder[""]
                connectionStringBuilder.Server = "mysql-weblogic-passwordless.mysql.database.azure.com";
                connectionStringBuilder.UserID = "fmiguel@microsoft.com";
                connectionStringBuilder.Database = "checklist";
                connectionStringBuilder.SslMode = MySqlSslMode.Required;
                connectionStringBuilder.SslCa = "DigiCertGlobalRootCA.crt.pem";
                return connectionStringBuilder.GetConnectionString(false);

            }
        }

        // GET api/values
        public async Task<IEnumerable<string>> Get()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await connection.CloseAsync();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
