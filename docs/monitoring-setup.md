# Monitoring Setup

BauDoku uses the [Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone) for centralized monitoring of metrics, traces, and logs via OpenTelemetry.

`ServiceDefaults` already configures full OTLP export (metrics, traces, Serilog logs) when the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable is set. No .NET code changes are needed — just provide the endpoint.

## 1. Local Development with Aspire

When running via `dotnet run --project src/backend/AppHost/BauDoku.AppHost`, the Aspire Dashboard is built-in. No additional setup required.

- Dashboard: https://localhost:15888 (shown in console output)

## 2. Aspire with Grafana Stack

Run the full Grafana observability stack alongside the Aspire Dashboard by passing the `--grafana` flag:

```bash
dotnet run --project src/backend/AppHost/BauDoku.AppHost -- --grafana
```

This adds 5 containers to the Aspire orchestration: OTel Collector, Prometheus, Tempo, Loki, and Grafana. Telemetry is routed through the Collector which fans out to both dashboards simultaneously:

```
.NET Services (host) → OTel Collector (container, :4317)
                           ├── → Aspire Dashboard (host, :18889)
                           ├── → Tempo (container)
                           ├── → Loki (container)
                           └── → Prometheus (scrapes :8889)

Grafana (container, :3000) → Prometheus, Tempo, Loki
```

| URL | Description |
|-----|-------------|
| https://localhost:17178 | Aspire Dashboard (metrics, traces, logs, resources) |
| http://localhost:3000 | Grafana (pre-built dashboards, service map, alerts) |
| http://localhost:9090 | Prometheus (raw metrics, target health) |

Without the `--grafana` flag, behavior is unchanged — only the Aspire Dashboard runs.

> **Note:** The `--grafana` flag requires the `https` launch profile (default). The OTel Collector connects to the Aspire Dashboard via `host.docker.internal` with TLS skip-verify for the self-signed certificate.

## 3. Local Development without Aspire

For running services individually (e.g. a single API during debugging):

```bash
# Start the monitoring stack (Aspire Dashboard + Grafana stack)
docker compose -f docker-compose.monitoring.yml up -d
```

Then set the environment variable for each service you run:

```bash
# Send telemetry via OTel Collector (fans out to Aspire + Grafana stack)
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```

Or in `launchSettings.json`:

```json
{
  "profiles": {
    "standalone": {
      "environmentVariables": {
        "OTEL_EXPORTER_OTLP_ENDPOINT": "http://localhost:4317"
      }
    }
  }
}
```

- Aspire Dashboard UI: http://localhost:18888
- Grafana UI: http://localhost:3000
- OTLP gRPC endpoint (Collector): `localhost:4317`
- OTLP HTTP endpoint (Collector): `localhost:4318`

To stop: `docker compose -f docker-compose.monitoring.yml down`

## 4. Full Observability Stack (Grafana + Prometheus + Tempo + Loki)

For a production-like observability experience with cross-signal correlation, service maps, and pre-built dashboards, run the full stack alongside the Aspire Dashboard:

```
.NET Services → OTel Collector (:4317)
                    ├── → Aspire Dashboard (:18889)  [metrics, traces, logs]
                    ├── → Prometheus (:8889 scrape)   [metrics]
                    ├── → Tempo (:4317 internal)      [traces]
                    └── → Loki (:3100/otlp)           [logs]

Grafana (:3000) queries Prometheus, Tempo, Loki
```

```bash
# Start the full stack
docker compose -f docker-compose.monitoring.yml up -d

# Verify all containers are running
docker compose -f docker-compose.monitoring.yml ps
```

Then set the OTLP endpoint for each service you run locally:

```bash
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```

**Dashboards:**

| URL | Description |
|-----|-------------|
| http://localhost:18888 | Aspire Dashboard (metrics, traces, logs) |
| http://localhost:3000 | Grafana (pre-built dashboards, service map, alerts) |
| http://localhost:9090 | Prometheus (raw metrics, target health) |

**Grafana features:**

- **Pre-built dashboards:** Service Overview, API Latency, Infrastructure, Business Metrics (in `grafana/dashboards/`)
- **Cross-signal correlation:** Click a trace to see its logs, click a metric to jump to traces
- **Service map:** Auto-generated from trace data (Tempo metrics generator → Prometheus)
- **Alerting rules:** Defined in `grafana/alerting/rules.yml`, evaluated by Prometheus

**Reset all data:**

```bash
docker compose -f docker-compose.monitoring.yml down -v
```

## 5. Kubernetes (Staging / Production)

The Helm chart deploys both the Aspire Dashboard and the full Grafana observability stack to Kubernetes.

### Architecture

```
.NET Services → OTel Collector (otel-collector:4317)
                    ├── → Aspire Dashboard (aspire-dashboard:18889)
                    ├── → Tempo (tempo:4317)         [traces]
                    ├── → Loki (loki:3100)           [logs]
                    └── → Prometheus scrape (:8889)   [metrics]

Grafana (:3000) → Prometheus, Tempo, Loki
```

All monitoring components are single-instance (no HA), deployed in the same namespace as the application.

### Components

| Component | Type | Storage | Purpose |
|-----------|------|---------|---------|
| OTel Collector | Deployment | - | Receives OTLP, fans out to all backends |
| Prometheus | StatefulSet | PVC | Metrics storage, alerting rules |
| Tempo | StatefulSet | PVC | Distributed tracing |
| Loki | StatefulSet | PVC | Log aggregation |
| Grafana | Deployment | - | Visualization, dashboards, service map |
| Aspire Dashboard | Deployment | - | .NET resource explorer, traces, logs |

### URLs

| Environment | Grafana | Aspire Dashboard |
|-------------|---------|------------------|
| Staging | `https://grafana.staging.baudoku.apps.smartsolutionslab.tech` | `https://monitoring.staging.baudoku.apps.smartsolutionslab.tech` |
| Production | `https://grafana.baudoku.apps.smartsolutionslab.tech` | `https://monitoring.baudoku.apps.smartsolutionslab.tech` |

### Retention and Storage

| Environment | Retention | Storage per backend |
|-------------|-----------|---------------------|
| Staging | 7 days | 10Gi |
| Production | 30 days | 50Gi |

### Configuration

The monitoring stack is gated by `monitoring.enabled` in the Helm values. When enabled, telemetry flows through the OTel Collector instead of directly to the Aspire Dashboard:

```yaml
otel:
  enabled: true
  endpoint: http://otel-collector:4317  # NOT aspire-dashboard:18889

monitoring:
  enabled: true

  prometheus:
    retention: 7d
    storage:
      size: 10Gi

  grafana:
    ingress:
      enabled: true
      host: grafana.staging.baudoku.apps.smartsolutionslab.tech
```

### Secrets

Two secrets are required:

```bash
# Aspire Dashboard browser token (managed by Terraform)
kubectl create secret generic baudoku-dashboard-auth \
  --from-literal=browser-token="$(openssl rand -hex 32)"

# Grafana admin password (managed by Terraform)
kubectl create secret generic grafana-admin-credentials \
  --from-literal=admin-password="YOUR_PASSWORD"
```

When deploying via Terraform, both secrets are created automatically from input variables.

### Deploy

```bash
# Via Helm directly
helm upgrade --install baudoku deploy/helm/baudoku \
  -f deploy/helm/baudoku/values-staging.yaml

# Via Terraform (recommended)
cd deploy/terraform/environments/staging
terraform apply
```

### Adding Dashboards

1. Create or export a Grafana dashboard JSON file
2. Place it in `grafana/dashboards/`
3. Copy it to `deploy/helm/baudoku/files/dashboards/`
4. Redeploy — Grafana picks up new dashboards from the ConfigMap automatically

### Alerting

Prometheus alerting rules are defined in `grafana/alerting/rules.yml` and packaged into the Helm chart at `files/alerting/rules.yml`. Current alerts:

- **HighErrorRate** (critical): 5xx rate > 5% for 5 minutes
- **HighLatency** (warning): p95 > 2s for 5 minutes
- **HealthCheckFailure** (critical): service down for 1 minute
- **HighSyncConflictRate** (warning): conflict rate > 1/s for 10 minutes
- **DatabaseConnectionPoolExhausted** (warning): pool > 90% used for 5 minutes
