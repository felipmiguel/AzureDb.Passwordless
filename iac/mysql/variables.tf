variable "application_name" {
  type        = string
  description = "The name of your application"
  default     = "netmysqlpwdless"
}

variable "resource_group_name" {
  type        = string
  description = "The name of the resource group in which to create the resources"
  default     = "rg-sfinks-stage"
}

variable "environment" {
  type        = string
  description = "The environment (dev, test, prod...)"
  default     = "dev"
}

variable "location" {
  type        = string
  description = "The Azure region where all resources in this example should be created"
  default     = "eastus"
}

variable "administrator_login" {
  type        = string
  description = "The MySQL administrator login"
  default     = "myadmin"
}

variable "database_name" {
  type        = string
  description = "The MySQL database name"
  default     = "db"
}
