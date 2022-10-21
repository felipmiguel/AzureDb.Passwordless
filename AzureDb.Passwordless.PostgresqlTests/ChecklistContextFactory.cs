using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sample.Repository;
using System.Reflection;

namespace AzureDb.Passwordless.PostgresqlTests
{
    public class ChecklistContextFactory : IDesignTimeDbContextFactory<ChecklistContext>
    {
        public ChecklistContext CreateDbContext(string[] args)
        {
            Console.WriteLine("args {0}", string.Join(",", args));

            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = configBuilder.Build();

            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<ChecklistContext>(options =>
            {
                options.UseNpgsql(GetPGConnString(config), optionsBuilder =>
                optionsBuilder
                    .MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
                    .UseAadAuthentication());
            });

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<ChecklistContext>();
        }

        private string GetPGConnString(IConfiguration configuration)
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = configuration.GetSection("postgresql:host").Value,
                Database = configuration.GetSection("postgresql:database").Value,
                Username = configuration.GetSection("postgresql:user").Value,
                Port = 5432,
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                Timeout = 30,
            };
            return connectionStringBuilder.ConnectionString;
        }
    }
}
