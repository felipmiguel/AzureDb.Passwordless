RESOURCE_GROUP=rg-mysql-passwordless
MYSQL_HOST=mysql-passwordless
DATABASE_NAME=checklist
DATABASE_FQDN=${MYSQL_HOST}.mysql.database.azure.com
LOCATION=eastus
MYSQL_ADMIN_USER=azureuser
# Generating a random password for the MySQL user as it is mandatory
# mysql admin won't be used as Azure AD authentication is leveraged also for administering the database
MYSQL_ADMIN_PASSWORD=$(pwgen -s 15 1)

# User Managed Identity name for MySQL AAD authentication
MYSQL_UMI_NAME="id-mysql-aad"

# Get current user logged in azure cli to make it mysql AAD admin
CURRENT_USER=$(az account show --query user.name -o tsv)
CURRENT_USER_OBJECTID=$(az ad user show --id $CURRENT_USER --query id -o tsv)

# Create a resource group
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION
# create mysql server
az mysql flexible-server create \
    --name $MYSQL_HOST \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $MYSQL_ADMIN_USER \
    --admin-password $MYSQL_ADMIN_PASSWORD \
    --public-access 0.0.0.0 \
    --tier Burstable \
    --sku-name Standard_B1ms

# create User Managed Identity for MySQL to be used for AAD authentication
az identity create -g $RESOURCE_GROUP -n $MYSQL_UMI_NAME

## assign the identity to the MySQL server
az mysql flexible-server identity assign \
    --server-name $MYSQL_HOST \
    --resource-group $RESOURCE_GROUP \
    --identity $MYSQL_UMI_NAME

# create mysql server AAD admin user
az mysql flexible-server ad-admin create \
    --server-name $MYSQL_HOST \
    --resource-group $RESOURCE_GROUP \
    --object-id $CURRENT_USER_OBJECTID \
    --display-name $CURRENT_USER \
    --identity $MYSQL_UMI_NAME

# Now the logged in user is the AAD admin for the MySQL server and can access it with AAD authentication
echo "User ${CURRENT_USER} is now the AAD admin for the MySQL server"

# create mysql database
az mysql flexible-server db create \
    -g $RESOURCE_GROUP \
    -s $MYSQL_HOST \
    -d $DATABASE_NAME

# Create a temporary firewall rule to allow connections from current machine to the mysql server
MY_IP=$(curl http://whatismyip.akamai.com)
az mysql flexible-server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --name $MYSQL_HOST \
    --rule-name AllowCurrentMachineToConnect \
    --start-ip-address ${MY_IP} \
    --end-ip-address ${MY_IP}
