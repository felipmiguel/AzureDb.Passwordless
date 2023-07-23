location=westeurope
resource_group=rg-batec-stage
tfstate_storage_account=batecstate
tfstate_group_name=rg-batec-tfstate
echo 'Create an app registration for GitHub Actions'
gh_id=$(az ad app create --display-name "GitHub Actions dotnet Batec" --output tsv --query appId)
az ad app federated-credential create --id $gh_id --parameters gh_credential.json
gh_sp_oid=$(az ad sp create --id $gh_id --output tsv --query id)
echo 'Create a resource group for servers'
group_id=$(az group create --name $resource_group --location $location --output tsv --query id)
echo "Make GitHub Actions ($gh_sp_oid) a contributor to the resource group"
az role assignment create --assignee $gh_sp_oid --role contributor --scope $group_id

storage_account_id=$(az storage account show --name $tfstate_storage_account --resource-group $tfstate_group_name --output tsv --query id)
az role assignment create --assignee $gh_sp_oid --role contributor --scope $storage_account_id
