# -----------------------------------------------------------------------------
# Namespace
# -----------------------------------------------------------------------------

resource "kubernetes_namespace" "this" {
  metadata {
    name = "baudoku-${var.environment}"

    labels = {
      app         = "baudoku"
      environment = var.environment
    }
  }
}
