variable "environment" {
  description = "Environment name (staging, production)"
  type        = string

  validation {
    condition     = contains(["staging", "production"], var.environment)
    error_message = "Environment must be 'staging' or 'production'."
  }
}

variable "image_tag" {
  description = "Docker image tag to deploy"
  type        = string
  default     = "latest"
}

variable "domain" {
  description = "Domain name for the application (e.g. staging.baudoku.apps.smartsolutionslab.tech)"
  type        = string
}

variable "aspnetcore_environment" {
  description = "ASP.NET Core environment name"
  type        = string
  default     = "Production"
}

variable "api_replicas" {
  description = "Number of replicas for each API service"
  type        = number
  default     = 1
}

variable "dashboard_enabled" {
  description = "Whether to deploy the Aspire dashboard"
  type        = bool
  default     = true
}

# --- GHCR credentials ---

variable "ghcr_username" {
  description = "GitHub Container Registry username"
  type        = string
}

variable "ghcr_token" {
  description = "GitHub Container Registry token (PAT or GITHUB_TOKEN)"
  type        = string
  sensitive   = true
}

# --- Keycloak ---

variable "keycloak_admin_password" {
  description = "Keycloak admin password"
  type        = string
  sensitive   = true
}

# --- Storage sizes ---

variable "postgresql_storage_size" {
  description = "PVC size for PostgreSQL (Projects + Sync)"
  type        = string
  default     = "10Gi"
}

variable "postgis_storage_size" {
  description = "PVC size for PostGIS (Documentation)"
  type        = string
  default     = "10Gi"
}

variable "rabbitmq_storage_size" {
  description = "PVC size for RabbitMQ"
  type        = string
  default     = "2Gi"
}

variable "redis_storage_size" {
  description = "PVC size for Redis"
  type        = string
  default     = "1Gi"
}

# --- Helm chart ---

variable "helm_chart_path" {
  description = "Path to the BauDoku Helm chart"
  type        = string
  default     = "../../../../deploy/helm/baudoku"
}
