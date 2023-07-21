cd iac/mysql
terraform init
terraform apply -auto-approve
MYSQL_FQDN=$(terraform output -raw fqdn)
MYSQL_DATABASE=$(terraform output -raw database)
MYSQL_SERVER_ADMIN=$(terraform output -raw aad_admin)

cd ../../Microsoft.Azure.Data.Extensions.MySqlConnector/tests
dotnet test -e MYSQL_FQDN=$MYSQL_FQDN -e MYSQL_DATABASE=$MYSQL_DATABASE -e MYSQL_SERVER_ADMIN=$MYSQL_SERVER_ADMIN
cd ../../Microsoft.Azure.Data.Extensions.Pomelo.EntityFrameworkCore/tests
MYSQL_CONNECTION_STRING="Server=$MYSQL_FQDN;Port=3306;User ID=$MYSQL_SERVER_ADMIN;Database=$MYSQL_DATABASE;SSL Mode=Required;Allow Public Key Retrieval=True;Connection Timeout=30"
echo "{\"ConnectionStrings\":{\"DefaultConnection\":\"${MYSQL_CONNECTION_STRING}\"}}" >appsettings.json

dotnet ef database update

dotnet test -e MYSQL_FQDN=$MYSQL_FQDN -e MYSQL_DATABASE=$MYSQL_DATABASE -e MYSQL_SERVER_ADMIN=$MYSQL_SERVER_ADMIN

cd ../../iac/mysql
terraform destroy -auto-approve