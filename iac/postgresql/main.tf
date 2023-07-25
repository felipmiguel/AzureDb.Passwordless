terraform {
  required_providers {
    azurecaf = {
      source  = "aztfmod/azurecaf"
      version = "1.2.24"
    }
  }
  backend "azurerm" {
    # change this to your own storage account name
    resource_group_name  = "rg-batec-tfstate"
    storage_account_name = "batecstate"
    container_name       = "tfstate-pgsql"
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

data "azurerm_resource_group" "resource_group" {
  name = var.resource_group_name
}

resource "azurecaf_name" "psql_umi" {
  name          = var.application_name
  resource_type = "azurerm_user_assigned_identity"
  suffixes      = [var.environment]
}

resource "azurerm_user_assigned_identity" "psql_umi" {
  name                = azurecaf_name.psql_umi.result
  resource_group_name = data.azurerm_resource_group.resource_group.name
  location            = var.location
}

resource "azurecaf_name" "postgresql_server" {
  name          = var.application_name
  resource_type = "azurerm_postgresql_flexible_server"
  suffixes      = [var.environment]
}

resource "azurerm_postgresql_flexible_server" "database" {
  name                = azurecaf_name.postgresql_server.result
  resource_group_name = data.azurerm_resource_group.resource_group.name
  location            = var.location

  sku_name                     = "B_Standard_B1ms"
  storage_mb                   = 32768
  backup_retention_days        = 7
  version                      = "13"
  geo_redundant_backup_enabled = false

  identity {
    identity_ids = [azurerm_user_assigned_identity.psql_umi.id]
    type         = "UserAssigned"
  }

  authentication {
    active_directory_auth_enabled = true
    password_auth_enabled         = false
    tenant_id                     = azurerm_user_assigned_identity.psql_umi.tenant_id
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

data "azuread_user" "aad_admin" {
  count     = data.azuread_directory_object.current_client.type == "User" ? 1 : 0
  object_id = data.azurerm_client_config.current_client.object_id
}

data "azuread_application" "aad_admin" {
  count          = data.azuread_directory_object.current_client.type == "ServicePrincipal" ? 1 : 0
  application_id = data.azurerm_client_config.current_client.client_id
}

locals {
  # login_name = strcontains(data.azuread_user.aad_admin.user_principal_name, "#EXT#") ? data.azuread_user.aad_admin.other_mails[0] : data.azuread_user.aad_admin.user_principal_name
  login_name = data.azuread_directory_object.current_client.type == "User" ? data.azuread_user.aad_admin[0].user_principal_name : data.azuread_application.aad_admin[0].display_name
  login_sid  = data.azuread_directory_object.current_client.type == "User" ? data.azurerm_client_config.current_client.object_id : data.azuread_application.aad_admin[0].object_id
}

resource "azurerm_postgresql_flexible_server_active_directory_administrator" "aad_admin" {
  server_name         = azurerm_postgresql_flexible_server.database.name
  resource_group_name = data.azurerm_resource_group.resource_group.name
  tenant_id           = data.azurerm_client_config.current_client.tenant_id
  object_id           = local.login_sid
  principal_name      = local.login_name
  principal_type      = data.azuread_directory_object.current_client.type
}

resource "azurecaf_name" "postgresql_database" {
  name          = var.application_name
  resource_type = "azurerm_postgresql_flexible_server_database"
  suffixes      = [var.environment]
}

resource "azurerm_postgresql_flexible_server_database" "database" {
  name      = azurecaf_name.postgresql_database.result
  server_id = azurerm_postgresql_flexible_server.database.id
  charset   = "utf8"
  collation = "en_US.utf8"
}

resource "azurecaf_name" "postgresql_firewall_rule" {
  name          = var.application_name
  resource_type = "azurerm_postgresql_flexible_server_firewall_rule"
  suffixes      = [var.environment]
}

# This rule is to enable the 'Allow access to Azure services' checkbox
resource "azurerm_postgresql_flexible_server_firewall_rule" "database" {
  name             = azurecaf_name.postgresql_firewall_rule.result
  server_id        = azurerm_postgresql_flexible_server.database.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# This rule is to enable the access from agent IP
resource "azurecaf_name" "postgresql_firewall_rule_allow_iac_machine" {
  name          = var.application_name
  resource_type = "azurerm_postgresql_flexible_server_firewall_rule"
  suffixes      = [var.environment, "iac"]
}

data "http" "myip" {
  url = "http://whatismyip.akamai.com"
}

locals {
  myip = chomp(data.http.myip.response_body)
}

# This rule is to enable current machine
resource "azurerm_postgresql_flexible_server_firewall_rule" "rule_allow_iac_machine" {
  name             = azurecaf_name.postgresql_firewall_rule_allow_iac_machine.result
  server_id        = azurerm_postgresql_flexible_server.database.id
  start_ip_address = local.myip
  end_ip_address   = local.myip
}
