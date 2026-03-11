# ADR-004: .NET Aspire fuer lokale Orchestrierung

## Status

Accepted (aktualisiert Maerz 2026 fuer Aspire 13.1.1)

## Kontext

Die lokale Entwicklung erfordert das Starten mehrerer Services (3 APIs, Gateway, PostgreSQL, Redis, RabbitMQ, Keycloak). Ohne Orchestrierung ist das Setup-Erlebnis schlecht.

## Entscheidung

Wir verwenden **.NET Aspire 13.1.1** als lokale Orchestrierungsloesung:

- **AppHost-Projekt** definiert alle Services und deren Abhaengigkeiten
- Container fuer PostgreSQL+PostGIS, Redis, RabbitMQ, Keycloak und pgAdmin werden automatisch gestartet
- Service Discovery und Connection Strings werden automatisch injiziert
- Aspire Dashboard bietet Observability (Logs, Traces, Metrics) via OpenTelemetry
- Optional: Grafana-Stack (Prometheus, Tempo, Loki) ueber `--grafana` Flag

Fuer Production wird **Kubernetes** verwendet.

### SDK-basiertes Hosting (kein Workload)

Das AppHost-Projekt nutzt das Aspire SDK direkt im Projektformat:

```xml
<Project Sdk="Aspire.AppHost.Sdk/13.1.1">
```

Dies bringt DCP (Developer Control Plane) mit – ein separater `dotnet workload install aspire` ist **nicht** noetig. Die SDK-Variante ist portabler und vermeidet Workload-Versionskonflikte in CI.

### Container-Konfiguration

- **PostGIS-Image explizit setzen:** `postgres.WithImage("postgis/postgis", "17-3.5")` – der Standard `AddPostgres()` nutzt nur plain PostgreSQL ohne PostGIS-Erweiterung
- **DB-Ressourcennamen muessen uebereinstimmen:** Die Namen in AppHost (`ProjectsDb`, `DocumentationDb`, `SyncDb`) muessen exakt den Connection-String-Keys in den Services entsprechen
- **Keycloak** wird als Container mit Realm-Import konfiguriert

### Service Discovery

YARP (API Gateway) benoetigt fuer Aspire Service Discovery:
- Package `Microsoft.Extensions.ServiceDiscovery.Yarp`
- Registrierung: `.AddServiceDiscoveryDestinationResolver()`
- `appsettings.json` nutzt logische Servicenamen (`http://projects-api`)
- `appsettings.Development.json` (gitignored) enthaelt Localhost-Fallback fuer Entwicklung ohne Aspire

### HTTPS und Dashboard

Aspire 13.1 erfordert **HTTPS fuer das Dashboard**. Das `https` Launch-Profil ist Standard. Ohne HTTPS muss `ASPIRE_ALLOW_UNSECURED_TRANSPORT=true` gesetzt werden.

Das AppHost-Projekt **benoetigt** `Properties/launchSettings.json` mit OTLP-Endpunkt-Umgebungsvariablen – ohne diese schlaegt das Dashboard mit `OptionsValidationException` fehl.

### Central Package Management (CPM)

- **Kein `Aspire.Hosting` oder `Aspire.Hosting.AppHost` Package deklarieren** – diese sind im SDK implizit enthalten. Eine CPM `PackageVersion`-Deklaration verursacht NU1009-Fehler.
- Nur explizit benoetigte Hosting-Extensions deklarieren: `Aspire.Hosting.JavaScript`, `.PostgreSQL`, `.Redis`, `.RabbitMQ`

## Konsequenzen

### Positiv
- Ein Befehl startet die gesamte Umgebung (`dotnet run --project AppHost`)
- Automatisches Service Discovery zwischen allen Services
- Integriertes Dashboard fuer Debugging (Metrics, Traces, Logs)
- Kein Docker-Compose noetig fuer lokale Entwicklung
- Optionaler Grafana-Stack fuer produktionsnahe Observability
- SDK-basiert: kein Aspire-Workload noetig, portabler in CI/CD

### Negativ
- Aspire hat Breaking Changes zwischen Major-Versionen (v9 → v13)
- Production-Deployment braucht separate Kubernetes-Konfiguration
- `appsettings.Development.json` ist gitignored und muss lokal erstellt werden (Aspire injiziert Connection Strings automatisch, Standalone-Betrieb braucht manuelle Konfiguration)

### Bekannte Fallstricke
- `IsAspireHost` nicht auf `false` setzen – das SDK handhabt es
- `AddProject()` nutzt string-basierte Pfade (`AddProject("name", "path")`) statt generischer `AddProject<T>()` (keine Typ-Generierung ohne Workload)
- PostGIS-Image muss explizit gesetzt werden (Standard ist plain PostgreSQL)
- ServiceDefaults-Projekt braucht `IsAspireSharedProject=true` und `FrameworkReference` statt `Sdk.Web`
