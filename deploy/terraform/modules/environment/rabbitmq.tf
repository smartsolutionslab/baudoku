# -----------------------------------------------------------------------------
# RabbitMQ
# -----------------------------------------------------------------------------

resource "random_password" "rabbitmq" {
  length  = 24
  special = false
}

resource "helm_release" "rabbitmq" {
  name       = "rabbitmq-${var.environment}"
  repository = "oci://registry-1.docker.io/bitnamicharts"
  chart      = "rabbitmq"
  version    = "16.0.14"
  namespace  = kubernetes_namespace.this.metadata[0].name
  timeout    = 600

  # Bitnami moved images from docker.io/bitnami to docker.io/bitnamilegacy
  set {
    name  = "image.repository"
    value = "bitnamilegacy/rabbitmq"
  }

  set {
    name  = "auth.username"
    value = "baudoku"
  }

  set_sensitive {
    name  = "auth.password"
    value = random_password.rabbitmq.result
  }

  set {
    name  = "persistence.size"
    value = var.rabbitmq_storage_size
  }

  set {
    name  = "resources.requests.cpu"
    value = "100m"
  }

  set {
    name  = "resources.requests.memory"
    value = "256Mi"
  }

  set {
    name  = "resources.limits.cpu"
    value = "500m"
  }

  set {
    name  = "resources.limits.memory"
    value = "512Mi"
  }
}
