# -----------------------------------------------------------------------------
# PostgreSQL (standard) — Projects + Sync databases
# -----------------------------------------------------------------------------

resource "random_password" "postgresql" {
  length  = 24
  special = false
}

resource "helm_release" "postgresql" {
  name       = "postgresql-${var.environment}"
  repository = "oci://registry-1.docker.io/bitnamicharts"
  chart      = "postgresql"
  version    = "16.4.3"
  namespace  = kubernetes_namespace.this.metadata[0].name

  set {
    name  = "auth.username"
    value = "baudoku"
  }

  set_sensitive {
    name  = "auth.password"
    value = random_password.postgresql.result
  }

  set {
    name  = "auth.database"
    value = "baudoku_projects"
  }

  # Create the sync database via init scripts
  set {
    name  = "primary.initdb.scripts.create-sync-db\\.sql"
    value = "CREATE DATABASE baudoku_sync OWNER baudoku;"
  }

  set {
    name  = "primary.persistence.size"
    value = var.postgresql_storage_size
  }

  set {
    name  = "primary.resources.requests.cpu"
    value = "100m"
  }

  set {
    name  = "primary.resources.requests.memory"
    value = "256Mi"
  }

  set {
    name  = "primary.resources.limits.cpu"
    value = "500m"
  }

  set {
    name  = "primary.resources.limits.memory"
    value = "512Mi"
  }
}
