output "fqdn" {
  value       = azurerm_mysql_flexible_server.database.fqdn
  description = "The MySQL server FQDN."
}

output "database" {
  value       = azurerm_mysql_flexible_database.database.name
  description = "Database name"
}

output "aad_admin" {
  value       = data.azuread_user.aad_admin.user_principal_name
  description = "MySql Azure AD admin."
}
