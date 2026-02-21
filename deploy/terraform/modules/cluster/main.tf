# -----------------------------------------------------------------------------
# DOKS Cluster
# -----------------------------------------------------------------------------

data "digitalocean_kubernetes_versions" "this" {
  version_prefix = "${var.k8s_version}."
}

resource "digitalocean_kubernetes_cluster" "this" {
  name    = var.cluster_name
  region  = var.region
  version = data.digitalocean_kubernetes_versions.this.latest_version

  node_pool {
    name       = "${var.cluster_name}-pool"
    size       = var.node_size
    auto_scale = true
    min_nodes  = var.min_nodes
    max_nodes  = var.max_nodes
  }

  maintenance_policy {
    start_time = "04:00"
    day        = "sunday"
  }
}

# -----------------------------------------------------------------------------
# Wait for cluster API to become available after creation
# -----------------------------------------------------------------------------

resource "time_sleep" "wait_for_cluster" {
  create_duration = "60s"
  depends_on      = [digitalocean_kubernetes_cluster.this]
}

# -----------------------------------------------------------------------------
# nginx-ingress controller
# -----------------------------------------------------------------------------

resource "helm_release" "nginx_ingress" {
  name             = "ingress-nginx"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  version          = "4.12.0"
  namespace        = "ingress-nginx"
  create_namespace = true
  cleanup_on_fail  = true

  set {
    name  = "controller.publishService.enabled"
    value = "true"
  }

  set {
    name  = "controller.resources.requests.cpu"
    value = "100m"
  }

  set {
    name  = "controller.resources.requests.memory"
    value = "128Mi"
  }

  depends_on = [time_sleep.wait_for_cluster]
}

# -----------------------------------------------------------------------------
# cert-manager
# -----------------------------------------------------------------------------

resource "helm_release" "cert_manager" {
  name             = "cert-manager"
  repository       = "https://charts.jetstack.io"
  chart            = "cert-manager"
  version          = "1.17.1"
  namespace        = "cert-manager"
  create_namespace = true
  cleanup_on_fail  = true

  set {
    name  = "crds.enabled"
    value = "true"
  }

  depends_on = [time_sleep.wait_for_cluster]
}

# -----------------------------------------------------------------------------
# ClusterIssuer for Let's Encrypt (production)
#
# Uses a local Helm chart instead of kubernetes_manifest because the latter
# requires a live cluster connection at plan time, which fails on first apply
# when the cluster doesn't exist yet.
# -----------------------------------------------------------------------------

resource "helm_release" "cluster_issuer" {
  name            = "letsencrypt-issuer"
  chart           = "${path.module}/charts/cluster-issuer"
  namespace       = "cert-manager"
  cleanup_on_fail = true

  set {
    name  = "email"
    value = var.acme_email
  }

  depends_on = [helm_release.cert_manager]
}
