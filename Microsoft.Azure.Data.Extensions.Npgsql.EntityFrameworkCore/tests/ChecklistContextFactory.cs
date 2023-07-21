using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sample.Repository;
using System.Reflection;

namespace Microsoft.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.Tests
{
    public class ChecklistContextFactory : IDesignTimeDbContextFactory<ChecklistContext>
    {
        public ChecklistContext CreateDbContext(string[] args)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = configBuilder.Build();

            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<ChecklistContext>(options =>
            {
                options.UseNpgsql(GetConnectionString(config), optionsBuilder =>
                optionsBuilder
                    .MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
                    .UseAzureADAuthentication(new DefaultAzureCredential()));
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