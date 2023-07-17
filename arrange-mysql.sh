cd iac/mysql
terraform init
terraform apply -auto-approve
MYSQL_FQDN=$(terraform output -raw fqdn)
MYSQL_DATABASE=$(terraform output -raw database)
MYSQL_SERVER_ADMIN=$(terraform output -raw aad_admin)

cd ../../Microsoft.Azure.Data.Extensions.MySqlConnector
dotnet test -e MYSQL_FQDN=$MYSQL_FQDN -e MYSQL_DATABASE=$MYSQL_DATABASE -e MYSQL_SERVER_ADMIN=$MYSQL_SERVER_ADMIN