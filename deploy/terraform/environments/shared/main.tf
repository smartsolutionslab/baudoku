# -----------------------------------------------------------------------------
# Shared Infrastructure — DOKS cluster + cluster-wide components
# -----------------------------------------------------------------------------

provider "digitalocean" {
  # Token from DIGITALOCEAN_TOKEN env var
}

provider "kubernetes" {
  host                   = module.cluster.kubeconfig.host
  token                  = module.cluster.kubeconfig.token
  cluster_ca_certificate = base64decode(module.cluster.kubeconfig.cluster_ca_certificate)
}

provider "helm" {
  kubernetes {
    host                   = module.cluster.kubeconfig.host
    token                  = module.cluster.kubeconfig.token
    cluster_ca_certificate = base64decode(module.cluster.kubeconfig.cluster_ca_certificate)
  }
}

module "cluster" {
  source = "../../modules/cluster"

  cluster_name = var.cluster_name
  region       = var.region
  acme_email   = var.acme_email
}

output "cluster_id" {
  value = module.cluster.cluster_id
}

output "cluster_name" {
  value = module.cluster.cluster_name
}

output "endpoint" {
  value = module.cluster.endpoint
}
