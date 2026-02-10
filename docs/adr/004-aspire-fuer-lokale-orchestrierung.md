# ADR-004: .NET Aspire für lokale Orchestrierung

## Status

Accepted

## Kontext

Die lokale Entwicklung erfordert das Starten mehrerer Services (3 APIs, Gateway, PostgreSQL, Redis, RabbitMQ). Ohne Orchestrierung ist das Setup-Erlebnis schlecht.

## Entscheidung

Wir verwenden **.NET Aspire** als lokale Orchestrierungslösung:

- **AppHost-Projekt** definiert alle Services und deren Abhängigkeiten
- Container für PostgreSQL (mit PgAdmin), Redis und RabbitMQ werden automatisch gestartet
- Service Discovery und Connection Strings werden automatisch injiziert
- Aspire Dashboard bietet Observability (Logs, Traces, Metrics)

Für Production wird **Kubernetes** verwendet.

## Konsequenzen

### Positiv
- Ein Befehl startet die gesamte Umgebung (`dotnet run --project AppHost`)
- Automatisches Service Discovery
- Integriertes Dashboard für Debugging
- Kein Docker-Compose nötig für lokale Entwicklung

### Negativ
- Aspire ist relativ neu und hat Breaking Changes zwischen Versionen
- Production-Deployment braucht separate Kubernetes-Konfiguration
