using AzureDb.Passwordless.MySql;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);
//builder.Host.ConfigureAppConfiguration(options =>
//{ 
//    options.AddInMemoryCollection("MySQL.AuthenticationPlugins")
//});

//System.Configuration.ConfigurationManager.s = new MySql.Data.MySqlClient.MySqlConfiguration
//{
    
//};

// Add services to the container.
//builder.Services.Configure<MySqlConfiguration>(mySqlConfig =>
//{
//    mySqlConfig.AuthenticationPlugins.Append(new AuthenticationPluginConfigurationElement { Name = "mysql_clear_password2", Type = typeof(AzureIdentityMysqlAuthenticationPlugin).FullName });
//});

//System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
//MySqlConfiguration mySQLSection = new MySqlConfiguration();

//System.Configuration.ConfigurationSection sec = new System.Configuration;
//sec[""]
//config.Sections.Add("MySQL", mySQLSection);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
