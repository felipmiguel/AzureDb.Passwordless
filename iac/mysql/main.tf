terraform {
  required_providers {
    azurecaf = {
      source  = "aztfmod/azurecaf"
      version = "1.2.24"
    }
    azapi = {
      source = "azure/azapi"
    }
    azuread = {
      source = "hashicorp/azuread"
    }
  }
  backend "azurerm" {
      # change this to your own storage account name
      resource_group_name  = "rg-Batec-tfstate"
      storage_account_name = "Batecstate"
      container_name       = "tfstate-mysql"
      key                  = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}

# resource "azurecaf_name" "resource_group" {
#   name          = var.application_name
#   resource_type = "azurerm_resource_group"
#   suffixes      = [var.environment]
# }

# resource "azurerm_resource_group" "resource_group" {
#   name     = azurecaf_name.resource_group.result
#   location = var.location
# }

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
    ignore_changes = [ zone, high_availability.0.standby_availability_zone ]
  }
}

data "azurerm_client_config" "current_client" {
}

data "azuread_user" "aad_admin" {
  object_id = data.azurerm_client_config.current_client.object_id
}

locals {
  # login_name = strcontains(data.azuread_user.aad_admin.user_principal_name, "#EXT#") ? data.azuread_user.aad_admin.other_mails[0] : data.azuread_user.aad_admin.user_principal_name
  login_name = data.azuread_user.aad_admin.user_principal_name
}

resource "azapi_resource" "mysql_aad_admin" {
  type = "Microsoft.DBforMySQL/flexibleServers/administrators@2021-12-01-preview"
  name = "ActiveDirectory"
  depends_on = [
    azurerm_mysql_flexible_server.database
  ]
  parent_id = azurerm_mysql_flexible_server.database.id
  body = jsonencode({
    properties = {
      administratorType  = "ActiveDirectory"
      identityResourceId = azurerm_user_assigned_identity.mysql_umi.id
      login              = local.login_name
      sid                = data.azuread_user.aad_admin.object_id
      tenantId           = data.azurerm_client_config.current_client.tenant_id
    }
  })
  timeouts {
    create = "10m"
    update = "5m"
    delete = "10m"
    read   = "3m"
  }
}

resource "azurerm_mysql_flexible_database" "database" {
  name                = var.database_name
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
