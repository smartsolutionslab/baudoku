# -----------------------------------------------------------------------------
# PostGIS — Documentation database (requires PostGIS extension)
# Uses Bitnami PostgreSQL chart with PostGIS image override
# -----------------------------------------------------------------------------

resource "random_password" "postgis" {
  length  = 24
  special = false
}

resource "helm_release" "postgis" {
  name       = "postgis-${var.environment}"
  repository = "oci://registry-1.docker.io/bitnamicharts"
  chart      = "postgresql"
  version    = "16.4.3"
  namespace  = kubernetes_namespace.this.metadata[0].name

  # Ensure the service name matches what secrets.tf expects
  set {
    name  = "fullnameOverride"
    value = "postgis-${var.environment}"
  }

  # Override image to PostGIS
  set {
    name  = "image.registry"
    value = "docker.io"
  }

  set {
    name  = "image.repository"
    value = "postgis/postgis"
  }

  set {
    name  = "image.tag"
    value = "17-3.5"
  }

  set {
    name  = "auth.username"
    value = "baudoku"
  }

  set_sensitive {
    name  = "auth.password"
    value = random_password.postgis.result
  }

  set {
    name  = "auth.database"
    value = "baudoku_documentation"
  }

  # Enable PostGIS extension on the database
  set {
    name  = "primary.initdb.scripts.enable-postgis\\.sql"
    value = "CREATE EXTENSION IF NOT EXISTS postgis; CREATE EXTENSION IF NOT EXISTS postgis_topology;"
  }

  set {
    name  = "primary.persistence.size"
    value = var.postgis_storage_size
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
