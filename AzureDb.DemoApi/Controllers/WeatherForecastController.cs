using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AzureDb.DemoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        private string ConnectionString
        {
            get
            {
                MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
                connectionStringBuilder["authenticationPlugins"] = "AzureDb.Passwordless.MySql.AzureIdentityMysqlAuthenticationPlugin";
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

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var s = configuration.GetSection("MySQL");
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await connection.CloseAsync();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}