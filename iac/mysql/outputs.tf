output "fqdn" {
  value       = azurerm_mysql_flexible_server.database.fqdn
  description = "The MySQL server FQDN."
}

output "database" {
  value       = azurerm_mysql_flexible_database.database.name
  description = "Database name"
}

output "aad_admin" {
  value       = local.login_name
  description = "MySql Azure AD admin."
}

output "kv_name" {
  value = azurerm_key_vault.bateckv4455.name
}

output "aad_admin_client_id" {
  value = azuread_service_principal.aad_admin.application_id
}

output "aad_admin_client_secret" {
  sensitive = true
  value     = azuread_service_principal_password.aad_admin_password.value
}
