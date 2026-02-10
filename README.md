# BauDoku

Mobile App zur Dokumentation elektrischer Installationen auf Baustellen.

**Status:** 🚧 Phase 0 – Foundation

## Stack

| Schicht | Technologie |
|---------|-------------|
| Mobile App | React Native + Expo (TypeScript) |
| Backend | .NET 10 Minimal API, DDD + Clean Architecture |
| Datenbank | PostgreSQL + PostGIS / SQLite (offline) |
| Auth | Keycloak (OAuth2 + OIDC) |
| Orchestrierung | .NET Aspire (dev) / Kubernetes (prod) |

## Quick Start

### Backend (alle Services mit .NET Aspire)

```bash
cd src/backend/AppHost
dotnet run
# → Aspire Dashboard: https://localhost:15888
```

### Frontend (Expo)

```bash
cd src/frontend
npm install
npx expo start
```

## Architektur

Siehe [docs/architecture.md](docs/architecture.md) für das vollständige Architektur-Dokument.

### Bounded Contexts

- **Projects** – Projektverwaltung, Zonen-Hierarchie
- **Documentation** – Installations-Erfassung, Fotos, Messungen, GPS
- **Sync** – Offline-Synchronisation

## Lizenz

Proprietary – All rights reserved.
