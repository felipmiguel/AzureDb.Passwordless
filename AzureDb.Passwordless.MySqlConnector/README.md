# Authentication helper library for Azure Database for MySQL

Azure Database for MySQL accepts using an Azure AD issued access token as password. That access token should be issued for a specific audience `https://ossrdbms-aad.database.windows.net/.default`.
As described in [README](../README.md) there are some concepts to keep in mind, such as access token caching and connection pool fragementation.
This library provides some utilities to facilitate the connection to MySQL without worring for those implementation details.
For that purpose, this library provide extensions to [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/2.2.5?_src=template) library.

## MySqlConnector

MySqlConnector exposes a property in MySqlConnection to assign a delegate to obtain a password when a new connection is being created. See [MySqlConnection.ProvidePasswordCallback property](https://mysqlconnector.net/api/mysqlconnector/mysqlconnection/providepasswordcallback/) for details.

## AzureIdentityMySqlPasswordProvider

[AzureIdentityMySqlPasswordProvider](./AzureIdentityMySqlPasswordProvider.cs) provides the method `ProvidePasswordAsyncProvidePasswordAsync` that is compatible with [MySqlConnection.ProvidePasswordCallback](https://mysqlconnector.net/api/mysqlconnector/mysqlconnection/providepasswordcallback/).

AzureIdentityMySqlPasswordProvider can be configured in different ways to retrieve the access token:

* Using [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet). This component has a fallback mechanism trying to get an access token using different mechanisms. This is the default implementation.
* Specify an Azure Managed Identity. It uses DefaultAzureCredential, but tries to use a specific Managed Identity if the application hosting has more than one managed identity assigned.
* Specify a [TokenCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.core.tokencredential?view=azure-dotnet). It uses a TokenCredential provided by the caller to retrieve an access token.

### Sample default AzureIdentityMySqlPasswordProvider

This sample uses the default _AzureIdentityMySqlPasswordProvider_ constructor, that uses DefaultAzureCredential to obtain an access token. If you execute this sample in your local development environment it can take the credentials from environment variables, your IDE (Visual Studio, Visual Studio Code, IntelliJ) or Azure cli, see [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) for more details.

```csharp
AzureIdentityMySqlPasswordProvider passwordProvider = new AzureIdentityMySqlPasswordProvider();
using MySqlConnection connection = new MySqlConnection(GetConnectionString());
connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
await connection.OpenAsync();
```

### Sample using AzureIdentityMySqlPasswordProvider with a Managed Identity

This sample uses the _AzureIdentityMySqlPasswordProvider_ constructor with a managed identity. It is necessary to pass the managed identity client id, not the object id.

```csharp
string managedIdentityClientId = "00000000-0000-0000-0000-000000000000";
AzureIdentityMySqlPasswordProvider passwordProvider = new AzureIdentityMySqlPasswordProvider(managedIdentityClientId);
using MySqlConnection connection = new MySqlConnection(GetConnectionString());
connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
await connection.OpenAsync();
```

You can use the following command to retrieve the managed identity client id:

```bash
az identity show --resource-group ${RESOURCE_GROUP} --name ${MSI_NAME} --query clientId -o tsv
```

### Sample using AzureIdentityMySqlPasswordProvider using a TokenCredential

This sample uses the _AzureIdentityMySqlPasswordProvider_ constructor passing a TokenCredential. For simplicity this sample uses Azure cli credential

```csharp
AzureCliCredential credential = new AzureCliCredential();
AzureIdentityMySqlPasswordProvider passwordProvider = new AzureIdentityMySqlPasswordProvider(credential);
using MySqlConnection connection = new MySqlConnection(GetConnectionString());
connection.ProvidePasswordCallback = passwordProvider.ProvidePassword;
await connection.OpenAsync();
```