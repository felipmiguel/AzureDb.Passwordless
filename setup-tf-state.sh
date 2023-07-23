group_name=rg-Batec-tfstate
location=westeurope
account_name=Batecstate
az group create --name $group_name --location $location
az storage account create --name $account_name --resource-group $group_name --location $location --sku Standard_LRS
az storage container create --name tfstate-mysql --account-name $account_name
az storage container create --name tfstate-pgsql --account-name $account_name