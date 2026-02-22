# -----------------------------------------------------------------------------
# Keycloak (Bitnami Helm chart)
# -----------------------------------------------------------------------------

resource "helm_release" "keycloak" {
  name       = "keycloak-${var.environment}"
  repository = "oci://registry-1.docker.io/bitnamicharts"
  chart      = "keycloak"
  version    = "25.2.0"
  namespace  = kubernetes_namespace.this.metadata[0].name
  timeout    = 900

  # Bitnami moved images from docker.io/bitnami to docker.io/bitnamilegacy
  set {
    name  = "image.repository"
    value = "bitnamilegacy/keycloak"
  }

  # Allow non-standard images (bitnamilegacy for keycloak + postgresql sub-chart)
  set {
    name  = "global.security.allowInsecureImages"
    value = "true"
  }

  set {
    name  = "auth.adminUser"
    value = "admin"
  }

  set_sensitive {
    name  = "auth.adminPassword"
    value = var.keycloak_admin_password
  }

  # Use embedded PostgreSQL for Keycloak's own DB
  set {
    name  = "postgresql.enabled"
    value = "true"
  }

  set {
    name  = "postgresql.image.repository"
    value = "bitnamilegacy/postgresql"
  }

  set {
    name  = "ingress.enabled"
    value = "true"
  }

  set {
    name  = "ingress.ingressClassName"
    value = "nginx"
  }

  set {
    name  = "ingress.hostname"
    value = "auth.${var.domain}"
  }

  set {
    name  = "ingress.annotations.cert-manager\\.io/cluster-issuer"
    value = "letsencrypt-prod"
  }

  set {
    name  = "ingress.tls"
    value = "true"
  }

  set {
    name  = "proxy"
    value = "edge"
  }

  set {
    name  = "resources.requests.cpu"
    value = "200m"
  }

  set {
    name  = "resources.requests.memory"
    value = "512Mi"
  }

  set {
    name  = "resources.limits.cpu"
    value = "1000m"
  }

  set {
    name  = "resources.limits.memory"
    value = "1Gi"
  }
}
