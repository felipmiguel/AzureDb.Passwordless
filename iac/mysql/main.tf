terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.7.0"
    }
    azurecaf = {
      source  = "aztfmod/azurecaf"
      version = "1.2.24"
    }
    azapi = {
      source  = "azure/azapi"
      version = ">=1.3.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = ">=2.23.0"
    }
  }
  backend "azurerm" {
    # change this to your own storage account name
    resource_group_name  = "rg-batec-tfstate"
    storage_account_name = "batecstate"
    container_name       = "tfstate-mysql"
    key                  = "terraform.tfstate"
    use_oidc             = true
  }
}

provider "azurerm" {
  use_oidc = true
  features {}
}

provider "azuread" {
  use_oidc = true
}

provider "azapi" {
  use_oidc = true
}

data "azurerm_resource_group" "resource_group" {
  name = var.resource_group_name
}

resource "azurecaf_name" "mysql_umi" {
  name          = var.application_name
  resource_type = "azurerm_user_assigned_identity"
  suffixes      = [var.environment]
}

resource "azurerm_user_assigned_identity" "mysql_umi" {
  name                = azurecaf_name.mysql_umi.result
  resource_group_name = data.azurerm_resource_group.resource_group.name
  location            = var.location
}

resource "azurecaf_name" "mysql_server" {
  name          = var.application_name
  resource_type = "azurerm_mysql_server"
  suffixes      = [var.environment]
}

resource "random_password" "password" {
  length           = 32
  special          = true
  override_special = "_%@"
}

resource "azurerm_mysql_flexible_server" "database" {
  name                = azurecaf_name.mysql_server.result
  resource_group_name = data.azurerm_resource_group.resource_group.name
  location            = var.location

  administrator_login    = var.administrator_login
  administrator_password = random_password.password.result

  sku_name                     = "B_Standard_B1s"
  version                      = "8.0.21"
  backup_retention_days        = 7
  geo_redundant_backup_enabled = false

  identity {
    identity_ids = [azurerm_user_assigned_identity.mysql_umi.id]
    type         = "UserAssigned"
  }

  tags = {
    "environment"      = var.environment
    "application-name" = var.application_name
  }

  lifecycle {
    ignore_changes = [zone, high_availability.0.standby_availability_zone]
  }
}

data "azurerm_client_config" "current_client" {
}

data "azuread_directory_object" "current_client" {
  object_id = data.azurerm_client_config.current_client.object_id
}

data "azuread_service_principal" "current_client" {
  count     = data.azuread_directory_object.current_client.type == "ServicePrincipal" ? 1 : 0
  object_id = data.azurerm_client_config.current_client.object_id
}

data "azuread_user" "aad_admin" {
  count     = data.azuread_directory_object.current_client.type == "User" ? 1 : 0
  object_id = data.azurerm_client_config.current_client.object_id
}

locals {
  login_name = data.azuread_directory_object.current_client.type == "User" ? data.azuread_user.aad_admin[0].user_principal_name : data.azuread_service_principal.current_client[0].display_name
  login_sid  = data.azuread_directory_object.current_client.type == "User" ? data.azurerm_client_config.current_client.object_id : data.azuread_service_principal.current_client[0].application_id
}

resource "azurerm_mysql_flexible_server_active_directory_administrator" "aad_admin" {
  server_id   = azurerm_mysql_flexible_server.database.id
  identity_id = azurerm_user_assigned_identity.mysql_umi.id
  login       = local.login_name
  object_id   = local.login_sid
  tenant_id   = data.azurerm_client_config.current_client.tenant_id
}

resource "azurecaf_name" "mysql_database" {
  name          = var.application_name
  resource_type = "azurerm_mysql_flexible_server_database"
  suffixes      = [var.environment]
}

resource "azurerm_mysql_flexible_database" "database" {
  name                = azurecaf_name.mysql_database.result
  resource_group_name = data.azurerm_resource_group.resource_group.name
  server_name         = azurerm_mysql_flexible_server.database.name
  charset             = "utf8mb3"
  collation           = "utf8mb3_unicode_ci"
}

resource "azurecaf_name" "mysql_firewall_rule" {
  name          = var.application_name
  resource_type = "azurerm_mysql_firewall_rule"
  suffixes      = [var.environment]
}

# This rule is to enable the 'Allow access to Azure services' checkbox
resource "azurerm_mysql_flexible_server_firewall_rule" "database" {
  name                = azurecaf_name.mysql_firewall_rule.result
  resource_group_name = data.azurerm_resource_group.resource_group.name
  server_name         = azurerm_mysql_flexible_server.database.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

resource "azurecaf_name" "mysql_firewall_rule_allow_iac_machine" {
  name          = var.application_name
  resource_type = "azurerm_mysql_firewall_rule"
  suffixes      = [var.environment, "iac"]
}

data "http" "myip" {
  url = "http://whatismyip.akamai.com"
}

locals {
  myip = chomp(data.http.myip.response_body)
}

# This rule is to enable current machine
resource "azurerm_mysql_flexible_server_firewall_rule" "rule_allow_iac_machine" {
  name                = azurecaf_name.mysql_firewall_rule_allow_iac_machine.result
  resource_group_name = data.azurerm_resource_group.resource_group.name
  server_name         = azurerm_mysql_flexible_server.database.name
  start_ip_address    = local.myip
  end_ip_address      = local.myip
}
