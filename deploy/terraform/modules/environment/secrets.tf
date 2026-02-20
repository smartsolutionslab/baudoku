# -----------------------------------------------------------------------------
# Kubernetes Secrets
# -----------------------------------------------------------------------------

locals {
  pg_host      = "postgresql-${var.environment}"
  postgis_host = "postgis-${var.environment}"
  rabbitmq_host = "rabbitmq-${var.environment}"
  redis_host    = "redis-${var.environment}-master"

  projects_connection_string      = "Host=${local.pg_host};Database=baudoku_projects;Username=baudoku;Password=${random_password.postgresql.result}"
  documentation_connection_string = "Host=${local.postgis_host};Database=baudoku_documentation;Username=baudoku;Password=${random_password.postgis.result}"
  sync_connection_string          = "Host=${local.pg_host};Database=baudoku_sync;Username=baudoku;Password=${random_password.postgresql.result}"
  rabbitmq_connection_string      = "amqp://baudoku:${random_password.rabbitmq.result}@${local.rabbitmq_host}:5672"
  redis_connection_string         = "${local.redis_host}:6379,password=${random_password.redis.result}"
}

resource "kubernetes_secret" "db_credentials" {
  metadata {
    name      = "baudoku-db-credentials"
    namespace = kubernetes_namespace.this.metadata[0].name
  }

  data = {
    "projects-connection-string"      = local.projects_connection_string
    "documentation-connection-string"  = local.documentation_connection_string
    "sync-connection-string"           = local.sync_connection_string
  }
}

resource "kubernetes_secret" "rabbitmq_credentials" {
  metadata {
    name      = "baudoku-rabbitmq-credentials"
    namespace = kubernetes_namespace.this.metadata[0].name
  }

  data = {
    "connection-string" = local.rabbitmq_connection_string
  }
}

resource "kubernetes_secret" "redis_credentials" {
  metadata {
    name      = "baudoku-redis-credentials"
    namespace = kubernetes_namespace.this.metadata[0].name
  }

  data = {
    "connection-string" = local.redis_connection_string
  }
}

resource "kubernetes_secret" "ghcr_credentials" {
  metadata {
    name      = "ghcr-credentials"
    namespace = kubernetes_namespace.this.metadata[0].name
  }

  type = "kubernetes.io/dockerconfigjson"

  data = {
    ".dockerconfigjson" = jsonencode({
      auths = {
        "ghcr.io" = {
          username = var.ghcr_username
          password = var.ghcr_token
          auth     = base64encode("${var.ghcr_username}:${var.ghcr_token}")
        }
      }
    })
  }
}
