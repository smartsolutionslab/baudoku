output "app_url" {
  description = "Application URL"
  value       = "https://${var.domain}"
}

output "monitoring_url" {
  description = "Monitoring dashboard URL"
  value       = var.dashboard_enabled ? "https://monitoring.${var.domain}" : null
}

output "auth_url" {
  description = "Keycloak authentication URL"
  value       = "https://auth.${var.domain}"
}

output "namespace" {
  description = "Kubernetes namespace"
  value       = kubernetes_namespace.this.metadata[0].name
}

output "dashboard_token" {
  description = "Aspire dashboard browser token"
  value       = random_password.dashboard_token.result
  sensitive   = true
}
