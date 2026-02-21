# -----------------------------------------------------------------------------
# Redis (standalone mode)
# -----------------------------------------------------------------------------

resource "random_password" "redis" {
  length  = 24
  special = false
}

resource "helm_release" "redis" {
  name       = "redis-${var.environment}"
  repository = "oci://registry-1.docker.io/bitnamicharts"
  chart      = "redis"
  version    = "20.6.2"
  namespace  = kubernetes_namespace.this.metadata[0].name
  timeout    = 600

  # Bitnami moved images from docker.io/bitnami to docker.io/bitnamilegacy
  set {
    name  = "image.repository"
    value = "bitnamilegacy/redis"
  }

  # Allow non-standard image (bitnamilegacy)
  set {
    name  = "global.security.allowInsecureImages"
    value = "true"
  }

  set {
    name  = "architecture"
    value = "standalone"
  }

  set_sensitive {
    name  = "auth.password"
    value = random_password.redis.result
  }

  set {
    name  = "master.persistence.size"
    value = var.redis_storage_size
  }

  set {
    name  = "master.resources.requests.cpu"
    value = "50m"
  }

  set {
    name  = "master.resources.requests.memory"
    value = "64Mi"
  }

  set {
    name  = "master.resources.limits.cpu"
    value = "200m"
  }

  set {
    name  = "master.resources.limits.memory"
    value = "128Mi"
  }
}
