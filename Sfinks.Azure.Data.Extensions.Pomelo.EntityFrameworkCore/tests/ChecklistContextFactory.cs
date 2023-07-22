using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Sample.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Sfinks.Azure.Data.Extensions.MySqlConnectorTests
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
                string connectionString = GetConnectionString(config);

                var serverVersion = ServerVersion.Parse("5.7", ServerType.MySql);
                options.UseMySql(connectionString, serverVersion,
                    optionsBuilder => optionsBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName))
                .UseAzureADAuthentication(new DefaultAzureCredential());
            });

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<ChecklistContext>();
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("DefaultConnection");
        }
    }
}
