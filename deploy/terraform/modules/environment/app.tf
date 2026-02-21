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

resource "helm_release" "baudoku" {
  name      = "baudoku"
  chart     = var.helm_chart_path
  namespace = kubernetes_namespace.this.metadata[0].name
  timeout   = 600

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
    })
  ]

  depends_on = [
    helm_release.postgresql,
    helm_release.postgis,
    helm_release.rabbitmq,
    helm_release.redis,
    helm_release.keycloak,
    kubernetes_secret.db_credentials,
    kubernetes_secret.rabbitmq_credentials,
    kubernetes_secret.redis_credentials,
    kubernetes_secret.ghcr_credentials,
  ]
}
