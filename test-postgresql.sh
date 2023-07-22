cd iac/postgresql
terraform init
terraform apply -auto-approve
POSTGRESQL_FQDN=$(terraform output -raw fqdn)
POSTGRESQL_DATABASE=$(terraform output -raw database)
POSTGRESQL_SERVER_ADMIN=$(terraform output -raw aad_admin)

cd ../../Sfinks.Azure.Data.Extensions.Npgsql/tests
dotnet test -e POSTGRESQL_FQDN=$POSTGRESQL_FQDN -e POSTGRESQL_DATABASE=$POSTGRESQL_DATABASE -e POSTGRESQL_SERVER_ADMIN=$POSTGRESQL_SERVER_ADMIN

cd ../../Sfinks.Azure.Data.Extensions.Npgsql.EntityFrameworkCore/tests
POSTGRESQL_CONNECTION_STRING="Server=$POSTGRESQL_FQDN;Port=5432;User ID=$POSTGRESQL_SERVER_ADMIN;Database=$POSTGRESQL_DATABASE;SSL Mode=Require;Trust Server Certificate=true"
echo "{\"ConnectionStrings\":{\"DefaultConnection\":\"${POSTGRESQL_CONNECTION_STRING}\"}}" >appsettings.json
dotnet ef database update
dotnet test -e POSTGRESQL_FQDN=$POSTGRESQL_FQDN -e POSTGRESQL_DATABASE=$POSTGRESQL_DATABASE -e POSTGRESQL_SERVER_ADMIN=$POSTGRESQL_SERVER_ADMIN

cd ../../iac/postgresql
terraform destroy -auto-approve