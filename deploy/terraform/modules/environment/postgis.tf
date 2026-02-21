# -----------------------------------------------------------------------------
# PostGIS — Documentation database (requires PostGIS extension)
# Uses native Kubernetes StatefulSet with official PostGIS image
# (Bitnami Helm chart is incompatible with non-Bitnami images)
# -----------------------------------------------------------------------------

resource "random_password" "postgis" {
  length  = 24
  special = false
}

locals {
  postgis_name = "postgis-${var.environment}"
  postgis_labels = {
    "app.kubernetes.io/name"       = "postgis"
    "app.kubernetes.io/instance"   = local.postgis_name
    "app.kubernetes.io/component"  = "primary"
    "app.kubernetes.io/managed-by" = "terraform"
  }
}

resource "kubernetes_config_map" "postgis_init" {
  metadata {
    name      = "${local.postgis_name}-init"
    namespace = kubernetes_namespace.this.metadata[0].name
  }

  data = {
    "enable-postgis.sql" = "CREATE EXTENSION IF NOT EXISTS postgis; CREATE EXTENSION IF NOT EXISTS postgis_topology;"
  }
}

resource "kubernetes_stateful_set" "postgis" {
  metadata {
    name      = local.postgis_name
    namespace = kubernetes_namespace.this.metadata[0].name
    labels    = local.postgis_labels
  }

  spec {
    service_name = local.postgis_name
    replicas     = 1

    selector {
      match_labels = {
        "app.kubernetes.io/name"     = "postgis"
        "app.kubernetes.io/instance" = local.postgis_name
      }
    }

    template {
      metadata {
        labels = local.postgis_labels
      }

      spec {
        container {
          name  = "postgis"
          image = "postgis/postgis:17-3.5-alpine"

          port {
            name           = "postgresql"
            container_port = 5432
          }

          env {
            name  = "POSTGRES_USER"
            value = "baudoku"
          }

          env {
            name  = "POSTGRES_PASSWORD"
            value = random_password.postgis.result
          }

          env {
            name  = "POSTGRES_DB"
            value = "baudoku_documentation"
          }

          env {
            name  = "PGDATA"
            value = "/var/lib/postgresql/data/pgdata"
          }

          volume_mount {
            name       = "data"
            mount_path = "/var/lib/postgresql/data"
          }

          volume_mount {
            name       = "init-scripts"
            mount_path = "/docker-entrypoint-initdb.d"
            read_only  = true
          }

          resources {
            requests = {
              cpu    = "100m"
              memory = "256Mi"
            }
            limits = {
              cpu    = "500m"
              memory = "512Mi"
            }
          }

          liveness_probe {
            exec {
              command = ["pg_isready", "-U", "baudoku", "-d", "baudoku_documentation"]
            }
            initial_delay_seconds = 30
            period_seconds        = 10
            timeout_seconds       = 5
            failure_threshold     = 6
          }

          readiness_probe {
            exec {
              command = ["pg_isready", "-U", "baudoku", "-d", "baudoku_documentation"]
            }
            initial_delay_seconds = 5
            period_seconds        = 10
            timeout_seconds       = 5
            failure_threshold     = 6
          }
        }

        volume {
          name = "init-scripts"
          config_map {
            name = kubernetes_config_map.postgis_init.metadata[0].name
          }
        }
      }
    }

    volume_claim_template {
      metadata {
        name = "data"
      }

      spec {
        access_modes = ["ReadWriteOnce"]

        resources {
          requests = {
            storage = var.postgis_storage_size
          }
        }
      }
    }
  }

  timeouts {
    create = "10m"
    update = "10m"
  }
}

resource "kubernetes_service" "postgis" {
  metadata {
    name      = local.postgis_name
    namespace = kubernetes_namespace.this.metadata[0].name
    labels    = local.postgis_labels
  }

  spec {
    selector = {
      "app.kubernetes.io/name"     = "postgis"
      "app.kubernetes.io/instance" = local.postgis_name
    }

    port {
      name        = "postgresql"
      port        = 5432
      target_port = 5432
    }

    type = "ClusterIP"
  }
}
