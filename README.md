# AzureDb.Passwordless

This repository contains helper libraries that can be used to connect to Azure Database for Postgresql and Mysql using Azure AD authentication. Many Azure services support Azure AD authentication, they require an Azure AD access token with specific scopes. Azure Database for Postgres and Azure Database for MySql expect an Azure AD access token with <https://ossrdbms-aad.database.windows.net> audience. It is possible to get an access of this kind using Azure.Identity library or even using Azure CLI.

This repository provides a core library that gets access tokens for Azure Database for Postgresql and MySql and also caches them while they are not expired.

## Postgresql

[Npgsql](https://www.npgsql.org/doc/security.html?tabs=tabid-1) library offers a mechanism to obtain the password each time there is a physical connection to the database. That is _ProvidePasswordCallback_ method. The library AzureDb.Passwordless.Postgresql provides a class that implements the delegate signature and obtains the password from Azure AD.

The connection using this library looks like this:

```csharp
AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider();
using NpgsqlConnection connection = new NpgsqlConnection
{
    ConnectionString = connectionStringBuilder.ConnectionString,
    ProvidePasswordCallback = passwordProvider.ProvidePasswordCallback
};
connection.Open();
/* Do something with the connection */
```

If the connection string contains _Password_ this callback will be ignored.

The library uses Azure.Identity, which tries to use different authentication mechanisms to get the access token. It includes Managed Identity, Visual Studio, Azure CLI, and others. In Azure workloads a hosting environment may have more than one Managed Identity assigned, for instance when using User Assigned Managed Identity. In that case it can be necessary to specify which Managed Identity to use. This can be done by setting the clientId attribute to the AzureIdentityPostgresqlPasswordProvider constructor.

```csharp
string managedIdentityClientId= "00000000-0000-0000-0000-000000000000";
AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider(managedIdentityClientId);
using NpgsqlConnection connection = new NpgsqlConnection
{
    ConnectionString = connectionStringBuilder.ConnectionString,
    ProvidePasswordCallback = passwordProvider.ProvidePasswordCallback
};
connection.Open();
/* Do something with the connection */
```

### Entity Framework Core

Npgsql Entity Framework Core provider supports the same mechanism to obtain the password. In this case in DbContextOptionsBuilderOptions.ProvidePasswordCallback. Then it will look like this:

For DbContext factories:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContextFactory<MyContext>(options =>
    {        
        AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider();
        string connectionString = "PSQL CONNECTION STRING";
        options.UseNpgsql(connectionString, npgopts =>
        {
            npgopts.ProvidePasswordCallback(passwordProvider.ProvidePasswordCallback);
        });
    });
}
```

For DbContext:

```csharp
builder.Services.AddDbContext<MyContext>(options =>
{
    AzureIdentityPostgresqlPasswordProvider passwordProvider = new AzureIdentityPostgresqlPasswordProvider();
    string connectionString = "PSQL CONNECTION STRING";
    options.UseNpgsql(connectionString, npgopts =>
    {
        npgopts.ProvidePasswordCallback(passwordProvider.ProvidePasswordCallback);
    });
});
```

## MySql

MySql allows to configure a custom [authentication plugin](https://dev.mysql.com/doc/connector-net/en/connector-net-programming-authentication-user-plugin.html). It should derive from `MySql.Data.MySqlClient.Authentication.MySqlAuthenticationPlugin`. Active Directory authentication is very similar to Clear Text authentication. For that reason, the [AzureIdentityMysqlAuthenticationPlugin](AzureDb.Passwordless.MySql/AzureIdentityMysqlAuthenticationPlugin.cs) implementation in this repository is based on the [ClearPasswordPlugin](https://github.com/mysql/mysql-connector-net/blob/8.0/MySQL.Data/src/Authentication/ClearPasswordPlugin.cs) from MySql.Data library.

To override the default client/server negotiation of the plugin to use, it is necessary to specify _DefaultAuthenticationPlugin_ in the connection string.

```bash
server=myserver.mysql.database.azure.com;user id=myuser@myserver;database=mydb;sslmode=Required;defaultauthenticationplugin=mysql_clear_password;
```

To configure the authentication plugin it is necessary to enable the plugin in the configuration file:

```xml
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="MySQL" type="MySql.Data.MySqlClient.MySqlConfiguration,
MySql.Data"/>
  </configSections>
  <MySQL>
    <AuthenticationPlugins>
      <add name="mysql_native_password"
type="AzureDb.Passwordless.MySql.AzureIdentityMysqlAuthenticationPlugin, AzureDb.Passwordless.MySql"></add>
    </AuthenticationPlugins>  
  </MySQL>
...
</configuration>
```

This configuration implementation has a major issue. It is based on legacy System.Configuration implementation, and most of the .NET Core applications do not use it. The configuration file is not read by default and it can mess the implementation of the application.

### Experimental implementation

I created an [experimental implementation](https://github.com/felipmiguel/mysql-connector-net) that can configure the authentication plugin using the connection string. In this case, the connection string should contain the _AuthenticationPlugins_ parameter, with a list of the plugins to use. Then the connection string looks like this:

```bash
server=myserver.mysql.database.azure.com;user id=myuser@myserver;database=mydb;sslmode=Required;defaultauthenticationplugin=mysql_clear_password;authenticationplugins=mysql_clear_password:AzureDb.Passwordless.MySql.AzureIdentityMysqlAuthenticationPlugin# AzureDb.Passwordless.MySql# Version=1.0.0.0# Culture=neutral# PublicKeyToken=null;
```

> Note that the plugin list replaced character , with character #. This is because the attribute is an array it can be confused with item separator.

If you want to use this connector, you can add the [following package(s)](https://github.com/felipmiguel?tab=packages&repo_name=mysql-connector-net) to your project:

For MySql connections:

```dotnetcli
dotnet add PROJECT package MySql.Data --version 8.0.30.1
```

And for MySql Entity Framework Core provider:

```dotnetcli
dotnet add PROJECT package MySql.EntityFrameworkCore --version 6.0.4
```

## Nuget packages

This repository contains the following Nuget packages:

* [AzureDb.Passwordless.Postgresql](https://github.com/felipmiguel/AzureDb.Passwordless/packages/1669347)
* [AzureDb.Passwordless.MySql](https://github.com/felipmiguel/AzureDb.Passwordless/packages/1669348). This package references [MySql.Data experimental implementation](https://github.com/felipmiguel?tab=packages&repo_name=mysql-connector-net).
* [AzureDb.Passwordless.Core](https://github.com/felipmiguel/AzureDb.Passwordless/packages/1669407). This package is referenced by the other two packages.

If you want to use above packages you should add the nuget feed to your project:

```bash
dotnet nuget add source --username [YOUR GITHUB USERID] --password [YOUR PAT] --store-password-in-clear-text --name github "https://nuget.pkg.github.com/felipmiguel/index.json"
```

You PAT should include the following scope `_read:packages_`.

## Reference links

* Postgresql: <https://www.npgsql.org/doc/security.html?tabs=tabid-1>
* MySql: <https://dev.mysql.com/doc/connector-net/en/connector-net-programming-authentication-user-plugin.html>
