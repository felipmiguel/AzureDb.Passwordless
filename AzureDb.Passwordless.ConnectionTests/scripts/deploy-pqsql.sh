RESOURCE_GROUP=rg-postgres-passwordless
POSTGRESQL_HOST=postgres-passwordless
DATABASE_NAME=checklist
DATABASE_FQDN=${POSTGRESQL_HOST}.postgres.database.azure.com
LOCATION=eastus
POSTGRESQL_ADMIN_USER=azureuser
# Generating a random password for Posgresql admin user as it is mandatory
# postgresql admin won't be used as Azure AD authentication is leveraged also for administering the database
POSTGRESQL_ADMIN_PASSWORD=$(pwgen -s 15 1)

# Get current user logged in azure cli to make it postgresql AAD admin
CURRENT_USER=$(az account show --query user.name -o tsv)
CURRENT_USER_OBJECTID=$(az ad user show --id $CURRENT_USER --query id -o tsv)

az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION
# create postgresql server
az postgres server create \
    --name $POSTGRESQL_HOST \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $POSTGRESQL_ADMIN_USER \
    --admin-password "$POSTGRESQL_ADMIN_PASSWORD" \
    --public 0.0.0.0 \
    --sku-name GP_Gen5_2 \
    --version 11 \
    --storage-size 5120

# create postgres database
az postgres db create \
    -g $RESOURCE_GROUP \
    -s $POSTGRESQL_HOST \
    -n $DATABASE_NAME

# create postgresql server AAD admin user
az postgres server ad-admin create \
    --server-name $POSTGRESQL_HOST \
    --resource-group $RESOURCE_GROUP \
    --object-id $CURRENT_USER_OBJECTID \
    --display-name $CURRENT_USER

# Create a temporary firewall rule to allow connections from current machine to the postgresql server
MY_IP=$(curl http://whatismyip.akamai.com)
az postgres server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server-name $POSTGRESQL_HOST \
    --name AllowCurrentMachineToConnect \
    --start-ip-address ${MY_IP} \
    --end-ip-address ${MY_IP}
