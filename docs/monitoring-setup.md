# Monitoring Setup

BauDoku uses the [Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone) for centralized monitoring of metrics, traces, and logs via OpenTelemetry.

`ServiceDefaults` already configures full OTLP export (metrics, traces, Serilog logs) when the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable is set. No .NET code changes are needed — just provide the endpoint.

## 1. Local Development with Aspire

When running via `dotnet run --project src/backend/AppHost/BauDoku.AppHost`, the Aspire Dashboard is built-in. No additional setup required.

- Dashboard: https://localhost:15888 (shown in console output)

## 2. Local Development without Aspire

For running services individually (e.g. a single API during debugging):

```bash
# Start the standalone dashboard
docker compose -f docker-compose.monitoring.yml up -d
```

Then set the environment variable for each service you run:

```bash
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:18889
```

Or in `launchSettings.json`:

```json
{
  "profiles": {
    "standalone": {
      "environmentVariables": {
        "OTEL_EXPORTER_OTLP_ENDPOINT": "http://localhost:18889"
      }
    }
  }
}
```

- Dashboard UI: http://localhost:18888
- OTLP gRPC endpoint: `localhost:18889`
- OTLP HTTP endpoint: `localhost:18890`

To stop: `docker compose -f docker-compose.monitoring.yml down`

## 3. Kubernetes (Staging / Production)

The Helm chart includes an optional Aspire Dashboard deployment, gated by `dashboard.enabled`.

### Enable in values file

```yaml
dashboard:
  enabled: true
  auth:
    browserToken: true
    secretName: baudoku-dashboard-auth
    secretKey: browser-token
  ingress:
    enabled: true
    host: monitoring.staging.baudoku.example.com

otel:
  enabled: true
  endpoint: http://aspire-dashboard:18889
```

When `otel.enabled` is true, `OTEL_EXPORTER_OTLP_ENDPOINT` is automatically injected into all service containers.

### Create the browser token Secret

```bash
kubectl create secret generic baudoku-dashboard-auth \
  --from-literal=browser-token="$(openssl rand -hex 32)"
```

Without the `browserToken` flag, the dashboard runs in anonymous mode (not recommended for production).

### Deploy

```bash
helm upgrade --install baudoku deploy/helm/baudoku \
  -f deploy/helm/baudoku/values-staging.yaml
```

The dashboard will be available at the configured ingress host.
