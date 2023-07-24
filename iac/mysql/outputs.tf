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
