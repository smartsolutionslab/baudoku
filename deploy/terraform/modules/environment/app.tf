# -----------------------------------------------------------------------------
# BauDoku Application (deployed via existing Helm chart)
# -----------------------------------------------------------------------------

resource "random_password" "dashboard_token" {
  length  = 32
  special = false
}

resource "kubernetes_secret" "dashboard_auth" {
  count = var.dashboard_enabled ? 1 : 0

  metadata {
    name      = "baudoku-dashboard-auth"
    namespace = kubernetes_namespace.this.metadata[0].name
  }

  data = {
    "browser-token" = random_password.dashboard_token.result
  }
}

resource "kubernetes_secret" "grafana_auth" {
  count = var.monitoring_enabled ? 1 : 0

  metadata {
    name      = "grafana-admin-credentials"
    namespace = kubernetes_namespace.this.metadata[0].name
  }

  data = {
    "admin-password" = var.grafana_admin_password
  }
}

resource "helm_release" "baudoku" {
  name      = "baudoku"
  chart     = var.helm_chart_path
  namespace = kubernetes_namespace.this.metadata[0].name
  timeout   = 600

  # Don't wait for pods to become ready — they may fail readiness checks
  # until external DNS is configured for the domain (Keycloak authority
  # URL must be resolvable from within the cluster)
  wait = false

  values = [
    templatefile("${path.module}/templates/values.yaml.tftpl", {
      environment            = var.environment
      aspnetcore_environment = var.aspnetcore_environment
      image_tag              = var.image_tag
      api_replicas           = var.api_replicas
      domain                 = var.domain
      dashboard_enabled      = var.dashboard_enabled
      pull_policy            = var.environment == "staging" ? "Always" : "IfNotPresent"
      keycloak_authority     = "https://auth.${var.domain}/realms/baudoku"
      monitoring_enabled     = var.monitoring_enabled
      monitoring_retention   = var.monitoring_retention
    })
  ]

  depends_on = [
    helm_release.postgresql,
    kubernetes_stateful_set.postgis,
    helm_release.rabbitmq,
    helm_release.redis,
    helm_release.keycloak,
    kubernetes_secret.db_credentials,
    kubernetes_secret.rabbitmq_credentials,
    kubernetes_secret.redis_credentials,
    kubernetes_secret.ghcr_credentials,
    kubernetes_secret.grafana_auth,
  ]
}
