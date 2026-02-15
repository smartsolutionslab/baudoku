# BauDoku

Mobile App zur Dokumentation elektrischer Installationen auf Baustellen.

**Status:** 🚧 Sprint 8 – MVP Release-Vorbereitung

## Projektbeschreibung

BauDoku ist eine offline-faehige mobile Anwendung zur systematischen Dokumentation elektrischer Installationen auf Baustellen. Die App richtet sich an Elektriker, Tiefbauer, Elektroplaner und Bauleiter.

### Kernfunktionen

- **Installationsdokumentation** – Erfassung von Elektrokomponenten (Kabeltrassen, Verteilerdosen, Erdung, Blitzschutz u.v.m.) mit strukturierten Metadaten wie Kabelspezifikationen, Einbautiefen und Herstellerangaben
- **Fotodokumentation** – Vorher-/Nachher-/Detailfotos direkt an Installationen anhaengen, mit optionaler GPS-Verortung pro Bild
- **Messprotokollierung** – Elektrische Pruefmessungen (Isolationswiderstand, Schleifenimpedanz, RCD-Ausloesezeit) mit automatischer Pass/Fail-Bewertung anhand konfigurierbarer Schwellwerte
- **GPS-Positionierung** – Dreistufiges Positionierungskonzept: internes Smartphone-GPS (~3-5 m), externes DGNSS mit SAPOS-EPS (~30-50 cm) und RTK mit SAPOS-HEPS (~1-3 cm)
- **Offline-Synchronisation** – Vollstaendige Offline-Faehigkeit mit Delta-Sync, Versionskontrolle und automatischer Konflikterkennung/-aufloesung beim Wiederherstellen der Verbindung
- **Projektverwaltung** – Hierarchische Zonenstruktur (Gebaeude, Stockwerk, Raum, Graben) zur raeumlichen Organisation aller Installationen

## Stack

| Schicht | Technologie |
|---------|-------------|
| Mobile App | React Native + Expo (TypeScript) |
| Backend | .NET 10 Minimal API, DDD + Clean Architecture |
| Datenbank | PostgreSQL + PostGIS / SQLite (offline) |
| Auth | Keycloak (OAuth2 + OIDC) |
| Messaging | RabbitMQ |
| Orchestrierung | .NET Aspire (dev) / Kubernetes (prod) |

## Voraussetzungen

| Tool | Version | Hinweis |
|------|---------|---------|
| .NET SDK | 10.0.100 | Siehe `global.json` |
| Node.js | 22 LTS | Fuer Expo-Frontend |
| Docker Desktop | aktuell | Fuer Aspire-Container (PostgreSQL, Redis, RabbitMQ, Keycloak) |
| Expo CLI | aktuell | Wird via `npx` genutzt |
| EAS CLI | optional | Nur fuer native Builds (`npm install -g eas-cli`) |

## Projekt einrichten

### Backend (Aspire)

```bash
git clone https://github.com/your-org/baudoku.git
cd baudoku

# NuGet-Pakete wiederherstellen
dotnet restore src/backend/BauDoku.slnx

# Aspire AppHost starten (startet alle Services + Container)
dotnet run --project src/backend/AppHost/BauDoku.AppHost
```

Das Aspire Dashboard oeffnet sich automatisch unter `https://localhost:17178`. Folgende Ressourcen werden gestartet:

| Ressource | Typ | Beschreibung |
|-----------|-----|--------------|
| projects-api | .NET Service | Projektverwaltung API |
| documentation-api | .NET Service | Dokumentations-API |
| sync-api | .NET Service | Synchronisations-API |
| api-gateway | YARP Proxy | Reverse Proxy (Port 5000) |
| keycloak | Container | OAuth2/OIDC (Port 8080) |
| PostgreSQL + PostGIS | Container | 3 Datenbanken (ProjectsDb, DocumentationDb, SyncDb) |
| Redis | Container | Caching |
| RabbitMQ | Container | Messaging |
| pgAdmin | Container | DB-Administration |

### Frontend (Expo)

```bash
cd src/frontend
npm install
npx expo start
```

| Script | Befehl | Beschreibung |
|--------|--------|--------------|
| `start` | `npm start` | Expo Dev-Server starten |
| `android` | `npm run android` | Android-Emulator starten |
| `ios` | `npm run ios` | iOS-Simulator starten |
| `web` | `npm run web` | Web-Modus starten |
| `typecheck` | `npm run typecheck` | TypeScript-Pruefung |
| `test` | `npm test` | Jest-Tests ausfuehren |
| `test:watch` | `npm run test:watch` | Jest im Watch-Modus |

### Keycloak

Keycloak wird automatisch durch Aspire als Container gestartet und mit dem Realm aus `./keycloak` konfiguriert. Standard-Zugangsdaten: `admin` / `admin`.

> **Physisches Geraet:** Die `apiBaseUrl` und `keycloakUrl` in `src/frontend/app.json` muessen auf die LAN-IP des Entwicklungsrechners angepasst werden (Standard: `10.0.0.6`).

## Tests ausfuehren

### Backend

```bash
# Alle Unit- und Architektur-Tests (494+)
dotnet test src/backend/BauDoku.slnx

# Einzelnes Testprojekt
dotnet test tests/backend/BauDoku.Projects.UnitTests
```

Integrationstests und E2E-Tests benoetigen eine laufende Docker-Instanz (Testcontainers):

```bash
# Integrationstests (Docker muss laufen)
dotnet test tests/backend/BauDoku.Projects.IntegrationTests
dotnet test tests/backend/BauDoku.Documentation.IntegrationTests
dotnet test tests/backend/BauDoku.Sync.IntegrationTests

# E2E Smoke Tests (Cross-BC: Projects → Documentation → Sync)
dotnet test tests/backend/BauDoku.E2E.SmokeTests
```

| Testprojekt | Typ | Tests |
|-------------|-----|-------|
| `BauDoku.BuildingBlocks.UnitTests` | Unit | 59 |
| `BauDoku.Projects.UnitTests` | Unit | 49 |
| `BauDoku.Documentation.UnitTests` | Unit | 272 |
| `BauDoku.Sync.UnitTests` | Unit | 100 |
| `BauDoku.ArchitectureTests` | Architektur | 14 |
| `BauDoku.Projects.IntegrationTests` | Integration | 2 |
| `BauDoku.Documentation.IntegrationTests` | Integration | 10 |
| `BauDoku.Sync.IntegrationTests` | Integration | 14 |
| `BauDoku.E2E.SmokeTests` | E2E | 1 |

### Frontend

```bash
cd src/frontend
npm run typecheck   # TypeScript-Pruefung
npm test            # Jest Unit-Tests
```

## Architektur

```
┌─────────────┐     ┌──────────────────────────────────────────────┐
│  Mobile App │────▶│  API Gateway (YARP)  :5000                   │
│  (Expo)     │     └──────┬──────────┬──────────┬─────────────────┘
└─────────────┘            │          │          │
                    ┌──────▼───┐ ┌────▼─────┐ ┌─▼──────┐
                    │ Projects │ │  Docu-   │ │  Sync  │
                    │   API    │ │mentation │ │  API   │
                    │          │ │   API    │ │        │
                    └──────┬───┘ └────┬─────┘ └─┬──────┘
                           │          │          │
                    ┌──────▼──────────▼──────────▼──────┐
                    │   PostgreSQL + PostGIS             │
                    │   (3 separate Datenbanken)         │
                    └───────────────────────────────────┘
```

Jeder Bounded Context folgt DDD + Clean Architecture (Domain → Application → Infrastructure → Api) mit eigenem Dispatcher (kein MediatR).

Detaillierte Dokumentation:
- [Architektur-Dokument](docs/baudoku-architektur.md)
- [Monitoring & Observability](docs/monitoring-setup.md)

### Bounded Contexts

- **Projects** – Projektverwaltung, Zonen-Hierarchie (Gebaeude → Stockwerk → Raum → Graben)
- **Documentation** – Installations-Erfassung, Fotos, Messungen, GPS-Positionen, Kabelspezifikationen
- **Sync** – Offline-Synchronisation (Outbox Pattern, Delta-Sync, Konfliktaufloesung)

### Architecture Decision Records (ADR)

| ADR | Thema |
|-----|-------|
| [001](docs/adr/001-dotnet-clean-architecture.md) | .NET Clean Architecture |
| [002](docs/adr/002-eigener-dispatcher-statt-mediatr.md) | Eigener Dispatcher statt MediatR |
| [003](docs/adr/003-bounded-contexts-als-services.md) | Bounded Contexts als Services |
| [004](docs/adr/004-aspire-fuer-lokale-orchestrierung.md) | Aspire fuer lokale Orchestrierung |
| [005](docs/adr/005-offline-first-mit-sqlite.md) | Offline-First mit SQLite |
| [006](docs/adr/006-awesomeassertions-statt-fluentassertions.md) | AwesomeAssertions statt FluentAssertions |

## Umgebungsvariablen

| Variable | Beschreibung | Standard |
|----------|-------------|----------|
| `ASPIRE_ALLOW_UNSECURED_TRANSPORT` | HTTP statt HTTPS fuer Aspire Dashboard erlauben | nicht gesetzt (HTTPS) |
| `DOTNET_DASHBOARD_OTLP_ENDPOINT_URL` | OTLP-Endpunkt fuer Aspire Dashboard | `https://localhost:21147` |
| `DOTNET_RESOURCE_SERVICE_ENDPOINT_URL` | Resource-Service-Endpunkt | `https://localhost:22239` |
| `apiBaseUrl` (app.json) | Backend-URL fuer Mobile App | `http://10.0.0.6:5000` |
| `keycloakUrl` (app.json) | Keycloak-URL fuer Mobile App | `http://10.0.0.6:8080` |

> **Hinweis:** `appsettings.Development.json`-Dateien sind in `.gitignore` eingetragen und muessen lokal erstellt werden. Die Aspire-Orchestrierung konfiguriert Connection Strings automatisch via Service Discovery.

## CI/CD

| Workflow | Datei | Beschreibung |
|----------|-------|-------------|
| **CI** | `.github/workflows/ci.yml` | Build, Unit-/Integrations-/E2E-Tests, Docker-Images. Laueft bei Push/PR auf `main` und `development`. |
| **E2E** | `.github/workflows/e2e.yml` | Naechtliche mobile E2E-Tests (Android-Emulator). Taeglich 3:00 UTC, ueberspringt wenn keine Commits in 24h. |
| **Deploy** | `.github/workflows/deploy.yml` | Deployment nach Staging (Azure AKS + Helm). Wird nach erfolgreichem CI auf `main` ausgeloest. |

## Troubleshooting

**DLL-Lock auf Windows**
Laufende Services sperren DLL-Dateien. Vor einem Rebuild alle Prozesse beenden:
```powershell
Get-Process -Name 'BauDoku*' | Stop-Process -Force
```

**Docker-Speicher**
Aspire startet mehrere Container gleichzeitig (PostgreSQL, Redis, RabbitMQ, Keycloak, pgAdmin). Docker Desktop sollte mindestens 4 GB RAM zugewiesen haben.

**Port-Konflikte**
Haeufig belegte Ports: Keycloak (`8080`), API Gateway (`5000`), Aspire Dashboard (`17178`), Expo (`8081`).

**expo-sqlite im Web-Modus**
`expo-sqlite` funktioniert nur auf nativen Plattformen (Android/iOS). Im Web-Modus tritt ein WASM-Fehler auf – `npx expo start --web` daher nur fuer UI-Entwicklung ohne SQLite-Features nutzen.

**Testcontainers: Docker muss laufen**
Integrations- und E2E-Tests verwenden Testcontainers und benoetigen eine laufende Docker-Instanz. Der Ryuk-Container raeumt gestoppte Test-Container automatisch auf.

## Lizenz

Proprietary – All rights reserved.
