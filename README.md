# BauDoku

Mobile App zur Dokumentation elektrischer Installationen auf Baustellen.

**Status:** 🚧 Phase 0 – Foundation

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
