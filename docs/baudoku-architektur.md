# Solution Architecture: BauDoku App
## Elektrische Installationsdokumentation für Baustellen

**Version:** 4.0 – DDD-Architektur mit .NET 10, eigenem Dispatcher, sealed record ValueObjects  
**Datum:** Februar 2026  
**Stack:** React Native + Expo (TypeScript) / .NET 10 Minimal API (C#) / DDD + Clean Architecture

---

## 1. Executive Summary

BauDoku ist eine mobile App zur Dokumentation elektrischer Installationen auf Baustellen. Zielgruppe sind Elektriker, Tiefbauer, Elektroplaner und Bauleiter. Die App ermöglicht die Offline-fähige Erfassung von Elektrokomponenten mit Fotos, GPS-Position, Maßangaben und Prüfprotokollen.

### GPS/GNSS-Strategie

Die App implementiert ein dreistufiges Positionierungskonzept: Stufe 1 nutzt internes Smartphone-GPS (~3–5 m) für grobe Verortung. Stufe 2 bindet externe DGNSS-Empfänger per Bluetooth an und nutzt SAPOS-EPS-Korrekturdaten für ~30–50 cm Genauigkeit. Stufe 3 unterstützt RTK-Empfänger mit SAPOS-HEPS für Zentimeter-Genauigkeit bei professioneller Vermessung. Jede gespeicherte Position enthält vollständige Metadaten (Quelle, Genauigkeit, Korrekturdienst, RTK-Status).

### Technologie-Entscheidung

| Schicht | Technologie | Begründung |
|---------|-------------|------------|
| **Mobile App** | React Native + Expo (TypeScript) | Größtes Ökosystem, Offline-First-Support, Web-Target möglich, TypeScript-Erfahrung vorhanden |
| **Web-Portal** | Expo Web (react-native-web) | Code-Sharing mit Native App (~60-70%), Dashboard für Bauleiter |
| **Backend API** | .NET 10 Minimal API (C#) | 19+ Jahre C#/.NET-Expertise, DDD + Clean Architecture, eigener Dispatcher (kein MediatR), sealed record ValueObjects |
| **Datenbank** | PostgreSQL + PostGIS | Geo-Queries, Open Source, bewährt |
| **Dateispeicher** | Azure Blob Storage / S3 | Skalierbar für Fotos |
| **Auth** | Keycloak / Azure AD B2C | Enterprise-ready, OIDC/OAuth2 |

### Strategischer Vorteil: Eine Codebase, drei Targets

```
Expo Codebase (TypeScript)
├── expo run:android   → Play Store (Bauarbeiter)
├── expo run:ios       → App Store (Bauarbeiter)
└── expo export:web    → PWA/Web-Portal (Bauleiter/Auftraggeber)
```

---

## 2. Zielgruppen & Use Cases

### Primär: Erfasser (Native App – iOS & Android)

| Rolle | Kernaktionen |
|-------|-------------|
| Elektriker / Monteur | Fotos aufnehmen, Annotation zeichnen, Komponenten erfassen, Prüfwerte eintragen |
| Tiefbauer | Kabeltrassen dokumentieren, Verlegetiefe messen, GPS-Position erfassen |
| Elektroplaner | Stromkreise zuordnen, Prüfprotokolle erstellen, Abnahmen signieren |

### Sekundär: Konsumenten (Web-Portal / PWA)

| Rolle | Kernaktionen |
|-------|-------------|
| Bauleiter | Fortschritt prüfen, Dokumentationen freigeben, Berichte generieren |
| Auftraggeber | Dokumentationen einsehen, Suche nach Installationen |
| Facility Manager | Bestandsdokumentation nutzen, Wartungsplanung |

---

## 3. Funktionale Anforderungen

### 3.1 MVP – Phase 1

**Projektverwaltung**
- Projekte anlegen (Name, Adresse, GPS, Auftraggeber)
- Baustellenbereiche/Zonen hierarchisch definieren (Gebäude → Stockwerk → Raum)
- QR-Code-Zuordnung zu physischen Bereichen

**Installationsdokumentation**
- Elektrokomponenten erfassen: Typ, Hersteller, Modell, Seriennummer
- Kabeltyp, Querschnitt, Länge, Verlegeweg
- Anschlussart, Absicherung, Stromkreiszuordnung
- Freitext-Notizen und Sprachnotizen

**Fotodokumentation**
- Mehrere Fotos pro Installation (vorher / nachher / Detail)
- Automatische Metadaten: GPS, Zeitstempel, Geräte-ID
- Foto-Annotation: Pfeile, Maßlinien, Textlabels direkt auf dem Bild

**Vermessung & Positionierung** (Details: Abschnitt 6 – GPS/GNSS-Architektur)
- GPS-Koordinaten automatisch erfassen (internes GPS als Minimum)
- Dreistufiges Genauigkeitskonzept: intern (~3–5 m) → DGNSS+SAPOS-EPS (~30 cm) → RTK+SAPOS-HEPS (~2 cm)
- Vollständige GPS-Metadaten mitspeichern: Quelle, Genauigkeit, Korrekturdienst, RTK-Status, Satellitenanzahl
- Genauigkeitswarnung in der UI wenn unter Schwellwert
- Tiefenangaben (Kabelverlegungstiefe)
- Abstands-/Maßangaben relativ zu Referenzpunkten
- Unterstützung externer GNSS-Empfänger via Bluetooth (NMEA-Protokoll)
- NTRIP-Client für SAPOS-Korrekturdaten (Phase 2)

**Offline-Funktionalität**
- 100% Kernfunktionalität offline nutzbar
- Lokale SQLite-Datenbank + lokaler Dateispeicher für Fotos
- Automatische Synchronisation bei Verbindung
- Konfliktauflösung bei gleichzeitiger Bearbeitung

### 3.2 Phase 2 – Erweiterungen

- PDF-Berichtsgenerierung (VDE/DIN-konform)
- QR-/Barcode-Scanner für Komponentenidentifikation
- Grundriss-Integration (Position auf Plan markieren)
- Web-Portal für Bauleiter (Expo Web)
- Prüfprotokoll-Vorlagen (Isolationswiderstand, Schleifenimpedanz, RCD)
- Unterschriften-Funktion für Abnahmen

### 3.3 Phase 3 – Premium

- CAD-Viewer (DWG/DXF) mit Positionsmarkierung
- AR-Overlay für Kabelverläufe
- Dashboard & Analytics
- API für Drittsystem-Integration (ERP, SAP, BIM)
- Mandantenfähigkeit (SaaS-Modell)

---

## 4. Nicht-funktionale Anforderungen

| Anforderung | Spezifikation |
|-------------|---------------|
| **Offline-Fähigkeit** | 100% Kernfunktionalität offline, bis zu 2 GB lokaler Cache |
| **Performance** | App-Start < 3s, Foto-Erfassung < 1s, Annotation flüssig bei 60fps |
| **Sync** | Delta-Sync, Resume bei Verbindungsabbruch, Chunked Photo Upload |
| **Sicherheit** | SQLCipher (verschlüsselte SQLite), TLS 1.3, JWT, RBAC |
| **UX** | Große Touch-Targets (48dp+), Handschuh-tauglich, Sonnenlicht-lesbar |
| **Akku** | Sparsamer GPS-Modus, ganztägiger Einsatz möglich |
| **GPS/GNSS** | Internes GPS als Fallback (3–5 m), SAPOS-EPS ~30 cm (Ziel-MVP), SAPOS-HEPS ~2 cm (Phase 3). Genauigkeit immer mitspeichern. |
| **Compliance** | DSGVO-konform, optionale VDE-/DIN-Konformität der Berichte |
| **Skalierung** | Bis 500 gleichzeitige Nutzer, 10.000+ Dokumentationen/Projekt |

---

## 5. Systemarchitektur

### 5.1 High-Level Übersicht

```
┌──────────────────────────────────────────────────────────────────┐
│                        CLIENTS                                    │
│                                                                    │
│  ┌─────────────────────┐   ┌─────────────────────────────────┐   │
│  │   Native App         │   │   Web-Portal / PWA               │   │
│  │   (Expo + RN)        │   │   (Expo Web)                     │   │
│  │                      │   │                                   │   │
│  │   iOS + Android      │   │   Bauleiter / Auftraggeber       │   │
│  │   Offline-First      │   │   Online, Read-Heavy             │   │
│  │   Kamera, GPS, Skia  │   │   Dashboard, Berichte, Suche    │   │
│  │   SQLite + FileSystem│   │   Service Worker Cache           │   │
│  │   BLE → Ext. GNSS    │   │                                   │   │
│  │   NTRIP → SAPOS      │   │                                   │   │
│  └──────────┬──────────┘   └───────────────┬─────────────────┘   │
│             │                               │                      │
│             │     Shared Codebase:          │                      │
│             │     - DTOs / Interfaces       │                      │
│             │     - API Client (fetch/axios) │                     │
│             │     - Validierung             │                      │
│             │     - State Management        │                      │
│             │     - UI Komponenten (~60%)   │                      │
└─────────────┼───────────────────────────────┼──────────────────────┘
              │          REST / gRPC          │
              ▼                               ▼
┌──────────────────────────────────────────────────────────────────┐
│                     BACKEND (.NET 10 – DDD + Clean Architecture)  │
│                                                                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌────────────────┐   │
│  │ API      │  │ Sync     │  │ Report   │  │ Notification   │   │
│  │ Gateway  │  │ Service  │  │ Service  │  │ Service        │   │
│  │ (YARP)   │  │          │  │ (PDF)    │  │ (SignalR/Push) │   │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────────┬───────┘   │
│       │              │              │                  │           │
│  ┌────▼──────────────▼──────────────▼──────────────────▼───────┐  │
│  │                    Application Layer                         │  │
│  │    DDD + Clean Architecture / CQRS                         │  │
│  │    Eigener Dispatcher (kein MediatR / Wolverine)           │  │
│  │    sealed record ValueObjects (keine Primitiven im Domain) │  │
│  └─────────────────────────┬───────────────────────────────────┘  │
│                             │                                      │
│  ┌──────────────────────────▼──────────────────────────────────┐  │
│  │                    Infrastructure Layer                      │  │
│  │                                                              │  │
│  │  PostgreSQL    Azure Blob     Redis      RabbitMQ            │  │
│  │  + PostGIS     Storage        Cache      Message Queue       │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  Auth: Keycloak / Azure AD B2C (OAuth2 + OIDC)                   │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│                     INFRASTRUKTUR                                 │
│  Docker + Kubernetes (AKS / EKS)                                  │
│  CI/CD: GitHub Actions                                            │
│  Monitoring: Seq + Grafana + Prometheus                           │
│  Environments: Dev → QA → Staging → Production                    │
└──────────────────────────────────────────────────────────────────┘
```

### 5.2 Frontend-Architektur (Expo + React Native)

```
baudoku-app/
├── app/                          # Expo Router (File-based Routing)
│   ├── (auth)/                   # Auth-Screens (Login, Register)
│   │   ├── login.tsx
│   │   └── register.tsx
│   ├── (tabs)/                   # Hauptnavigation (Tab Bar)
│   │   ├── projects/             # Projekt-Liste & Detail
│   │   │   ├── index.tsx         # Projekt-Übersicht
│   │   │   └── [id]/
│   │   │       ├── index.tsx     # Projekt-Detail
│   │   │       ├── zones.tsx     # Zonen-Verwaltung
│   │   │       └── installations/
│   │   │           ├── index.tsx
│   │   │           ├── [installId].tsx
│   │   │           └── new.tsx   # Neue Installation erfassen
│   │   ├── capture/              # Schnellerfassung (Kamera direkt)
│   │   │   └── index.tsx
│   │   ├── sync/                 # Sync-Status
│   │   │   └── index.tsx
│   │   └── profile/              # Einstellungen
│   │       └── index.tsx
│   └── _layout.tsx               # Root Layout
│
├── src/
│   ├── components/               # Wiederverwendbare UI-Komponenten
│   │   ├── ui/                   # Basis (Button, Input, Card, ...)
│   │   ├── forms/                # Formular-Komponenten
│   │   │   ├── InstallationForm.tsx
│   │   │   ├── ComponentPicker.tsx
│   │   │   └── MeasurementInput.tsx
│   │   ├── photo/                # Foto-bezogene Komponenten
│   │   │   ├── CameraCapture.tsx
│   │   │   ├── PhotoAnnotation.tsx  # ← Skia Canvas
│   │   │   ├── PhotoGallery.tsx
│   │   │   └── PhotoThumbnail.tsx
│   │   ├── map/                  # Karten / Positionierung
│   │   │   ├── LocationPicker.tsx
│   │   │   ├── GnssStatusBar.tsx    # RTK-Status, Genauigkeit, Satelliten
│   │   │   └── AccuracyWarning.tsx  # Warnung bei zu geringer Genauigkeit
│   │   └── sync/                 # Sync-Status-Anzeige
│   │       ├── SyncIndicator.tsx
│   │       └── ConflictResolver.tsx
│   │
│   ├── db/                       # Lokale Datenbank (Expo SQLite + Drizzle)
│   │   ├── schema.ts             # Drizzle Table Definitions
│   │   ├── migrations/           # SQL Migrations
│   │   ├── client.ts             # DB Connection Setup
│   │   └── repositories/         # Data Access Layer
│   │       ├── projectRepo.ts
│   │       ├── installationRepo.ts
│   │       ├── photoRepo.ts
│   │       └── syncRepo.ts       # Outbox / Change-Log
│   │
│   ├── api/                      # Backend API Client
│   │   ├── client.ts             # Axios/Fetch Setup + Auth Interceptor
│   │   ├── endpoints/
│   │   │   ├── projects.ts
│   │   │   ├── installations.ts
│   │   │   ├── photos.ts
│   │   │   └── sync.ts
│   │   └── types/                # Shared DTOs (generiert via OpenAPI)
│   │       └── api.generated.ts
│   │
│   ├── sync/                     # Offline-Sync Engine
│   │   ├── SyncManager.ts        # Orchestriert den Sync-Prozess
│   │   ├── SyncQueue.ts          # Outbox-Pattern Implementation
│   │   ├── ConflictStrategy.ts   # Last-Writer-Wins / Field-Level Merge
│   │   ├── PhotoUploader.ts      # Chunked Upload mit Resume
│   │   └── ConnectivityMonitor.ts
│   │
│   ├── gnss/                     # GPS/GNSS-Positionierung (Abschnitt 6)
│   │   ├── types.ts              # GeoPosition, PositionSource, RtkFixStatus
│   │   ├── PositionManager.ts    # Orchestriert Positionsquellen
│   │   ├── InternalGpsProvider.ts # expo-location (Stufe 1)
│   │   ├── ExternalGnssProvider.ts # BLE + NMEA (Stufe 2+3)
│   │   ├── NtripClient.ts        # SAPOS-Korrekturdaten (Phase 2)
│   │   ├── NmeaParser.ts         # NMEA 0183 Parsing (GGA, RMC, GSA)
│   │   └── config.ts             # SAPOS-Presets pro Bundesland
│   │
│   ├── store/                    # State Management (Zustand)
│   │   ├── useProjectStore.ts
│   │   ├── useInstallationStore.ts
│   │   ├── useSyncStore.ts
│   │   └── useAuthStore.ts
│   │
│   ├── hooks/                    # Custom React Hooks
│   │   ├── useCamera.ts
│   │   ├── useLocation.ts        # Nutzt PositionManager (GNSS-Abstraktion)
│   │   ├── useGnssStatus.ts      # RTK-Status, Genauigkeit, Satellite Count
│   │   ├── useOfflineData.ts
│   │   └── useSyncStatus.ts
│   │
│   ├── utils/                    # Hilfsfunktionen
│   │   ├── photoCompression.ts
│   │   ├── geoUtils.ts
│   │   ├── validation.ts
│   │   └── platform.ts           # Platform.OS Checks
│   │
│   └── constants/                # Konfiguration
│       ├── theme.ts              # Farben, Fonts, Spacing
│       ├── installationTypes.ts  # Elektro-Komponenten-Katalog
│       └── config.ts             # API URLs, Feature Flags
│
├── assets/                       # Statische Assets
├── app.json                      # Expo Konfiguration
├── tsconfig.json
├── drizzle.config.ts
└── package.json
```

### 5.3 Expo Tech-Stack im Detail

```json
{
  "dependencies": {
    "expo": "~52.x",
    "expo-router": "~4.x",
    "expo-camera": "~16.x",
    "expo-location": "~18.x",
    "expo-file-system": "~18.x",
    "expo-sqlite": "~15.x",
    "expo-barcode-scanner": "~13.x",
    "expo-image-picker": "~16.x",
    "expo-secure-store": "~14.x",
    "expo-notifications": "~0.29.x",

    "react-native": "0.76.x",
    "@shopify/react-native-skia": "^1.x",
    "react-native-reanimated": "~3.x",
    "react-native-gesture-handler": "~2.x",
    "react-native-ble-plx": "~3.x",
    "react-native-tcp-socket": "~6.x",

    "drizzle-orm": "~0.44.x",
    "@tanstack/react-query": "~5.x",
    "zustand": "~5.x",
    "axios": "~1.x",
    "zod": "~3.x"
  }
}
```

| Paket | Zweck |
|-------|-------|
| **expo-camera** | Foto- & Videoaufnahme, Blitzsteuerung |
| **expo-location** | GPS-Koordinaten, Hintergrund-Location |
| **expo-sqlite** | Lokale Offline-Datenbank (SQLCipher möglich) |
| **expo-file-system** | Lokale Fotospeicherung & -verwaltung |
| **expo-secure-store** | Sichere Token-Speicherung (Keychain/Keystore) |
| **@shopify/react-native-skia** | Canvas für Foto-Annotation (Pfeile, Maßlinien) |
| **react-native-ble-plx** | Bluetooth LE – Verbindung zu ext. GNSS-Empfängern (Phase 2) |
| **react-native-tcp-socket** | TCP-Socket für NTRIP-Client zu SAPOS (Phase 2) |
| **drizzle-orm** | Type-safe SQL Queries für SQLite |
| **@tanstack/react-query** | Server State, Caching, Sync-Hooks |
| **zustand** | Leichtgewichtiges Client State Management |
| **zod** | Runtime-Validierung (shared mit Backend-DTOs) |

### 5.4 Backend-Architektur (.NET 10 – DDD + Clean Architecture)

Die Backend-Architektur folgt dem Vorbild der Orange Car Rental Solution: Domain-Driven Design mit Clean Architecture, eigenem Dispatcher (kein MediatR/Wolverine), sealed record ValueObjects und strikter Trennung in Bounded Contexts.

**Designprinzipien:**
- Keine primitiven Typen im Domain Model – nur ValueObjects
- `sealed record` für ValueObjects mit Konstruktor-Properties
- Domain Events in Vergangenheitsform (`InstallationDocumented`, `PhotoUploaded`)
- Ein eigener leichtgewichtiger Dispatcher statt MediatR
- Aggregate Roots als Transaktionsgrenzen
- Eine Datei pro Klasse

```
BauDoku.Server/
├── src/
│   ├── BuildingBlocks/                         # Shared DDD Infrastructure
│   │   ├── BauDoku.BuildingBlocks.Domain/      # Basis-Klassen für DDD
│   │   │   ├── Entity.cs                       # abstract class Entity<TId>
│   │   │   ├── AggregateRoot.cs                # abstract class AggregateRoot<TId> : Entity<TId>
│   │   │   ├── ValueObject.cs                  # abstract record ValueObject
│   │   │   ├── IDomainEvent.cs                 # interface IDomainEvent
│   │   │   ├── IBusinessRule.cs                # interface IBusinessRule
│   │   │   └── BusinessRuleException.cs        # Exception bei Regelverletzung
│   │   │
│   │   ├── BauDoku.BuildingBlocks.Application/ # Shared Application Patterns
│   │   │   ├── Dispatcher/                     # Eigener CQRS Dispatcher
│   │   │   │   ├── IDispatcher.cs              # interface IDispatcher
│   │   │   │   ├── Dispatcher.cs               # DI-basierte Implementierung
│   │   │   │   ├── ICommand.cs                 # interface ICommand / ICommand<TResult>
│   │   │   │   ├── ICommandHandler.cs          # interface ICommandHandler<TCommand, TResult>
│   │   │   │   ├── IQuery.cs                   # interface IQuery<TResult>
│   │   │   │   ├── IQueryHandler.cs            # interface IQueryHandler<TQuery, TResult>
│   │   │   │   ├── IDomainEventHandler.cs      # interface IDomainEventHandler<TEvent>
│   │   │   │   └── DispatcherRegistration.cs   # IServiceCollection Extensions
│   │   │   ├── Behaviors/                      # Pipeline-Behaviors (Cross-Cutting)
│   │   │   │   ├── ValidationBehavior.cs       # FluentValidation vor Handler
│   │   │   │   ├── LoggingBehavior.cs          # Structured Logging
│   │   │   │   └── TransactionBehavior.cs      # Unit of Work / SaveChanges
│   │   │   └── IUnitOfWork.cs                  # interface IUnitOfWork
│   │   │
│   │   └── BauDoku.BuildingBlocks.Infrastructure/  # Shared Infra
│   │       ├── Persistence/
│   │       │   ├── BaseDbContext.cs             # EF Core Basis mit Domain Events
│   │       │   └── UnitOfWork.cs               # IUnitOfWork-Implementierung
│   │       ├── Auth/
│   │       │   └── KeycloakExtensions.cs
│   │       └── Messaging/
│   │           └── RabbitMqEventPublisher.cs
│   │
│   ├── Services/                               # Bounded Contexts
│   │   │
│   │   ├── Documentation/                      # BC: Installationsdokumentation
│   │   │   ├── BauDoku.Documentation.Domain/
│   │   │   │   ├── Aggregates/
│   │   │   │   │   ├── Installation/
│   │   │   │   │   │   ├── Installation.cs     # Aggregate Root
│   │   │   │   │   │   ├── InstallationId.cs   # sealed record : ValueObject
│   │   │   │   │   │   ├── InstallationType.cs # sealed record : ValueObject
│   │   │   │   │   │   └── InstallationStatus.cs
│   │   │   │   │   ├── Photo/
│   │   │   │   │   │   ├── Photo.cs            # Entity (gehört zu Installation)
│   │   │   │   │   │   ├── PhotoId.cs          # sealed record : ValueObject
│   │   │   │   │   │   └── PhotoType.cs        # sealed record : ValueObject
│   │   │   │   │   └── Measurement/
│   │   │   │   │       ├── Measurement.cs      # Entity
│   │   │   │   │       ├── MeasurementId.cs
│   │   │   │   │       ├── MeasurementType.cs
│   │   │   │   │       └── MeasurementValue.cs # sealed record (value + unit + passed)
│   │   │   │   ├── ValueObjects/
│   │   │   │   │   ├── GpsPosition.cs          # sealed record (lat, lng, accuracy, source, ...)
│   │   │   │   │   ├── CableSpecification.cs   # sealed record (type, crossSection, length)
│   │   │   │   │   ├── CircuitAssignment.cs    # sealed record (circuitId, fuseType, rating)
│   │   │   │   │   ├── Depth.cs                # sealed record (mm) mit Validierung
│   │   │   │   │   └── Annotation.cs           # sealed record (JSON arrows, lines, labels)
│   │   │   │   ├── Events/
│   │   │   │   │   ├── InstallationDocumented.cs       # IDomainEvent
│   │   │   │   │   ├── InstallationStatusChanged.cs
│   │   │   │   │   ├── PhotoAdded.cs
│   │   │   │   │   ├── MeasurementRecorded.cs
│   │   │   │   │   └── InstallationInspected.cs
│   │   │   │   ├── Rules/                      # Business Rules
│   │   │   │   │   ├── MeasurementMustHaveValidValue.cs
│   │   │   │   │   └── InstallationMustBeInZone.cs
│   │   │   │   └── Repositories/
│   │   │   │       └── IInstallationRepository.cs      # interface
│   │   │   │
│   │   │   ├── BauDoku.Documentation.Application/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── DocumentInstallation/
│   │   │   │   │   │   ├── DocumentInstallationCommand.cs
│   │   │   │   │   │   ├── DocumentInstallationHandler.cs
│   │   │   │   │   │   └── DocumentInstallationValidator.cs
│   │   │   │   │   ├── AddPhoto/
│   │   │   │   │   │   ├── AddPhotoCommand.cs
│   │   │   │   │   │   └── AddPhotoHandler.cs
│   │   │   │   │   ├── RecordMeasurement/
│   │   │   │   │   │   ├── RecordMeasurementCommand.cs
│   │   │   │   │   │   └── RecordMeasurementHandler.cs
│   │   │   │   │   └── ChangeInstallationStatus/
│   │   │   │   │       ├── ChangeInstallationStatusCommand.cs
│   │   │   │   │       └── ChangeInstallationStatusHandler.cs
│   │   │   │   ├── Queries/
│   │   │   │   │   ├── GetInstallation/
│   │   │   │   │   │   ├── GetInstallationQuery.cs
│   │   │   │   │   │   ├── GetInstallationHandler.cs
│   │   │   │   │   │   └── InstallationDto.cs
│   │   │   │   │   └── ListInstallationsByZone/
│   │   │   │   │       ├── ListInstallationsByZoneQuery.cs
│   │   │   │   │       └── ListInstallationsByZoneHandler.cs
│   │   │   │   └── EventHandlers/
│   │   │   │       └── InstallationDocumentedHandler.cs
│   │   │   │
│   │   │   ├── BauDoku.Documentation.Infrastructure/
│   │   │   │   ├── Persistence/
│   │   │   │   │   ├── DocumentationDbContext.cs
│   │   │   │   │   ├── Configurations/
│   │   │   │   │   │   ├── InstallationConfiguration.cs  # EF Core Fluent API
│   │   │   │   │   │   ├── PhotoConfiguration.cs
│   │   │   │   │   │   └── MeasurementConfiguration.cs
│   │   │   │   │   └── Repositories/
│   │   │   │   │       └── InstallationRepository.cs
│   │   │   │   └── Storage/
│   │   │   │       └── PhotoBlobStorage.cs
│   │   │   │
│   │   │   └── BauDoku.Documentation.Api/
│   │   │       ├── Endpoints/                  # Minimal API Endpoints
│   │   │       │   ├── InstallationEndpoints.cs
│   │   │       │   ├── PhotoEndpoints.cs
│   │   │       │   └── MeasurementEndpoints.cs
│   │   │       └── ServiceRegistration.cs
│   │   │
│   │   ├── Projects/                           # BC: Projektverwaltung
│   │   │   ├── BauDoku.Projects.Domain/
│   │   │   │   ├── Aggregates/
│   │   │   │   │   ├── Project/
│   │   │   │   │   │   ├── Project.cs          # Aggregate Root
│   │   │   │   │   │   ├── ProjectId.cs
│   │   │   │   │   │   ├── ProjectName.cs      # sealed record mit max length
│   │   │   │   │   │   └── ProjectStatus.cs
│   │   │   │   │   └── Zone/
│   │   │   │   │       ├── Zone.cs             # Entity (gehört zu Project)
│   │   │   │   │       ├── ZoneId.cs
│   │   │   │   │       ├── ZoneName.cs
│   │   │   │   │       └── ZoneType.cs         # building | floor | room | trench
│   │   │   │   ├── ValueObjects/
│   │   │   │   │   ├── Address.cs              # sealed record (street, city, zip)
│   │   │   │   │   └── ClientInfo.cs           # sealed record (name, contact)
│   │   │   │   ├── Events/
│   │   │   │   │   ├── ProjectCreated.cs
│   │   │   │   │   ├── ZoneAdded.cs
│   │   │   │   │   └── ProjectCompleted.cs
│   │   │   │   └── Repositories/
│   │   │   │       └── IProjectRepository.cs
│   │   │   ├── BauDoku.Projects.Application/
│   │   │   ├── BauDoku.Projects.Infrastructure/
│   │   │   └── BauDoku.Projects.Api/
│   │   │
│   │   └── Sync/                               # BC: Offline-Synchronisation
│   │       ├── BauDoku.Sync.Domain/
│   │       ├── BauDoku.Sync.Application/
│   │       │   ├── Commands/
│   │       │   │   ├── ProcessSyncBatch/
│   │       │   │   └── ResolveConflict/
│   │       │   └── Queries/
│   │       │       └── GetChangesSince/
│   │       ├── BauDoku.Sync.Infrastructure/
│   │       └── BauDoku.Sync.Api/
│   │
│   ├── ApiGateway/                             # YARP Reverse Proxy
│   │   └── BauDoku.ApiGateway/
│   │       └── Program.cs
│   │
│   └── AppHost/                                # .NET Aspire Orchestrierung
│       └── BauDoku.AppHost/
│           └── Program.cs
│
├── tests/
│   ├── BauDoku.Documentation.UnitTests/
│   ├── BauDoku.Documentation.IntegrationTests/
│   ├── BauDoku.Projects.UnitTests/
│   ├── BauDoku.Sync.IntegrationTests/
│   └── BauDoku.Api.Tests/
│
├── docker-compose.yml
├── Dockerfile
└── BauDoku.sln
```

### 5.5 BuildingBlocks: DDD Basis-Klassen

#### ValueObject (sealed record mit Konstruktor-Properties)

```csharp
// BuildingBlocks/BauDoku.BuildingBlocks.Domain/ValueObject.cs

namespace BauDoku.BuildingBlocks.Domain;

/// <summary>
/// Basis für alle ValueObjects. Verwendet sealed record mit
/// Konstruktor-Property-Definitionen statt positional records.
/// Equality und GetHashCode werden automatisch von record bereitgestellt.
/// </summary>
public abstract record ValueObject;
```

```csharp
// Beispiel: GpsPosition als sealed record mit Konstruktor-Properties
// Services/Documentation/BauDoku.Documentation.Domain/ValueObjects/GpsPosition.cs

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record GpsPosition : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }
    public double? Altitude { get; }
    public double HorizontalAccuracy { get; }
    public PositionSource Source { get; }
    public CorrectionService CorrectionService { get; }
    public RtkFixStatus RtkFixStatus { get; }
    public int? SatelliteCount { get; }
    public double? Hdop { get; }
    public double? CorrectionAge { get; }

    public GpsPosition(
        double latitude,
        double longitude,
        double horizontalAccuracy,
        PositionSource source,
        CorrectionService correctionService = CorrectionService.None,
        RtkFixStatus rtkFixStatus = RtkFixStatus.Autonomous,
        double? altitude = null,
        int? satelliteCount = null,
        double? hdop = null,
        double? correctionAge = null)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude));
        if (horizontalAccuracy < 0)
            throw new ArgumentOutOfRangeException(nameof(horizontalAccuracy));

        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        HorizontalAccuracy = horizontalAccuracy;
        Source = source;
        CorrectionService = correctionService;
        RtkFixStatus = rtkFixStatus;
        SatelliteCount = satelliteCount;
        Hdop = hdop;
        CorrectionAge = correctionAge;
    }

    public bool MeetsAccuracy(double requiredMeters) =>
        HorizontalAccuracy <= requiredMeters;
}

public sealed record PositionSource : ValueObject
{
    public static readonly PositionSource InternalGps = new("internal_gps");
    public static readonly PositionSource ExternalDgnss = new("external_dgnss");
    public static readonly PositionSource ExternalRtk = new("external_rtk");

    public string Value { get; }

    public PositionSource(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}

public sealed record CorrectionService : ValueObject
{
    public static readonly CorrectionService None = new("none");
    public static readonly CorrectionService SaposEps = new("sapos_eps");
    public static readonly CorrectionService SaposHeps = new("sapos_heps");
    public static readonly CorrectionService SaposGpps = new("sapos_gpps");

    public string Value { get; }

    public CorrectionService(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}

public sealed record RtkFixStatus : ValueObject
{
    public static readonly RtkFixStatus NoFix = new("no_fix");
    public static readonly RtkFixStatus Autonomous = new("autonomous");
    public static readonly RtkFixStatus Dgps = new("dgps");
    public static readonly RtkFixStatus RtkFloat = new("rtk_float");
    public static readonly RtkFixStatus RtkFixed = new("rtk_fixed");

    public string Value { get; }

    public RtkFixStatus(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
```

```csharp
// Weitere ValueObject-Beispiele

public sealed record CableSpecification : ValueObject
{
    public string CableType { get; }          // z.B. "NYM-J 5x2.5"
    public double CrossSectionMm2 { get; }
    public double LengthMeters { get; }

    public CableSpecification(string cableType, double crossSectionMm2, double lengthMeters)
    {
        if (string.IsNullOrWhiteSpace(cableType))
            throw new ArgumentException("Kabeltyp darf nicht leer sein.", nameof(cableType));
        if (crossSectionMm2 <= 0)
            throw new ArgumentOutOfRangeException(nameof(crossSectionMm2));
        if (lengthMeters <= 0)
            throw new ArgumentOutOfRangeException(nameof(lengthMeters));

        CableType = cableType;
        CrossSectionMm2 = crossSectionMm2;
        LengthMeters = lengthMeters;
    }
}

public sealed record Depth : ValueObject
{
    public int Millimeters { get; }

    public Depth(int millimeters)
    {
        if (millimeters < 0)
            throw new ArgumentOutOfRangeException(nameof(millimeters), "Tiefe darf nicht negativ sein.");

        Millimeters = millimeters;
    }

    public double ToMeters() => Millimeters / 1000.0;
    public double ToCentimeters() => Millimeters / 10.0;
}

public sealed record InstallationId : ValueObject
{
    public Guid Value { get; }

    public InstallationId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ID darf nicht leer sein.", nameof(value));

        Value = value;
    }

    public static InstallationId New() => new(Guid.NewGuid());
}
```

#### Entity und AggregateRoot

```csharp
// BuildingBlocks/BauDoku.BuildingBlocks.Domain/Entity.cs

namespace BauDoku.BuildingBlocks.Domain;

public abstract class Entity<TId> where TId : ValueObject
{
    public TId Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !Equals(left, right);
}
```

```csharp
// BuildingBlocks/BauDoku.BuildingBlocks.Domain/AggregateRoot.cs

namespace BauDoku.BuildingBlocks.Domain;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : ValueObject
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            throw new BusinessRuleException(rule);
    }
}
```

```csharp
// BuildingBlocks/BauDoku.BuildingBlocks.Domain/IDomainEvent.cs

namespace BauDoku.BuildingBlocks.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
```

```csharp
// BuildingBlocks/BauDoku.BuildingBlocks.Domain/IBusinessRule.cs

namespace BauDoku.BuildingBlocks.Domain;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
```

#### Eigener Dispatcher (Ersatz für MediatR)

```csharp
// BuildingBlocks/BauDoku.BuildingBlocks.Application/Dispatcher/IDispatcher.cs

namespace BauDoku.BuildingBlocks.Application.Dispatcher;

public interface IDispatcher
{
    Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken ct = default);
    Task Send(ICommand command, CancellationToken ct = default);
    Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken ct = default);
    Task Publish(IDomainEvent domainEvent, CancellationToken ct = default);
}
```

```csharp
// BuildingBlocks/BauDoku.BuildingBlocks.Application/Dispatcher/Dispatcher.cs

namespace BauDoku.BuildingBlocks.Application.Dispatcher;

/// <summary>
/// Leichtgewichtiger DI-basierter Dispatcher.
/// Löst Handler über IServiceProvider auf – kein Reflection zur Laufzeit,
/// kein Overhead wie bei MediatR.
/// </summary>
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public async Task<TResult> Send<TResult>(
        ICommand<TResult> command, CancellationToken ct = default)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResult));

        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        return await handler.Handle((dynamic)command, ct);
    }

    public async Task Send(ICommand command, CancellationToken ct = default)
    {
        var handlerType = typeof(ICommandHandler<>)
            .MakeGenericType(command.GetType());

        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        await handler.Handle((dynamic)command, ct);
    }

    public async Task<TResult> Query<TResult>(
        IQuery<TResult> query, CancellationToken ct = default)
    {
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));

        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        return await handler.Handle((dynamic)query, ct);
    }

    public async Task Publish(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        var handlerType = typeof(IDomainEventHandler<>)
            .MakeGenericType(domainEvent.GetType());

        var handlers = serviceProvider.GetServices(handlerType);

        foreach (dynamic handler in handlers)
        {
            await handler.Handle((dynamic)domainEvent, ct);
        }
    }
}
```

```csharp
// Interfaces

public interface ICommand;
public interface ICommand<TResult>;
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task Handle(TCommand command, CancellationToken ct);
}
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken ct);
}
public interface IQuery<TResult>;
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken ct);
}
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken ct);
}
```

```csharp
// DI-Registrierung: Assembly-Scanning für Handler

public static class DispatcherRegistration
{
    public static IServiceCollection AddDispatcher(
        this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<IDispatcher, Dispatcher>();

        foreach (var assembly in assemblies)
        {
            // Alle ICommandHandler<,>, IQueryHandler<,>, IDomainEventHandler<> registrieren
            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }

        return services;
    }
}
```

#### Aggregate Root Beispiel: Installation

```csharp
// Services/Documentation/BauDoku.Documentation.Domain/Aggregates/Installation/Installation.cs

namespace BauDoku.Documentation.Domain.Aggregates.Installation;

public sealed class Installation : AggregateRoot<InstallationId>
{
    public ZoneId ZoneId { get; private set; }
    public InstallationType Type { get; private set; }
    public InstallationStatus Status { get; private set; }

    // Positionierung (ValueObjects – keine Primitiven!)
    public GpsPosition? Position { get; private set; }
    public Depth? Depth { get; private set; }

    // Elektrische Daten (ValueObjects)
    public CableSpecification? Cable { get; private set; }
    public CircuitAssignment? Circuit { get; private set; }

    // Zugehörige Entities
    private readonly List<Photo> _photos = [];
    public IReadOnlyList<Photo> Photos => _photos.AsReadOnly();

    private readonly List<Measurement> _measurements = [];
    public IReadOnlyList<Measurement> Measurements => _measurements.AsReadOnly();

    // Meta
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Installation() { } // EF Core

    public static Installation Document(
        InstallationId id,
        ZoneId zoneId,
        InstallationType type,
        GpsPosition? position = null,
        CableSpecification? cable = null,
        CircuitAssignment? circuit = null,
        Depth? depth = null,
        string? notes = null)
    {
        var installation = new Installation
        {
            Id = id,
            ZoneId = zoneId,
            Type = type,
            Status = InstallationStatus.InProgress,
            Position = position,
            Cable = cable,
            Circuit = circuit,
            Depth = depth,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        installation.AddDomainEvent(new InstallationDocumented(
            id, zoneId, type, DateTime.UtcNow));

        return installation;
    }

    public void AddPhoto(PhotoId photoId, PhotoType type,
        string localPath, GpsPosition? position = null)
    {
        var photo = new Photo(photoId, Id, type, localPath, position);
        _photos.Add(photo);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PhotoAdded(Id, photoId, type, DateTime.UtcNow));
    }

    public void RecordMeasurement(MeasurementId measurementId,
        MeasurementType type, MeasurementValue value, string measuredBy)
    {
        var measurement = new Measurement(
            measurementId, Id, type, value, measuredBy);
        _measurements.Add(measurement);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MeasurementRecorded(
            Id, measurementId, type, value.Passed, DateTime.UtcNow));
    }

    public void ChangeStatus(InstallationStatus newStatus)
    {
        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new InstallationStatusChanged(
            Id, oldStatus, newStatus, DateTime.UtcNow));
    }

    public void UpdatePosition(GpsPosition position)
    {
        Position = position;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

#### Minimal API Endpoints (kein Controller)

```csharp
// Services/Documentation/BauDoku.Documentation.Api/Endpoints/InstallationEndpoints.cs

namespace BauDoku.Documentation.Api.Endpoints;

public static class InstallationEndpoints
{
    public static void MapInstallationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/installations")
            .WithTags("Installations")
            .RequireAuthorization();

        group.MapPost("/", async (
            DocumentInstallationCommand command,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var id = await dispatcher.Send<InstallationId>(command, ct);
            return Results.Created($"/api/installations/{id.Value}", id);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var result = await dispatcher.Query(
                new GetInstallationQuery(new InstallationId(id)), ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapPost("/{id:guid}/photos", async (
            Guid id,
            AddPhotoCommand command,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            await dispatcher.Send(command with
            {
                InstallationId = new InstallationId(id)
            }, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/measurements", async (
            Guid id,
            RecordMeasurementCommand command,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            await dispatcher.Send(command with
            {
                InstallationId = new InstallationId(id)
            }, ct);
            return Results.NoContent();
        });
    }
}
```

### 5.6 Backend Tech-Stack (.NET 10)

| Paket | Zweck |
|-------|-------|
| **.NET 10** | Runtime + Minimal API (kein ASP.NET MVC Controller) |
| **Entity Framework Core 10** | ORM + PostGIS für Geo-Queries |
| **FluentValidation** | Command-/Query-Validierung (via Dispatcher Pipeline) |
| **Polly** | Resilience (Retry, Circuit Breaker) |
| **Serilog** | Structured Logging (JSON → Seq/Grafana) |
| **Keycloak** | Auth (OAuth2 + OIDC) |
| **RabbitMQ** | Async Messaging zwischen Bounded Contexts |
| **Scrutor** | Assembly-Scanning für DI-Registrierung (Handler) |
| **.NET Aspire** | Lokale Orchestrierung (DB, Keycloak, RabbitMQ, Services) |
| **xUnit + FluentAssertions** | Unit- und Integration-Tests |

**Bewusst NICHT verwendet:**
- ~~MediatR~~ → Eigener Dispatcher (leichtgewichtig, kein Reflection-Overhead, volle Kontrolle)
- ~~Wolverine~~ → Zu viel Magie, schwer debugbar
- ~~MassTransit~~ → Overhead für aktuelle Projektgröße, RabbitMQ direkt reicht
- ~~ASP.NET MVC Controller~~ → Minimal API (weniger Boilerplate, besser für Microservices)

---

## 6. GPS/GNSS-Architektur & Vermessung

### 6.1 Problemstellung

Smartphone-GPS allein liefert Genauigkeiten von 3–10 Metern – das reicht für Navigation, aber nicht für Baustellendokumentation und Vermessung. Für die BauDoku App benötigen verschiedene Use Cases unterschiedliche Genauigkeitsstufen:

| Use Case | Benötigte Genauigkeit | Anforderung |
|----------|:---------------------:|-------------|
| Ungefähre Position auf Baustellenplan | 3–5 m | Internes Smartphone-GPS reicht |
| Kabeltrasse im Grundriss verorten | < 1 m | Korrekturdaten nötig |
| Verlegetiefe/-position dokumentieren | < 30 cm | SAPOS-EPS + geeigneter Empfänger |
| Aufmaß / Massenermittlung | < 10 cm | SAPOS-HEPS + RTK-Empfänger |
| Grenznahe Leitungsverlegung | < 3 cm | SAPOS-HEPS + RTK-Empfänger |

### 6.2 SAPOS – Satellitenpositionierungsdienst der deutschen Landesvermessung

SAPOS ist ein Gemeinschaftsprojekt der Vermessungsverwaltungen aller Bundesländer. Über 270 permanent betriebene GNSS-Referenzstationen decken ganz Deutschland ab. Die Korrekturdaten werden über das NTRIP-Protokoll (Networked Transport of RTCM via Internet Protocol) bereitgestellt.

**SAPOS-Dienste:**

| Dienst | Genauigkeit | Verfahren | Empfänger-Anforderung | Kosten |
|--------|:-----------:|-----------|----------------------|:------:|
| **EPS** (Echtzeit-Positionierung) | 30–80 cm | DGNSS-Codekorrektur | Einfrequenz DGNSS-fähig | Kostenlos* |
| **HEPS** (Hochpräzise Echtzeit) | 1–2 cm Lage, 2–3 cm Höhe | RTK (Trägerphasenkorrektur) | Mehrfrequenz RTK-fähig | Kostenlos* |
| **GPPS** (Geodätisches Postprocessing) | < 1 cm | Nachbearbeitung (RINEX) | Rohdatenaufzeichnung | Kostenlos* |

*In den meisten Bundesländern mittlerweile kostenlos (Bayern, NRW, Niedersachsen, BaWü, Brandenburg, u.a.). Registrierung erforderlich.

**NTRIP-Zugangsdaten (Beispiel Bayern):**
```
Host:       sapos.bayern.de
Port:       2101
Mountpoint: MSM_MAXO (für HEPS)
User/Pass:  nach Registrierung
Format:     RTCM 3.x
```

### 6.3 Plattform-Einschränkungen: iOS vs. Android

**iOS – Kein Zugriff auf GNSS-Rohdaten:**
Apple stellt über CoreLocation nur fertige Positionsdaten bereit – keine Satelliten-Rohdaten (Pseudoranges, Trägerphasen). Das bedeutet: SAPOS-Korrekturdaten können NICHT auf den internen iPhone-GPS-Chip angewandt werden. Das iPhone Dual-Frequency GPS (L1+L5, ab iPhone 14 Pro) liefert bestenfalls 1–3 m Genauigkeit, die softwareseitig nicht verbesserbar ist.

**Android – Raw GNSS Measurements API (ab Android 7):**
Android stellt seit 2017 über die `GnssMeasurement`-API Rohdaten bereit (Pseudoranges, Trägerphasen, Doppler). Theoretisch könnten NTRIP-Korrekturen direkt auf interne Chips angewandt werden. In der Praxis ist die Qualität der Smartphone-GNSS-Chips jedoch unzureichend: winzige Antennen, verrauschte Trägerphasen und Multipath-Probleme begrenzen die Genauigkeit auf ca. 0,5–2 m – selbst mit Korrekturen. Die versprochene Sub-Meter-Genauigkeit (30 cm) der internen Chips hat sich in der Praxis als nicht zuverlässig erwiesen.

**Konsequenz für BauDoku:**
Für Genauigkeiten < 1 m ist auf BEIDEN Plattformen ein externer GNSS-Empfänger erforderlich, der per Bluetooth oder USB angebunden wird.

### 6.4 Dreistufiges Positionierungskonzept

```
┌───────────────────────────────────────────────────────────────────────┐
│                     POSITIONSQUELLEN-ARCHITEKTUR                       │
│                                                                       │
│  Stufe 1: Internes Smartphone-GPS (Standardmodus)                    │
│  ─────────────────────────────────────────────────                    │
│  Genauigkeit:  3–10 m                                                │
│  Zusatzgerät:  Keines                                                │
│  Use Case:     Grobe Verortung auf Baustellenplan                    │
│  Verfügbar:    iOS + Android, immer                                  │
│  API:          expo-location (CLLocation / FusedLocationProvider)     │
│                                                                       │
│  Stufe 2: Externer DGNSS-Empfänger + SAPOS-EPS                      │
│  ──────────────────────────────────────────────                      │
│  Genauigkeit:  30–50 cm (Zielbereich für BauDoku MVP)                │
│  Zusatzgerät:  DGNSS Bluetooth-Receiver (~100–300 €)                 │
│  Use Case:     Positionsdokumentation, Kabeltrassen verorten         │
│  Verfügbar:    iOS + Android via Bluetooth (NMEA-Protokoll)          │
│  Beispiele:    Columbus P-7 Pro, Bad Elf GNSS Surveyor               │
│  SAPOS-Dienst: EPS (Echtzeit-Positionierung, Codekorrektur)         │
│                                                                       │
│  Stufe 3: Externer RTK-Empfänger + SAPOS-HEPS                       │
│  ─────────────────────────────────────────────                       │
│  Genauigkeit:  1–3 cm                                                │
│  Zusatzgerät:  RTK GNSS-Receiver (~270–1.500 €)                     │
│  Use Case:     Professionelle Vermessung, Massenermittlung, Aufmaß   │
│  Verfügbar:    iOS + Android via Bluetooth (NMEA-Protokoll)          │
│  Beispiele:    ArduSimple RTK Handheld (~270 €),                     │
│                Emlid Reach RX2 (~1.500 €),                           │
│                CHCNAV LT60H (Rugged Phone mit RTK, ~2.000 €+)       │
│  SAPOS-Dienst: HEPS (Trägerphasenkorrektur, RTK)                    │
│                                                                       │
└───────────────────────────────────────────────────────────────────────┘
```

### 6.5 Software-Architektur: Positionsquellen-Abstraktion

```typescript
// src/gnss/types.ts

/** Quelle der Positionsdaten */
type PositionSource =
  | 'internal_gps'            // Internes Smartphone-GPS (~3-10 m)
  | 'external_dgnss'          // Ext. DGNSS + SAPOS-EPS (~30-50 cm)
  | 'external_rtk';           // Ext. RTK + SAPOS-HEPS (~1-3 cm)

/** Differentialkorrektur-Dienst */
type CorrectionService =
  | 'none'                    // Keine Korrektur
  | 'sapos_eps'               // SAPOS EPS (Dezimeter)
  | 'sapos_heps'              // SAPOS HEPS (Zentimeter)
  | 'sapos_gpps';             // SAPOS GPPS (Post-Processing)

/** RTK-Fixierungsstatus */
type RtkFixStatus =
  | 'no_fix'                  // Keine Position
  | 'autonomous'              // Standalone (keine Korrektur)
  | 'dgps'                    // Differentiell korrigiert
  | 'rtk_float'               // RTK Float (Dezimeter)
  | 'rtk_fixed';              // RTK Fixed (Zentimeter)

/** Position mit vollständigen Metadaten */
interface GeoPosition {
  latitude: number;           // WGS84
  longitude: number;          // WGS84
  altitude?: number;          // Meter über Ellipsoid
  altitudeMsl?: number;       // Meter über Normalnull (NHN)

  // Genauigkeit
  horizontalAccuracy: number; // geschätzte Lagegenauigkeit in Metern
  verticalAccuracy?: number;  // geschätzte Höhengenauigkeit in Metern
  hdop?: number;              // Horizontal Dilution of Precision
  pdop?: number;              // Position Dilution of Precision

  // Quelle & Korrektur
  source: PositionSource;
  correctionService: CorrectionService;
  rtkFixStatus: RtkFixStatus;
  satelliteCount?: number;    // Anzahl verwendeter Satelliten

  // SAPOS-spezifisch
  correctionAge?: number;     // Alter der Korrekturdaten in Sekunden
  baseStationId?: string;     // ID der SAPOS-Referenzstation

  // Meta
  timestamp: Date;
  deviceId: string;
}
```

```typescript
// src/gnss/PositionProvider.ts

interface PositionProvider {
  /** Aktuelle Position abfragen */
  getPosition(): Promise<GeoPosition>;

  /** Positionsstream starten (für kontinuierliche Aufnahme) */
  startTracking(onPosition: (pos: GeoPosition) => void): () => void;

  /** Aktuelle Genauigkeitsstufe */
  getAccuracyLevel(): PositionSource;

  /** NTRIP-Verbindung aktiv? */
  isNtripConnected(): boolean;
}
```

```typescript
// src/gnss/InternalGpsProvider.ts
// Stufe 1: Internes GPS via expo-location

import * as Location from 'expo-location';

class InternalGpsProvider implements PositionProvider {
  async getPosition(): Promise<GeoPosition> {
    const loc = await Location.getCurrentPositionAsync({
      accuracy: Location.Accuracy.BestForNavigation,
    });

    return {
      latitude: loc.coords.latitude,
      longitude: loc.coords.longitude,
      altitude: loc.coords.altitude ?? undefined,
      horizontalAccuracy: loc.coords.accuracy ?? 10,
      verticalAccuracy: loc.coords.altitudeAccuracy ?? undefined,
      source: 'internal_gps',
      correctionService: 'none',
      rtkFixStatus: 'autonomous',
      timestamp: new Date(loc.timestamp),
      deviceId: getDeviceId(),
    };
  }
}
```

```typescript
// src/gnss/ExternalGnssProvider.ts
// Stufe 2+3: Externer GNSS-Empfänger via Bluetooth (NMEA)

class ExternalGnssProvider implements PositionProvider {
  private bleConnection: BleConnection;
  private nmeaParser: NmeaParser;
  private ntripClient?: NtripClient;

  constructor(config: {
    receiverAddress: string;    // Bluetooth-Adresse des Receivers
    ntripConfig?: NtripConfig;  // SAPOS-Zugangsdaten
  }) {
    this.bleConnection = new BleConnection(config.receiverAddress);
    this.nmeaParser = new NmeaParser();

    if (config.ntripConfig) {
      this.ntripClient = new NtripClient(config.ntripConfig);
    }
  }

  async connect(): Promise<void> {
    // 1. Bluetooth-Verbindung zum GNSS-Receiver
    await this.bleConnection.connect();

    // 2. NTRIP-Verbindung zu SAPOS (falls konfiguriert)
    if (this.ntripClient) {
      await this.ntripClient.connect();
      // Korrekturdaten zum Receiver weiterleiten
      this.ntripClient.onData((rtcm) => {
        this.bleConnection.send(rtcm);
      });
    }

    // 3. NMEA-Daten vom Receiver empfangen
    this.bleConnection.onData((data) => {
      this.nmeaParser.parse(data);
    });
  }

  async getPosition(): Promise<GeoPosition> {
    const nmea = this.nmeaParser.getLatestGGA();

    return {
      latitude: nmea.latitude,
      longitude: nmea.longitude,
      altitude: nmea.altitude,
      altitudeMsl: nmea.geoidalSeparation
        ? nmea.altitude - nmea.geoidalSeparation
        : undefined,
      horizontalAccuracy: this.estimateAccuracy(nmea),
      hdop: nmea.hdop,
      source: nmea.fixQuality >= 4 ? 'external_rtk' : 'external_dgnss',
      correctionService: this.determineCorrectionService(nmea),
      rtkFixStatus: this.mapFixQuality(nmea.fixQuality),
      satelliteCount: nmea.satellitesUsed,
      correctionAge: nmea.correctionAge,
      baseStationId: nmea.referenceStationId,
      timestamp: new Date(),
      deviceId: getDeviceId(),
    };
  }

  /** NMEA Fix Quality → RTK Status Mapping */
  private mapFixQuality(quality: number): RtkFixStatus {
    switch (quality) {
      case 0: return 'no_fix';
      case 1: return 'autonomous';
      case 2: return 'dgps';           // SAPOS-EPS
      case 4: return 'rtk_fixed';      // SAPOS-HEPS (Fixed)
      case 5: return 'rtk_float';      // SAPOS-HEPS (Float)
      default: return 'autonomous';
    }
  }
}
```

```typescript
// src/gnss/NtripClient.ts
// NTRIP-Client für SAPOS-Korrekturdaten

interface NtripConfig {
  host: string;               // z.B. "sapos.bayern.de"
  port: number;               // z.B. 2101
  mountpoint: string;         // z.B. "MSM_MAXO" (HEPS) oder "VRS_3_4G_DE" (EPS)
  username: string;
  password: string;
}

class NtripClient {
  private socket: TcpSocket | null = null;
  private config: NtripConfig;
  private callbacks: ((data: Uint8Array) => void)[] = [];

  constructor(config: NtripConfig) {
    this.config = config;
  }

  async connect(): Promise<void> {
    // NTRIP v1.0 HTTP-basierte Verbindung
    const request = [
      `GET /${this.config.mountpoint} HTTP/1.0`,
      `Host: ${this.config.host}`,
      `Ntrip-Version: Ntrip/1.0`,
      `User-Agent: BauDoku/1.0`,
      `Authorization: Basic ${btoa(
        `${this.config.username}:${this.config.password}`
      )}`,
      '', ''
    ].join('\r\n');

    this.socket = new TcpSocket();
    await this.socket.connect(this.config.host, this.config.port);
    await this.socket.send(request);

    // RTCM3-Korrekturdaten empfangen und weiterleiten
    this.socket.onData((data: Uint8Array) => {
      this.callbacks.forEach(cb => cb(data));
    });
  }

  /** Position an SAPOS senden (für VRS – Virtuelle Referenzstation) */
  async sendPosition(lat: number, lng: number): Promise<void> {
    const gga = this.buildGGA(lat, lng);
    await this.socket?.send(gga);
  }

  onData(callback: (data: Uint8Array) => void): void {
    this.callbacks.push(callback);
  }
}
```

```typescript
// src/gnss/PositionManager.ts
// Orchestriert die Positionsquellen – wählt automatisch die beste verfügbare

class PositionManager {
  private internalProvider: InternalGpsProvider;
  private externalProvider?: ExternalGnssProvider;

  /** Beste verfügbare Position abrufen */
  async getPosition(): Promise<GeoPosition> {
    // Externer Empfänger hat Vorrang (falls verbunden)
    if (this.externalProvider?.isConnected()) {
      const pos = await this.externalProvider.getPosition();
      // Plausibilitätsprüfung: Position nicht älter als 5s
      if (Date.now() - pos.timestamp.getTime() < 5000) {
        return pos;
      }
    }

    // Fallback: Internes GPS
    return this.internalProvider.getPosition();
  }

  /** Prüft ob aktuelle Genauigkeit für Use Case ausreicht */
  meetsAccuracyRequirement(
    position: GeoPosition,
    requiredAccuracyM: number
  ): boolean {
    return position.horizontalAccuracy <= requiredAccuracyM;
  }
}
```

### 6.6 Android Mock Location (Alternative zu direkter BLE-Anbindung)

Auf Android gibt es eine elegante Alternative zur direkten Bluetooth-Anbindung in der App: Mock Location. Dabei übernimmt eine Companion-App (z.B. GNSS Master, Lefebure NTRIP) die Kommunikation mit dem externen GNSS-Empfänger und speist die korrigierten Koordinaten direkt ins Android-Betriebssystem ein. Die BauDoku App erhält dann über `expo-location` automatisch die hochgenauen Koordinaten, ohne selbst Bluetooth oder NTRIP implementieren zu müssen.

```
┌─────────────────────────────────────────────────────────────┐
│  Variante A: Direkte Anbindung (empfohlen für Produktion)    │
│                                                              │
│  BauDoku App                                                 │
│  ├── Bluetooth-Verbindung → Ext. GNSS-Receiver              │
│  ├── NTRIP-Client → SAPOS-Korrekturdaten                    │
│  └── NMEA-Parser → Korrigierte Position                     │
│                                                              │
│  Vorteil: Alles in einer App, kein Setup nötig               │
│  Nachteil: Höherer Entwicklungsaufwand                       │
├──────────────────────────────────────────────────────────────┤
│  Variante B: Mock Location (pragmatisch für MVP)             │
│                                                              │
│  GNSS Master App (3rd-Party)                                 │
│  ├── Bluetooth → Ext. GNSS-Receiver                          │
│  ├── NTRIP-Client → SAPOS                                    │
│  └── Mock Location → Android OS                              │
│           │                                                   │
│           ▼                                                   │
│  BauDoku App                                                  │
│  └── expo-location → Erhält automatisch korrigierte Position │
│                                                              │
│  Vorteil: Kein BLE/NTRIP-Code nötig, schneller MVP           │
│  Nachteil: Nur Android, extra App nötig, Developer Mode      │
└─────────────────────────────────────────────────────────────┘
```

**Empfohlene Strategie:**
- Phase 1 (MVP): Mock Location auf Android, expo-location auf iOS (internes GPS)
- Phase 2: Eigener NTRIP-Client + BLE-Anbindung in BauDoku (iOS + Android)
- Phase 3: RTK-Support, SAPOS-HEPS, Massenermittlung

### 6.7 NTRIP-Einstellungen in der App (Phase 2+)

```typescript
// src/gnss/config.ts
// SAPOS-Konfiguration für deutsche Bundesländer

const SAPOS_PRESETS: Record<string, NtripConfig> = {
  bayern: {
    host: 'sapos.bayern.de',
    port: 2101,
    mountpoint: 'MSM_MAXO',       // HEPS
    username: '',                   // Vom User bei Registrierung erhalten
    password: '',
  },
  nrw: {
    host: 'sapos.nrw.de',
    port: 2101,
    mountpoint: 'VRS_3_4G_DE',    // HEPS
    username: '',
    password: '',
  },
  niedersachsen: {
    host: 'www.sapos-ni-ntrip.de',
    port: 2101,
    mountpoint: 'VRS_3_4G_DE',
    username: '',
    password: '',
  },
  brandenburg: {
    host: 'www.sapos-bb-ntrip.de',
    port: 2101,
    mountpoint: 'VRS_3_4G_DE',
    username: '',
    password: '',
  },
  // ... weitere Bundesländer
};
```

### 6.8 Qualitätsanzeige in der UI

Die App zeigt dem Benutzer jederzeit die aktuelle Positionsqualität an:

```
┌─────────────────────────────────────────┐
│  📍 Positionsstatus                      │
│                                          │
│  ●●●●○ RTK Fixed – SAPOS HEPS          │
│  Genauigkeit: 0.02 m                    │
│  Satelliten: 18 | HDOP: 0.8            │
│  Korrekturalter: 2s | Ref: BY-0815     │
│                                          │
│  ● Grün:  RTK Fixed (< 5 cm)           │
│  ● Gelb:  DGPS / RTK Float (< 1 m)    │
│  ● Orange: Autonom (1–5 m)             │
│  ● Rot:   Kein Fix / > 5 m             │
├─────────────────────────────────────────┤
│  Warnung wenn Genauigkeit > Schwellwert: │
│  ⚠️ "Genauigkeit unzureichend für       │
│      Vermessung (aktuell 3.2 m).        │
│      Externen Empfänger verbinden?"      │
└─────────────────────────────────────────┘
```

### 6.9 Empfohlene externe GNSS-Hardware

| Gerät | Genauigkeit | Preis | Anbindung | Geeignet für |
|-------|:-----------:|:-----:|:---------:|:-------------|
| **Columbus P-7 Pro** | ~50 cm (L1+L5 Dual-Freq) | ~150 € | Bluetooth + USB | Stufe 2, EPS |
| **ArduSimple RTK Handheld** | ~2 cm (RTK) | ~270–470 € | USB-C / Bluetooth | Stufe 3, HEPS |
| **Emlid Reach RX2** | ~2 cm (RTK, Neigungskompensation) | ~1.500 € | Bluetooth | Stufe 3, HEPS (Profi) |
| **CHCNAV LT60H** | ~2 cm (RTK) | ~2.000 €+ | Eingebaut (Rugged Phone) | Stufe 3, All-in-One |

Für den **MVP mit Zielgenauigkeit ~30 cm** genügt ein günstiger DGNSS-Empfänger (~150 €) zusammen mit SAPOS-EPS. Für spätere Vermessungs-Features (Massenermittlung, Aufmaß) wird ein RTK-Empfänger (~270–470 €) mit SAPOS-HEPS empfohlen.

### 6.10 React Native Bibliotheken für GNSS

| Paket | Zweck | Phase |
|-------|-------|:-----:|
| `expo-location` | Internes GPS, Berechtigungen, Hintergrund-Location | MVP |
| `react-native-ble-plx` | Bluetooth Low Energy – Verbindung zu ext. GNSS-Empfänger | 2 |
| `react-native-tcp-socket` | TCP-Socket für NTRIP-Client | 2 |
| Eigene Lib: `NmeaParser` | NMEA 0183 Parsing (GGA, RMC, GSA, GSV) | 2 |
| Eigene Lib: `NtripClient` | NTRIP v1.0/v2.0 Client für SAPOS | 2 |

---

## 7. Offline-First Architektur (Kernstück)

### 7.1 Sync-Strategie: Outbox Pattern

```
┌────────────────────── Mobile App ──────────────────────┐
│                                                         │
│  User Action                                            │
│      │                                                  │
│      ▼                                                  │
│  ┌─────────────┐   ┌──────────────┐                    │
│  │ Zustand      │──▶│ SQLite DB    │                    │
│  │ (UI State)   │   │ (Drizzle)    │                    │
│  └─────────────┘   └──────┬───────┘                    │
│                            │                            │
│                            ▼                            │
│                    ┌───────────────┐                    │
│                    │  Change Log   │ (Outbox-Tabelle)   │
│                    │               │                    │
│                    │  id: uuid     │                    │
│                    │  entity: str  │                    │
│                    │  entityId: id │                    │
│                    │  operation:   │                    │
│                    │    C/U/D      │                    │
│                    │  payload: json│                    │
│                    │  timestamp    │                    │
│                    │  deviceId     │                    │
│                    │  version: int │                    │
│                    │  synced: bool │                    │
│                    └───────┬───────┘                    │
│                            │                            │
│                            ▼                            │
│                    ┌───────────────┐                    │
│                    │ Sync Manager  │                    │
│                    │               │                    │
│                    │ 1. Connectivity?                   │
│                    │ 2. Push Outbox → Server            │
│                    │ 3. Pull Changes ← Server           │
│                    │ 4. Resolve Conflicts               │
│                    │ 5. Mark synced                     │
│                    └───────┬───────┘                    │
│                            │                            │
└────────────────────────────┼────────────────────────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │  ASP.NET Core   │
                    │  Sync Endpoint  │
                    │                 │
                    │  POST /sync     │
                    │  GET  /sync     │
                    │    ?since=ts    │
                    └─────────────────┘
```

### 7.2 Sync-Flow im Detail

```typescript
// SyncManager.ts – Vereinfachte Darstellung
class SyncManager {
  private running = false;
  private queued = false;

  async requestSync(): Promise<void> {
    if (this.running) { this.queued = true; return; }

    const isOnline = await NetInfo.fetch();
    if (!isOnline.isConnected) return;

    this.running = true;
    try {
      // 1. Push: Lokale Änderungen hochladen
      const pending = await syncRepo.getUnsynced();
      if (pending.length > 0) {
        await api.sync.pushChanges(pending);
        await syncRepo.markSynced(pending.map(p => p.id));
      }

      // 2. Pull: Server-Änderungen holen
      const lastSync = await syncRepo.getLastSyncTimestamp();
      const serverChanges = await api.sync.pullChanges(lastSync);

      // 3. Merge: Konflikte auflösen (Field-Level)
      for (const change of serverChanges) {
        await conflictStrategy.apply(change);
      }

      // 4. Fotos separat uploaden (Chunked)
      await photoUploader.uploadPending();

    } finally {
      this.running = false;
      if (this.queued) { this.queued = false; await this.requestSync(); }
    }
  }
}
```

### 7.3 Foto-Upload: Chunked mit Resume

```
Foto (8 MB)
    │
    ├── Chunk 1 (1 MB) ──→ POST /photos/upload/init
    ├── Chunk 2 (1 MB) ──→ PUT  /photos/upload/{id}/chunk/2
    ├── Chunk 3 (1 MB) ──→ PUT  /photos/upload/{id}/chunk/3
    │   ⚡ Verbindung bricht ab
    │   ... Minuten später ...
    ├── Chunk 3 (retry) ──→ PUT  /photos/upload/{id}/chunk/3
    ├── Chunk 4-8        ──→ PUT  /photos/upload/{id}/chunk/4..8
    └── Finalize          ──→ POST /photos/upload/{id}/complete
```

### 7.4 Konfliktauflösung

| Strategie | Wann | Beispiel |
|-----------|------|---------|
| **Last-Writer-Wins (Feld-Level)** | Standard für die meisten Felder | Zwei User ändern Notizen → neuerer Timestamp gewinnt |
| **Merge** | Für Listen/Collections | Fotos hinzufügen → beide Fotos behalten |
| **Manuell** | Bei kritischen Widersprüchen | Status "Abgenommen" vs. "Nacharbeit" → User entscheidet |

---

## 8. Datenmodell

### 8.1 SQLite Schema (Expo App – Drizzle)

```typescript
// db/schema.ts
import { sqliteTable, text, integer, real, blob } from 'drizzle-orm/sqlite-core';

export const projects = sqliteTable('projects', {
  id:          text('id').primaryKey(),            // UUID
  name:        text('name').notNull(),
  address:     text('address'),
  gpsLat:      real('gps_lat'),
  gpsLng:      real('gps_lng'),
  client:      text('client'),                     // Auftraggeber
  status:      text('status').notNull(),           // active | completed | archived
  createdBy:   text('created_by').notNull(),
  createdAt:   integer('created_at', { mode: 'timestamp' }).notNull(),
  updatedAt:   integer('updated_at', { mode: 'timestamp' }).notNull(),
  version:     integer('version').notNull().default(1),
});

export const zones = sqliteTable('zones', {
  id:           text('id').primaryKey(),
  projectId:    text('project_id').notNull().references(() => projects.id),
  parentZoneId: text('parent_zone_id'),            // Hierarchisch
  name:         text('name').notNull(),
  type:         text('type').notNull(),            // building | floor | room | section | trench
  qrCode:       text('qr_code'),
  sortOrder:    integer('sort_order').default(0),
  version:      integer('version').notNull().default(1),
});

export const installations = sqliteTable('installations', {
  id:           text('id').primaryKey(),
  zoneId:       text('zone_id').notNull().references(() => zones.id),
  type:         text('type').notNull(),            // cable | junction | panel | socket | switch | ...
  status:       text('status').notNull(),          // planned | in_progress | completed | inspected

  // Positionierung & GNSS (siehe Abschnitt 6: GPS/GNSS-Architektur)
  gpsLat:            real('gps_lat'),
  gpsLng:            real('gps_lng'),
  gpsAltitude:       real('gps_altitude'),           // Meter über Ellipsoid (WGS84)
  gpsAltitudeMsl:    real('gps_altitude_msl'),       // Meter über Normalnull (NHN)
  gpsAccuracy:       real('gps_accuracy'),           // Geschätzte Lagegenauigkeit in Metern
  gpsSource:         text('gps_source'),             // internal_gps | external_dgnss | external_rtk
  gpsCorrService:    text('gps_corr_service'),       // none | sapos_eps | sapos_heps | sapos_gpps
  gpsRtkStatus:      text('gps_rtk_status'),         // no_fix | autonomous | dgps | rtk_float | rtk_fixed
  gpsSatCount:       integer('gps_sat_count'),        // Anzahl Satelliten
  gpsHdop:           real('gps_hdop'),               // Horizontal DOP
  gpsCorrAge:        real('gps_corr_age'),           // Korrekturalter in Sekunden
  depthMm:           integer('depth_mm'),             // Verlegetiefe
  positionOnPlan:    text('position_on_plan'),        // JSON {x, y}

  // Komponente
  manufacturer:    text('manufacturer'),
  model:           text('model'),
  serialNumber:    text('serial_number'),
  cableType:       text('cable_type'),             // z.B. "NYM-J 5x2.5"
  crossSectionMm2: real('cross_section_mm2'),
  lengthM:         real('length_m'),

  // Elektrische Daten
  circuitId:    text('circuit_id'),                // Stromkreis
  fuseType:     text('fuse_type'),
  fuseRatingA:  real('fuse_rating_a'),
  voltageV:     integer('voltage_v'),
  phase:        text('phase'),                     // L1 | L2 | L3 | N | PE

  // Meta
  notes:        text('notes'),
  installedBy:  text('installed_by'),
  installedAt:  integer('installed_at', { mode: 'timestamp' }),
  inspectedBy:  text('inspected_by'),
  inspectedAt:  integer('inspected_at', { mode: 'timestamp' }),
  createdAt:    integer('created_at', { mode: 'timestamp' }).notNull(),
  updatedAt:    integer('updated_at', { mode: 'timestamp' }).notNull(),
  version:      integer('version').notNull().default(1),
});

export const photos = sqliteTable('photos', {
  id:             text('id').primaryKey(),
  installationId: text('installation_id').notNull().references(() => installations.id),
  localPath:      text('local_path').notNull(),    // Lokaler Dateipfad
  remotePath:     text('remote_path'),             // Blob Storage URL (nach Sync)
  thumbnailPath:  text('thumbnail_path'),
  type:           text('type').notNull(),          // before | after | detail | overview
  annotations:    text('annotations'),             // JSON (Pfeile, Maßlinien, Labels)
  gpsLat:         real('gps_lat'),
  gpsLng:         real('gps_lng'),
  gpsAccuracy:    real('gps_accuracy'),           // Genauigkeit in Metern
  gpsSource:      text('gps_source'),             // internal_gps | external_dgnss | external_rtk
  gpsCorrService: text('gps_corr_service'),       // none | sapos_eps | sapos_heps
  takenAt:        integer('taken_at', { mode: 'timestamp' }).notNull(),
  uploadStatus:   text('upload_status').notNull(), // pending | uploading | uploaded | failed
  version:        integer('version').notNull().default(1),
});

export const measurements = sqliteTable('measurements', {
  id:             text('id').primaryKey(),
  installationId: text('installation_id').notNull().references(() => installations.id),
  type:           text('type').notNull(),          // insulation_resistance | loop_impedance | rcd | ...
  value:          real('value').notNull(),
  unit:           text('unit').notNull(),           // MΩ | Ω | ms | ...
  passed:         integer('passed', { mode: 'boolean' }),
  measuredAt:     integer('measured_at', { mode: 'timestamp' }).notNull(),
  measuredBy:     text('measured_by').notNull(),
  version:        integer('version').notNull().default(1),
});

// Sync Outbox
export const syncOutbox = sqliteTable('sync_outbox', {
  id:         text('id').primaryKey(),
  entityType: text('entity_type').notNull(),       // project | zone | installation | photo | measurement
  entityId:   text('entity_id').notNull(),
  operation:  text('operation').notNull(),          // create | update | delete
  payload:    text('payload').notNull(),            // JSON
  timestamp:  integer('timestamp', { mode: 'timestamp' }).notNull(),
  deviceId:   text('device_id').notNull(),
  retryCount: integer('retry_count').default(0),
  synced:     integer('synced', { mode: 'boolean' }).default(false),
});

export const syncMeta = sqliteTable('sync_meta', {
  key:   text('key').primaryKey(),
  value: text('value').notNull(),
});
```

### 8.2 PostgreSQL Schema (Backend – EF Core 10 mit DDD)

Das Backend-Schema spiegelt das SQLite-Schema, erweitert um:
- PostGIS `geography`-Spalten für Geo-Queries (räumliche Indizes auf GPS-Positionen)
- GNSS-Metadaten: Genauigkeit, Quelle, Korrekturdienst, RTK-Status pro Position
- Mandanten-ID (Tenant) für Multi-Tenancy
- Audit-Trail (created_by, updated_by, deleted_at für Soft-Delete)
- Volltextsuche über `tsvector`-Spalten
- Spatial Queries: "Alle Installationen im Umkreis von X Metern" via PostGIS `ST_DWithin`

**ValueObject-Persistierung:**
- EF Core Owned Types für komplexe ValueObjects (`GpsPosition`, `CableSpecification`)
- Value Conversions für einfache sealed record ValueObjects (`InstallationId` → `Guid`, `Depth` → `int`)
- Pro Bounded Context ein eigenes `DbContext` mit separatem Migrationspfad

---

## 9. API-Design

### 9.1 REST Endpoints (Minimal API pro Bounded Context)

Jeder Bounded Context registriert seine Endpoints als `MapGroup()` Extension Methods. Kein MVC, keine Controller – nur statische `MapXxxEndpoints()`-Methoden.

```
Auth
  POST   /api/auth/login
  POST   /api/auth/refresh

Projects
  GET    /api/projects                    # Liste (paginiert, filterbar)
  POST   /api/projects                    # Neues Projekt
  GET    /api/projects/{id}               # Detail
  PUT    /api/projects/{id}               # Update
  DELETE /api/projects/{id}               # Soft-Delete

Zones
  GET    /api/projects/{id}/zones         # Alle Zonen eines Projekts
  POST   /api/projects/{id}/zones
  PUT    /api/zones/{id}
  DELETE /api/zones/{id}

Installations
  GET    /api/zones/{id}/installations    # Installationen in Zone
  POST   /api/zones/{id}/installations
  GET    /api/installations/{id}          # Detail mit Fotos & Messungen
  PUT    /api/installations/{id}
  DELETE /api/installations/{id}

Photos
  POST   /api/photos/upload/init          # Chunked Upload starten
  PUT    /api/photos/upload/{id}/chunk/{n}# Chunk hochladen
  POST   /api/photos/upload/{id}/complete # Upload abschließen
  GET    /api/photos/{id}                 # Metadaten
  GET    /api/photos/{id}/image           # Bilddatei
  GET    /api/photos/{id}/thumbnail       # Thumbnail

Measurements
  GET    /api/installations/{id}/measurements
  POST   /api/installations/{id}/measurements
  PUT    /api/measurements/{id}

Sync
  POST   /api/sync/push                   # Client → Server (Batch)
  GET    /api/sync/pull?since={timestamp} # Server → Client (Delta)
  GET    /api/sync/status                 # Sync-Status des Clients

Reports
  POST   /api/reports/generate            # PDF erstellen
  GET    /api/reports/{id}                # PDF herunterladen
```

### 9.2 Sync-API: Push Request

```json
POST /api/sync/push
{
  "deviceId": "device-abc-123",
  "changes": [
    {
      "id": "change-uuid-1",
      "entityType": "installation",
      "entityId": "inst-uuid-1",
      "operation": "update",
      "timestamp": "2026-02-10T14:32:00Z",
      "version": 3,
      "payload": {
        "status": "completed",
        "notes": "Kabel verlegt, 3m Tiefe"
      }
    },
    {
      "id": "change-uuid-2",
      "entityType": "measurement",
      "entityId": "meas-uuid-1",
      "operation": "create",
      "timestamp": "2026-02-10T14:35:00Z",
      "version": 1,
      "payload": {
        "installationId": "inst-uuid-1",
        "type": "insulation_resistance",
        "value": 245.5,
        "unit": "MΩ",
        "passed": true,
        "measuredBy": "user-heiko"
      }
    }
  ]
}
```

### 9.3 Sync-API: Pull Response

```json
GET /api/sync/pull?since=2026-02-10T12:00:00Z
{
  "changes": [...],
  "conflicts": [
    {
      "entityType": "installation",
      "entityId": "inst-uuid-5",
      "field": "status",
      "serverValue": "inspected",
      "clientValue": "completed",
      "serverTimestamp": "2026-02-10T14:30:00Z",
      "clientTimestamp": "2026-02-10T14:28:00Z",
      "resolution": "server_wins"
    }
  ],
  "syncTimestamp": "2026-02-10T15:00:00Z"
}
```

---

## 10. Sicherheitsarchitektur

```
┌────────── Mobile App ──────────┐     ┌────────── Backend ──────────────┐
│                                 │     │                                  │
│  Biometrische Entsperrung      │     │  Keycloak / Azure AD B2C        │
│  (FaceID / Fingerprint)        │     │  ├── OAuth2 + OIDC              │
│          │                      │     │  ├── JWT Access Tokens (15min)  │
│          ▼                      │     │  ├── Refresh Tokens (7d)        │
│  expo-secure-store              │     │  └── RBAC Rollen:               │
│  (Keychain / Keystore)          │     │       Admin                     │
│  ├── JWT Tokens                 │     │       Planner (Bauleiter)       │
│  └── Encryption Keys            │     │       Worker (Monteur)          │
│          │                      │     │       Viewer (Auftraggeber)     │
│          ▼                      │     │                                  │
│  SQLCipher                      │     │  Row-Level Security             │
│  (verschlüsselte SQLite)        │     │  (Mandantentrennung)            │
│          │                      │     │                                  │
│          ▼                      │     │  Audit-Log                      │
│  TLS 1.3 ──────────────────────────▶  │  (alle Änderungen protokolliert)│
│  Certificate Pinning            │     │                                  │
│                                 │     │  DSGVO                          │
│                                 │     │  ├── Löschkonzept               │
│                                 │     │  ├── Datenexport (JSON)         │
│                                 │     │  └── Einwilligungsverwaltung    │
└─────────────────────────────────┘     └──────────────────────────────────┘
```

---

## 11. UI/UX-Design

### 11.1 Design-Prinzipien für die Baustelle

- **Große Touch-Targets** (min. 48x48dp) – Arbeitshandschuhe
- **Hoher Kontrast** – Sonneneinstrahlung, verschmutztes Display
- **Minimale Texteingabe** – Dropdowns, Scanner, Vorlagen statt Freitext
- **Ein-Hand-Bedienung** – Aktionen im Daumenbereich
- **Schnellerfassung** – Max. 3 Taps bis zur Fotodokumentation
- **Sync-Feedback** – Immer sichtbar: Online/Offline-Status, Pending-Count

### 11.2 Navigationsstruktur

```
┌──────────────────────────────────────────────────────┐
│                                                        │
│   Tab: Projekte    Tab: Erfassen    Tab: Sync    Tab: Profil
│   ─────────────    ──────────────   ─────────    ──────────
│                                                        │
│   Projekt-Liste    Kamera direkt    Upload-Queue  Einstellungen
│       │            → Foto machen    Letzter Sync  Fotoqualität
│       ▼            → Typ wählen     Konflikte     Offline-Daten
│   Projekt-Detail   → Metadaten                    Konto
│       │            → Speichern
│       ▼
│   Zonen
│       │
│       ▼
│   Installationen
│       │
│       ▼
│   Detail / Bearbeiten
│   ├── Fotos (Galerie + Kamera)
│   ├── Annotation
│   ├── Messwerte
│   └── Prüfprotokoll
│                                                        │
└──────────────────────────────────────────────────────┘
```

### 11.3 Schnellerfassung – Happy Path (Ziel: < 60 Sekunden)

```
App öffnen → Letztes Projekt automatisch geladen
    │
    ▼
Tab "Erfassen" tippen (oder Kamera-FAB)
    │
    ▼
Zone wählen (Liste) ODER QR-Code scannen
    │
    ▼
Typ wählen: [Kabel] [Dose] [Verteiler] [Schalter] [Sonstige]
    │
    ▼
📷 Foto aufnehmen (1-Tap)
    │
    ▼
Optional: Annotation zeichnen (Pfeil, Maßlinie)
    │
    ▼
Pflichtfelder ausfüllen (typabhängig, Dropdowns)
    │
    ▼
✅ Speichern → Lokal in SQLite → In Sync-Queue
```

---

## 12. Infrastruktur & Deployment

### 12.1 Container-Architektur

**Lokale Entwicklung: .NET Aspire** (bevorzugt – ein Kommando startet alles)

```bash
cd src/backend/AppHost
dotnet run
# → Startet: PostgreSQL, Keycloak, RabbitMQ, Redis
# → Startet: Documentation.Api, Projects.Api, Sync.Api, ApiGateway
# → Öffnet: Aspire Dashboard auf https://localhost:15888
```

**Alternative: docker-compose.yml** (für CI/CD und Nicht-.NET-Entwickler)

```yaml
# docker-compose.yml
services:
  api-gateway:
    image: baudoku-gateway:latest
    ports: ["5000:8080"]
    depends_on: [documentation-api, projects-api, sync-api]

  documentation-api:
    image: baudoku-documentation:latest
    environment:
      - ConnectionStrings__Documentation=Host=db;Database=baudoku_documentation
      - BlobStorage__ConnectionString=...

  projects-api:
    image: baudoku-projects:latest
    environment:
      - ConnectionStrings__Projects=Host=db;Database=baudoku_projects

  sync-api:
    image: baudoku-sync:latest
    environment:
      - ConnectionStrings__Sync=Host=db;Database=baudoku_sync

  db:
    image: postgis/postgis:16-3.4
    volumes: ["pgdata:/var/lib/postgresql/data"]

  keycloak:
    image: quay.io/keycloak/keycloak:26.0
    environment:
      - KC_DB=postgres
      - KC_DB_URL=jdbc:postgresql://db/keycloak

  redis:
    image: redis:7-alpine

  rabbitmq:
    image: rabbitmq:3-management
```

### 12.2 CI/CD Pipeline (GitHub Actions)

```
Push to main
    │
    ├── Backend Pipeline (pro Bounded Context)
    │   ├── dotnet build BauDoku.sln
    │   ├── dotnet test (Unit + Integration pro BC)
    │   ├── Docker build pro Service (Documentation, Projects, Sync, Gateway)
    │   ├── Docker push → Container Registry
    │   └── Deploy to Kubernetes (Staging → Manual Approval → Production)
    │
    ├── Mobile App Pipeline
    │   ├── npx expo prebuild
    │   ├── EAS Build (iOS + Android)
    │   ├── Detox E2E Tests
    │   └── EAS Submit (App Store + Play Store)
    │
    └── Web Pipeline
        ├── npx expo export:web
        └── Deploy to CDN (Vercel / Azure Static)
```

### 12.3 Environments

| Environment | Zweck | Infrastruktur |
|-------------|-------|---------------|
| **Dev** | Lokale Entwicklung | .NET Aspire (ein Kommando), alternativ Docker Compose |
| **QA** | Automatisierte Tests | Kubernetes (kleiner Cluster), TestContainers |
| **Staging** | Pre-Production, Feldtests | Kubernetes (Production-Like) |
| **Production** | Live-System | Kubernetes (AKS/EKS), managed PostgreSQL, managed Redis |

---

## 13. Repository & Projektstruktur

### 13.1 Monorepo-Aufbau

Ein einziges Git-Repository für Frontend, Backend und Infrastruktur. Kein Turborepo/Nx für den Start – die Trennung erfolgt über Ordnerstruktur und separate Build-Pipelines.

```
baudoku/
├── src/
│   ├── backend/                                # .NET 10 Solution
│   │   ├── BuildingBlocks/
│   │   │   ├── BauDoku.BuildingBlocks.Domain/
│   │   │   │   ├── Entity.cs
│   │   │   │   ├── AggregateRoot.cs
│   │   │   │   ├── ValueObject.cs              # abstract record ValueObject
│   │   │   │   ├── IDomainEvent.cs
│   │   │   │   ├── IBusinessRule.cs
│   │   │   │   └── BusinessRuleException.cs
│   │   │   ├── BauDoku.BuildingBlocks.Application/
│   │   │   │   ├── Dispatcher/
│   │   │   │   │   ├── IDispatcher.cs
│   │   │   │   │   ├── Dispatcher.cs
│   │   │   │   │   ├── ICommand.cs
│   │   │   │   │   ├── ICommandHandler.cs
│   │   │   │   │   ├── IQuery.cs
│   │   │   │   │   ├── IQueryHandler.cs
│   │   │   │   │   ├── IDomainEventHandler.cs
│   │   │   │   │   └── DispatcherRegistration.cs
│   │   │   │   ├── Behaviors/
│   │   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   │   └── TransactionBehavior.cs
│   │   │   │   └── IUnitOfWork.cs
│   │   │   └── BauDoku.BuildingBlocks.Infrastructure/
│   │   │       ├── Persistence/
│   │   │       │   ├── BaseDbContext.cs
│   │   │       │   └── UnitOfWork.cs
│   │   │       ├── Auth/
│   │   │       └── Messaging/
│   │   │
│   │   ├── Services/
│   │   │   ├── Documentation/                  # BC: Installationsdokumentation
│   │   │   │   ├── BauDoku.Documentation.Domain/
│   │   │   │   ├── BauDoku.Documentation.Application/
│   │   │   │   ├── BauDoku.Documentation.Infrastructure/
│   │   │   │   └── BauDoku.Documentation.Api/
│   │   │   ├── Projects/                       # BC: Projektverwaltung
│   │   │   │   ├── BauDoku.Projects.Domain/
│   │   │   │   ├── BauDoku.Projects.Application/
│   │   │   │   ├── BauDoku.Projects.Infrastructure/
│   │   │   │   └── BauDoku.Projects.Api/
│   │   │   └── Sync/                           # BC: Offline-Synchronisation
│   │   │       ├── BauDoku.Sync.Domain/
│   │   │       ├── BauDoku.Sync.Application/
│   │   │       ├── BauDoku.Sync.Infrastructure/
│   │   │       └── BauDoku.Sync.Api/
│   │   │
│   │   ├── ApiGateway/
│   │   │   └── BauDoku.ApiGateway/
│   │   ├── AppHost/
│   │   │   └── BauDoku.AppHost/                # .NET Aspire
│   │   │
│   │   ├── BauDoku.sln
│   │   ├── Directory.Build.props               # Shared .NET Properties
│   │   ├── Directory.Packages.props            # Central Package Management
│   │   └── .editorconfig
│   │
│   └── frontend/                               # React Native + Expo
│       ├── app/                                # Expo Router Pages
│       ├── src/
│       │   ├── components/
│       │   ├── hooks/
│       │   ├── services/
│       │   ├── stores/
│       │   ├── db/
│       │   ├── gnss/
│       │   └── types/
│       ├── app.json
│       ├── package.json
│       └── tsconfig.json
│
├── tests/
│   ├── backend/
│   │   ├── BauDoku.Documentation.UnitTests/
│   │   ├── BauDoku.Documentation.IntegrationTests/
│   │   ├── BauDoku.Projects.UnitTests/
│   │   ├── BauDoku.Sync.IntegrationTests/
│   │   └── BauDoku.Architecture.Tests/         # ArchUnit-Style Tests
│   └── frontend/
│       ├── __tests__/
│       └── e2e/                                # Detox E2E
│
├── infrastructure/
│   ├── docker/
│   │   ├── docker-compose.yml
│   │   └── Dockerfile.api                      # Multi-Stage Build
│   ├── k8s/
│   │   ├── base/
│   │   └── overlays/
│   │       ├── dev/
│   │       ├── staging/
│   │       └── production/
│   └── azure/                                  # Bicep Templates (optional)
│
├── docs/
│   ├── architecture.md                         # Dieses Dokument
│   ├── adr/                                    # Architecture Decision Records
│   │   ├── 001-react-native-expo.md
│   │   ├── 002-dotnet10-ddd.md
│   │   ├── 003-custom-dispatcher.md
│   │   ├── 004-sealed-record-valueobjects.md
│   │   └── 005-sapos-gnss-strategy.md
│   └── api/
│       └── openapi.yaml
│
├── .github/
│   └── workflows/
│       ├── backend-ci.yml
│       ├── mobile-ci.yml
│       └── web-ci.yml
│
├── .gitignore
├── README.md
└── LICENSE
```

### 13.2 .NET Solution-Struktur

```xml
<!-- BauDoku.sln – Solution Folders spiegeln die Ordnerstruktur -->

Solution
├── BuildingBlocks
│   ├── BauDoku.BuildingBlocks.Domain.csproj
│   ├── BauDoku.BuildingBlocks.Application.csproj
│   └── BauDoku.BuildingBlocks.Infrastructure.csproj
├── Services
│   ├── Documentation
│   │   ├── BauDoku.Documentation.Domain.csproj        → refs BuildingBlocks.Domain
│   │   ├── BauDoku.Documentation.Application.csproj   → refs Documentation.Domain, BuildingBlocks.Application
│   │   ├── BauDoku.Documentation.Infrastructure.csproj→ refs Documentation.Application, BuildingBlocks.Infrastructure
│   │   └── BauDoku.Documentation.Api.csproj           → refs Documentation.Infrastructure
│   ├── Projects
│   │   ├── BauDoku.Projects.Domain.csproj
│   │   ├── BauDoku.Projects.Application.csproj
│   │   ├── BauDoku.Projects.Infrastructure.csproj
│   │   └── BauDoku.Projects.Api.csproj
│   └── Sync
│       ├── BauDoku.Sync.Domain.csproj
│       ├── BauDoku.Sync.Application.csproj
│       ├── BauDoku.Sync.Infrastructure.csproj
│       └── BauDoku.Sync.Api.csproj
├── Gateway
│   └── BauDoku.ApiGateway.csproj
├── Orchestration
│   └── BauDoku.AppHost.csproj                         → .NET Aspire
└── Tests
    ├── BauDoku.Documentation.UnitTests.csproj
    ├── BauDoku.Documentation.IntegrationTests.csproj
    ├── BauDoku.Projects.UnitTests.csproj
    ├── BauDoku.Sync.IntegrationTests.csproj
    └── BauDoku.Architecture.Tests.csproj
```

**Dependency Rule (Clean Architecture):** Domain → ∅ (keine Abhängigkeiten), Application → Domain, Infrastructure → Application, Api → Infrastructure. Niemals rückwärts.

### 13.3 Central Package Management

```xml
<!-- Directory.Packages.props -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- Framework -->
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.*" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.*" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite" Version="10.0.*" />

    <!-- DDD / Application -->
    <PackageVersion Include="FluentValidation" Version="11.*" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />
    <PackageVersion Include="Scrutor" Version="5.*" />

    <!-- Infrastructure -->
    <PackageVersion Include="Polly" Version="8.*" />
    <PackageVersion Include="Serilog.AspNetCore" Version="9.*" />
    <PackageVersion Include="RabbitMQ.Client" Version="7.*" />
    <PackageVersion Include="Azure.Storage.Blobs" Version="12.*" />
    <PackageVersion Include="Yarp.ReverseProxy" Version="2.*" />

    <!-- Aspire -->
    <PackageVersion Include="Aspire.Hosting" Version="9.*" />
    <PackageVersion Include="Aspire.Hosting.PostgreSQL" Version="9.*" />

    <!-- Testing -->
    <PackageVersion Include="xunit" Version="2.*" />
    <PackageVersion Include="FluentAssertions" Version="7.*" />
    <PackageVersion Include="Testcontainers.PostgreSql" Version="4.*" />
    <PackageVersion Include="NSubstitute" Version="5.*" />
  </ItemGroup>
</Project>
```

### 13.4 Git-Workflow

**Branching-Strategie:** GitHub Flow (einfach, passt zu Solo/Kleines-Team)

```
main ──────────────────────────────────────────────── (immer deploybar)
  │
  ├── feature/BD-001-buildingblocks-domain ──── PR → main
  ├── feature/BD-002-project-aggregate ──────── PR → main
  ├── feature/BD-010-expo-setup ─────────────── PR → main
  ├── bugfix/BD-015-sync-conflict ───────────── PR → main
  └── hotfix/BD-020-photo-upload-crash ──────── PR → main
```

**Commit-Format:** Conventional Commits

```
feat(documentation): add Installation aggregate with GPS ValueObjects
fix(sync): resolve conflict when same installation edited offline
refactor(buildingblocks): extract validation pipeline from dispatcher
test(projects): add unit tests for Project aggregate
docs(adr): add ADR-003 custom dispatcher decision
chore(ci): add backend build pipeline
```

**Ticket-Prefix:** `BD-xxx` (BauDoku)

---

## 14. Projektplanung (Roadmap)

### Gesamtübersicht

```
Phase 0: Foundation          ██░░░░░░░░░░░░░░░░░░  2 Wochen
Phase 1: MVP                 ░░██████████░░░░░░░░░  3–4 Monate
Phase 2: Erweiterung         ░░░░░░░░░░░░████░░░░░  2–3 Monate
Phase 3: Premium             ░░░░░░░░░░░░░░░░████░  2–3 Monate
                             ───────────────────────
                             M1  M2  M3  M4  M5  M6  M7  M8  M9  M10
```

### Phase 0: Foundation (2 Wochen)

Ziel: Repo-Grundgerüst, BuildingBlocks lauffähig, alle Services starten mit Aspire.

| # | Aufgabe | Dauer | Ergebnis |
|---|---------|-------|----------|
| 0.1 | Git-Repo + README + .gitignore + LICENSE | 0,5d | Repo steht |
| 0.2 | .NET 10 Solution anlegen (alle 18 Projekte) | 1d | `dotnet build` grün |
| 0.3 | `BuildingBlocks.Domain` implementieren | 1d | ValueObject, Entity, AggregateRoot, IDomainEvent, IBusinessRule |
| 0.4 | `BuildingBlocks.Application` implementieren | 1,5d | Dispatcher, ICommand/IQuery, Handler-Interfaces, Behaviors, DI-Registration |
| 0.5 | `BuildingBlocks.Infrastructure` implementieren | 1d | BaseDbContext (Domain Events dispatchen bei SaveChanges), UnitOfWork |
| 0.6 | .NET Aspire AppHost konfigurieren | 0,5d | `dotnet run` startet PostgreSQL, Redis, RabbitMQ, Keycloak, alle APIs |
| 0.7 | Erster "Hello World"-Endpoint pro Service | 0,5d | GET /health auf allen drei BCs antwortet |
| 0.8 | API Gateway (YARP) konfigurieren | 0,5d | Routing: /api/documentation/* → Documentation.Api, etc. |
| 0.9 | Architecture Tests (Dependency Rule) | 0,5d | Domain referenziert keine Application/Infrastructure-Projekte |
| 0.10 | CI Pipeline (GitHub Actions): Backend Build + Test | 0,5d | Push → automatischer Build + Test |
| 0.11 | Expo-Projekt initialisieren (Frontend) | 0,5d | `npx expo start` läuft, Expo Router konfiguriert |
| 0.12 | ADRs schreiben (5 Entscheidungen dokumentieren) | 0,5d | ADR-001 bis ADR-005 |

### Phase 1: MVP (3–4 Monate, Sprint 1–8)

| Sprint | Fokus | Backend | Frontend | Deliverable |
|--------|-------|---------|----------|-------------|
| **1** | **Projects BC + Expo-Grundgerüst** | Project Aggregate (Root, Zone Entities), ProjectId/ZoneName/Address ValueObjects, CreateProject + ListProjects Handlers, EF Core Config + Migration | Expo Router Navigation, Auth-Flow (Keycloak), Theme + Basis-Komponenten | Projekt anlegen + anzeigen, End-to-End |
| **2** | **Documentation BC – Installation** | Installation Aggregate Root, GpsPosition/CableSpec/Depth/InstallationType ValueObjects, DocumentInstallation + GetInstallation Handlers | Installations-CRUD Screens, Formular mit Typ-Auswahl, GPS-Positionsanzeige | Installation erfassen mit allen Feldern |
| **3** | **Foto-Erfassung** | AddPhoto Command, PhotoBlobStorage (Azure/S3), Photo Entity im Installation Aggregate | expo-camera Integration, Foto aufnehmen + Vorschau, Foto-Galerie pro Installation | Fotos zu Installationen hinzufügen |
| **4** | **Messungen + Prüfungen** | Measurement Entity, MeasurementValue/MeasurementType ValueObjects, RecordMeasurement Handler, Business Rules (Grenzwerte) | Messwert-Erfassung (Isolationswiderstand, Schleifenimpedanz, RCD), Bestanden/Nicht-bestanden Anzeige | Messwerte erfassen + auswerten |
| **5** | **Offline-Datenbank** | — (rein Frontend) | SQLite mit Drizzle ORM: Schema für Projects, Zones, Installations, Photos, Measurements, Outbox-Tabelle für Sync | Komplette App funktioniert offline |
| **6** | **Sync-Engine** | Sync BC: ProcessSyncBatch + GetChangesSince + ResolveConflict, Delta-Sync API Endpoints | SyncManager, Outbox-Pattern, automatischer Sync bei Reconnect, Konflikt-UI | Offline-Daten werden synchronisiert |
| **7** | **Chunked Photo Upload** | Chunked Upload Endpoints (Init/Chunk/Complete), Resume-Logik | Foto-Upload mit Fortschrittsanzeige, Resume nach Abbruch, Hintergrund-Upload | Große Fotos zuverlässig hochladen |
| **8** | **GPS-Metadaten + Stabilisierung** | GPS-Metadaten in allen Entities persistiert (PostGIS), Spatial Queries | GPS-Genauigkeitsanzeige, Mock Location Support (Android), QualityIndicator-Komponente | MVP-Release: Vollständige Dokumentation offline + sync |

**MVP liefert:** Projekte mit Zonen anlegen, Installationen mit Fotos + Messungen + GPS-Position offline dokumentieren, automatische Synchronisation zum Server. GPS-Position (intern, ~3–5 m) mit vollständigen Metadaten. Android: Mock Location mit externem GNSS-Empfänger (SAPOS-EPS, ~30 cm).

### Phase 2: Erweiterung (2–3 Monate, Sprint 9–14)

| Sprint | Fokus | Details |
|--------|-------|---------|
| **9–10** | **GNSS: BLE + NTRIP** | Eigener BLE-Stack (react-native-ble-plx), NMEA-Parser, NTRIP-Client für SAPOS-EPS, ExternalGnssProvider, PositionManager mit automatischer Quellenwahl, ~30–50 cm Genauigkeit |
| **11** | **Foto-Annotation** | react-native-skia Integration, Pfeile/Maßlinien/Textlabels auf Fotos zeichnen, Annotation als JSON speichern |
| **12** | **PDF-Berichte** | Report BC (oder Service in Documentation), VDE/DIN-konforme PDF-Generierung, Prüfprotokoll-Vorlagen, Export per E-Mail/Share |
| **13** | **Web-Portal** | Expo Web Build, Dashboard für Bauleiter (Projekt-Übersicht, Fortschritt, Foto-Galerie), Read-Only Ansicht |
| **14** | **QR-Code + Unterschriften** | QR-Scanner für Komponentenidentifikation, digitale Unterschrift für Abnahmeprotokolle |

### Phase 3: Premium (2–3 Monate, Sprint 15–20)

| Sprint | Fokus | Details |
|--------|-------|---------|
| **15–16** | **GNSS: RTK + SAPOS-HEPS** | RTK-Empfänger Support (~1–3 cm), SAPOS-HEPS Integration, Massenermittlung aus GPS-Tracks |
| **17** | **CAD-Integration** | DXF-Export von vermessenen Punkten, Grundriss-Overlay mit Positionsmarkierung |
| **18** | **Dashboard & Analytics** | Projektfortschritt, Statistiken, Heatmaps, Export nach Excel/CSV |
| **19** | **Multi-Tenancy (SaaS)** | Mandantenfähigkeit, Tenant-Isolation auf DB-Ebene, Pricing/Billing |
| **20** | **API & Integrationen** | REST API für Drittsysteme (ERP, SAP, BIM), Webhook-System, API-Key-Verwaltung |

---

## 15. Sprint 1 – Detailplanung

### Sprint-Ziel
*"Erstes Projekt mit Zonen anlegen und in der App anzeigen – End-to-End (Frontend → API Gateway → Projects BC → PostgreSQL)."*

**Dauer:** 2 Wochen (nach Phase 0 Foundation)

### Tickets

#### Backend – Projects Bounded Context

| Ticket | Titel | Story Points | Beschreibung |
|--------|-------|:---:|-------------|
| **BD-101** | Project Aggregate Root implementieren | 5 | `Project.cs` als AggregateRoot mit Factory Method `Create()`. Zone als Child Entity. Domain Events: `ProjectCreated`, `ZoneAdded`. Alle Properties über ValueObjects. |
| **BD-102** | ValueObjects für Projects BC | 3 | `ProjectId`, `ProjectName` (max 200 chars), `ProjectStatus` (Draft/Active/Completed/Archived), `ZoneId`, `ZoneName`, `ZoneType` (Building/Floor/Room/Trench), `Address` (street, city, zip), `ClientInfo` (name, contact). Alle als `sealed record : ValueObject` mit Konstruktor-Validierung. |
| **BD-103** | Business Rules für Project | 2 | `ProjectMustHaveUniqueName`, `ZoneNameMustBeUniqueWithinProject`. Implementierung als `IBusinessRule`. |
| **BD-104** | CreateProject Command + Handler | 3 | `CreateProjectCommand` (name, address, client, initialZones), `CreateProjectHandler` mit Validierung (FluentValidation) und Persistierung. Gibt `ProjectId` zurück. |
| **BD-105** | AddZone Command + Handler | 2 | `AddZoneCommand` (projectId, name, type, parentZoneId?), Handler lädt Project Aggregate, ruft `project.AddZone()` auf, speichert. |
| **BD-106** | GetProject + ListProjects Queries | 3 | `GetProjectQuery` (id) → `ProjectDto` mit allen Zonen. `ListProjectsQuery` (page, pageSize, search?) → Paginierte Liste. DTOs flach (keine Domain-Objekte über API-Grenze). |
| **BD-107** | EF Core Configuration + Migration | 3 | `ProjectConfiguration.cs`, `ZoneConfiguration.cs` – Owned Types für Address/ClientInfo, Value Conversions für alle Ids/ValueObjects. Erste Migration. Seed-Daten (1 Beispielprojekt mit 3 Zonen). |
| **BD-108** | Minimal API Endpoints für Projects | 2 | `MapProjectEndpoints()`: POST /api/projects, GET /api/projects, GET /api/projects/{id}, POST /api/projects/{id}/zones. Alle über `IDispatcher`. |
| **BD-109** | Integration Test: Project CRUD | 3 | Testcontainers mit PostgreSQL. Teste: Projekt anlegen → Projekt laden → Zone hinzufügen → Projekt mit Zonen laden. Verifiziere Domain Events. |

#### Frontend – Expo Grundgerüst

| Ticket | Titel | Story Points | Beschreibung |
|--------|-------|:---:|-------------|
| **BD-110** | Expo Router Navigation Setup | 3 | Tab-Navigation (Projekte, Erfassung, Sync, Einstellungen). Stack-Navigation innerhalb Projekte (Liste → Detail → Zonen). Layout mit Header + Tab Bar. |
| **BD-111** | Auth-Flow (Keycloak OIDC) | 5 | Login/Logout mit expo-auth-session, Token-Storage in expo-secure-store, Auto-Refresh, Auth-Guard für geschützte Screens. |
| **BD-112** | Design-System Basis | 3 | Theme (Farben, Typo, Spacing – baustellentauglich: große Touch-Targets, hoher Kontrast), Basis-Komponenten: Button, TextInput, Card, ListItem, Badge, EmptyState. |
| **BD-113** | Projekt-Liste Screen | 3 | API-Anbindung mit @tanstack/react-query, Pull-to-Refresh, Suchfeld, FAB "Neues Projekt", Ladestate + Fehlermeldung + Empty State. |
| **BD-114** | Projekt anlegen Screen | 3 | Formular mit zod-Validierung: Name (Pflicht), Adresse (Straße, PLZ, Ort), Auftraggeber (Name, Kontakt). Submit → POST /api/projects → Zurück zur Liste. |
| **BD-115** | Projekt-Detail + Zonen Screen | 3 | Projekt-Info anzeigen, Liste der Zonen (hierarchisch: Gebäude → Stockwerk → Raum), "Zone hinzufügen"-Button, Typ-Auswahl. |
| **BD-116** | API-Client Setup | 2 | Axios-Instanz mit Base URL, Auth-Interceptor (Token), Error-Interceptor, Type-safe API-Funktionen für Projects. |

#### Übergreifend

| Ticket | Titel | Story Points | Beschreibung |
|--------|-------|:---:|-------------|
| **BD-117** | OpenAPI Spec für Projects BC | 2 | Swagger/Scalar UI auf Documentation.Api, generierte OpenAPI 3.1 Spec. Dient als Contract zwischen Frontend und Backend. |
| **BD-118** | E2E Smoke Test | 2 | Detox-Test: App starten → Login → Projekt anlegen → Projekt in Liste sehen → Projekt öffnen → Zone hinzufügen. Läuft gegen echtes Backend (Aspire). |

### Sprint 1 – Zusammenfassung

| Bereich | Tickets | Story Points |
|---------|:-------:|:---:|
| Backend (Projects BC) | 9 | 26 |
| Frontend (Expo) | 7 | 22 |
| Übergreifend | 2 | 4 |
| **Gesamt** | **18** | **52** |

**Kapazität:** Solo-Entwickler, 2-Wochen-Sprint → ~40–50 SP realistisch. Sprint 1 ist ambitioniert aber machbar, da Phase 0 das Grundgerüst bereits liefert.

**Definition of Done (Sprint 1):**
- Projekt mit Zonen anlegen funktioniert End-to-End (App → Gateway → API → DB → zurück)
- Auth-Flow mit Keycloak funktioniert
- Alle Unit-Tests grün, mindestens 1 Integration-Test
- Code reviewed (Self-Review + CI grün)
- ADR geschrieben für jede wesentliche Entscheidung

---

## 16. Risiken & Mitigationen

| # | Risiko | Impact | Wahrschein&shy;lichkeit | Mitigation |
|---|--------|:------:|:---:|------------|
| R1 | React Native lernen (Angular-Background) | Mittel | Hoch | Tutorial-Phase in Phase 0, Expo vereinfacht vieles, TypeScript-Erfahrung hilft |
| R2 | Expo-Modul-Inkompatibilität im Web | Mittel | Mittel | Platform.OS-Checks, Web-Features erst in Phase 2, separat testen |
| R3 | Sync-Konflikte bei Mehrbenutzer | Mittel | Mittel | Field-Level-Merge, UI für manuelle Auflösung, Konflikt-Logging |
| R4 | Foto-Speicher wird zu groß (lokal + Server) | Hoch | Hoch | Progressive Kompression, Thumbnails, Auto-Cleanup nach Sync |
| R5 | Akzeptanz bei Bauarbeitern | Hoch | Mittel | UX-Tests mit echten Nutzern, extreme Einfachheit, große Touch-Targets |
| R6 | react-native-skia Performance bei großen Fotos | Gering | Gering | Benchmark mit 12+ MP Fotos im Spike |
| R7 | GPS-Genauigkeit unzureichend ohne ext. Empfänger | Hoch | Hoch | Klare UI-Warnung, Genauigkeit mitspeichern, Stufen-Konzept mit Fallback |
| R8 | iOS kein Zugriff auf GNSS-Rohdaten | Hoch | Sicher | Externer BLE-Empfänger für iOS Pflicht bei Vermessung |
| R9 | SAPOS-Verfügbarkeit auf Baustelle (Mobilfunk) | Mittel | Mittel | Post-Processing (GPPS) als Fallback, Position auch ohne Korrektur erfassen |
| R10 | Bluetooth-Verbindung zu GNSS-Empfänger instabil | Mittel | Mittel | Auto-Reconnect, NMEA-Puffer, Fallback auf internes GPS |
| R11 | Multipath-Fehler auf Baustellen (Gebäude, Kräne) | Mittel | Hoch | RTK-Empfänger mit Multipath-Rejection, Genauigkeitswarnung, manuelle Korrektur |
| R12 | DDD-Overhead für Solo-Entwickler | Mittel | Mittel | BuildingBlocks als Template, Code-Generierung für neue BCs erwägen, Pragmatismus vor Perfektion |
| R13 | .NET 10 noch Preview (Stand Feb 2026) | Gering | Gering | .NET 10 GA erwartet Nov 2025, bereits stabil. Fallback: .NET 9 LTS als Zwischenlösung |
| R14 | Zu viele Microservices für MVP | Mittel | Mittel | Drei BCs ist schlank. Können als Monolith (ein Prozess, getrennte Module) starten und später splitten |

---

## 17. Nächste Schritte (Sofort-Aktionen)

| # | Aktion | Dauer | Priorität |
|---|--------|-------|:---------:|
| 1 | **Git-Repo erstellen** – GitHub, README, .gitignore, LICENSE | 0,5d | 🔴 |
| 2 | **.NET 10 Solution anlegen** – `dotnet new sln`, alle 18 Projekte, Directory.Build.props | 1d | 🔴 |
| 3 | **BuildingBlocks implementieren** – ValueObject, Entity, AggregateRoot, Dispatcher | 2,5d | 🔴 |
| 4 | **.NET Aspire AppHost** – PostgreSQL + Redis + RabbitMQ + Keycloak + 3 APIs | 0,5d | 🔴 |
| 5 | **Expo-Projekt initialisieren** – `npx create-expo-app`, Router, TypeScript | 0,5d | 🔴 |
| 6 | **CI/CD Pipeline** – GitHub Actions: Backend Build + Test + Frontend Lint | 0,5d | 🟡 |
| 7 | **SAPOS-Registrierung** – Kostenlos im relevanten Bundesland | 0,5d | 🟡 |
| 8 | **GNSS-Hardware bestellen** – Columbus P-7 Pro (~150€) für erste Tests | 1d | 🟡 |
| 9 | **ADRs schreiben** – Entscheidungen dokumentieren (5 initiale ADRs) | 0,5d | 🟡 |
| 10 | **UX-Wireframes** – Low-Fidelity für Schnellerfassung + GPS-Status (Figma/Paper) | 2d | 🟡 |
| 11 | **OpenAPI-Spec** – Contract zwischen Frontend und Backend (Projects BC zuerst) | 1d | 🟡 |
| 12 | **Datenmodell validieren** – Mit Elektroplanern branchenspezifische Felder abstimmen | 2d | 🟢 |
| 13 | **Sprint 1 starten** | — | 🔴 |

**Legende:** 🔴 Sofort (Phase 0), 🟡 Parallel/kurz danach, 🟢 Kann warten

---

*Dieses Dokument definiert die Zielarchitektur für BauDoku (v4.0). Backend: .NET 10 mit DDD, Clean Architecture, eigenem Dispatcher und sealed record ValueObjects nach dem Vorbild der Orange Car Rental Solution. Frontend: React Native + Expo. Enthält die vollständige GPS/GNSS-Strategie mit SAPOS-Integration. Alle Schätzungen sind Richtwerte und werden nach Phase 0 Foundation, Tech-Spike und UX-Workshops verfeinert.*
