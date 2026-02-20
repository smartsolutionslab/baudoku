# -----------------------------------------------------------------------------
# Staging Environment
# References existing DOKS cluster (owned by shared/ root)
# -----------------------------------------------------------------------------

provider "digitalocean" {
  # Token from DIGITALOCEAN_TOKEN env var
}

data "digitalocean_kubernetes_cluster" "this" {
  name = var.cluster_name
}

provider "kubernetes" {
  host                   = data.digitalocean_kubernetes_cluster.this.kube_config[0].host
  token                  = data.digitalocean_kubernetes_cluster.this.kube_config[0].token
  cluster_ca_certificate = base64decode(data.digitalocean_kubernetes_cluster.this.kube_config[0].cluster_ca_certificate)
}

provider "helm" {
  kubernetes {
    host                   = data.digitalocean_kubernetes_cluster.this.kube_config[0].host
    token                  = data.digitalocean_kubernetes_cluster.this.kube_config[0].token
    cluster_ca_certificate = base64decode(data.digitalocean_kubernetes_cluster.this.kube_config[0].cluster_ca_certificate)
  }
}

module "environment" {
  source = "../../modules/environment"

  environment            = "staging"
  image_tag              = var.image_tag
  domain                 = "staging.baudoku.example.com"
  aspnetcore_environment = "Staging"
  api_replicas           = 1
  dashboard_enabled      = true

  ghcr_username          = var.ghcr_username
  ghcr_token             = var.ghcr_token
  keycloak_admin_password = var.keycloak_admin_password

  postgresql_storage_size = "10Gi"
  postgis_storage_size    = "10Gi"
  rabbitmq_storage_size   = "2Gi"
  redis_storage_size      = "1Gi"
}

output "app_url" {
  value = module.environment.app_url
}

output "monitoring_url" {
  value = module.environment.monitoring_url
}

output "auth_url" {
  value = module.environment.auth_url
}

output "dashboard_token" {
  value     = module.environment.dashboard_token
  sensitive = true
}
