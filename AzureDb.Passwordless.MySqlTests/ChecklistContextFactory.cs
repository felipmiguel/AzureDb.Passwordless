using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Sample.Repository;
using System.Reflection;

namespace AzureDb.Passwordless.MysqlTests
{
    public class ChecklistContextFactory : IDesignTimeDbContextFactory<ChecklistContext>
    {
        public ChecklistContext CreateDbContext(string[] args)
        {
            Console.WriteLine("args {0}", string.Join(",", args));

            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = configBuilder.Build();

            Type t = typeof(Microsoft.EntityFrameworkCore.Storage.TypeMappingSourceDependencies);
            Console.WriteLine("TypeMappingSourceDependencies {0}", t.AssemblyQualifiedName);
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<ChecklistContext>(options =>
            {
                options.UseMySQL(GetMySqlConnString(config), options =>
                options
                    .MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
                    .UseAadAuthentication());
            });

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<ChecklistContext>();
        }

        private string GetMySqlConnString(IConfiguration configuration)
        {
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = configuration.GetSection("mySqlInfo:host").Value,
                UserID = configuration.GetSection("mySqlInfo:user").Value,
                Database = configuration.GetSection("mySqlInfo:database").Value,
                SslMode = MySqlSslMode.Required,
                DefaultAuthenticationPlugin = "mysql_clear_password",
                ConnectionTimeout = 120
            };
            return connectionStringBuilder.ConnectionString;
        }
    }
}
