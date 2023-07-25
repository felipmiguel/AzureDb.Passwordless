output "fqdn" {
  value       = azurerm_postgresql_flexible_server.database.fqdn
  description = "The MySQL server FQDN."
}

output "database" {
  value       = azurerm_postgresql_flexible_server_database.database.name
  description = "Database name"
}

output "aad_admin" {
  value       = azurerm_postgresql_flexible_server_active_directory_administrator.aad_admin.principal_name
  description = "Postgresql Azure AD admin."
}

output "principal_type" {
  value = data.azuread_directory_object.current_client.type
}