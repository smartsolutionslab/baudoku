#!/bin/bash
set -e

# Phase 0 Issues (BD-002 to BD-012)
echo "=== Creating Phase 0 Issues ==="

gh issue create --title "BD-002 .NET 10 Solution mit allen 18 Projekten anlegen" --label "phase-0,backend,building-blocks" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich eine vollständige .NET Solution mit allen 18 Projekten und korrekten Abhängigkeiten, damit ich sofort mit der Implementierung der BuildingBlocks beginnen kann.

## Akzeptanzkriterien
- [ ] `BauDoku.sln` existiert mit allen 18 Projekten
- [ ] BuildingBlocks: Domain, Application, Infrastructure
- [ ] Services/Projects: Domain, Application, Infrastructure, Api
- [ ] Services/Documentation: Domain, Application, Infrastructure, Api
- [ ] Services/Sync: Domain, Application, Infrastructure, Api
- [ ] ApiGateway/BauDoku.ApiGateway
- [ ] AppHost/BauDoku.AppHost
- [ ] Tests: Projects.UnitTests, Documentation.UnitTests, Documentation.IntegrationTests, Sync.IntegrationTests
- [ ] Dependency Rule eingehalten: Domain → ∅, Application → Domain, Infrastructure → Application, Api → Infrastructure
- [ ] `dotnet build` ist grün
- [ ] Solution Folders für logische Gruppierung

## Technische Details
- Setup-Script `scripts/setup-solution.sh` als Basis oder manuell anlegen
- `Directory.Build.props` und `Directory.Packages.props` bereits vorhanden
- Target Framework: .NET 10
EOF
)"
echo "Created BD-002"

gh issue create --title "BD-003 BuildingBlocks.Domain implementieren" --label "phase-0,backend,building-blocks,domain" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich die Domain-Basisklassen (ValueObject, Entity, AggregateRoot), damit alle Bounded Contexts konsistente Patterns verwenden.

## Akzeptanzkriterien
- [ ] `ValueObject` als `abstract record`
- [ ] `Entity<TId>` mit Id-Property (`where TId : ValueObject`)
- [ ] `AggregateRoot<TId>` mit DomainEvents-Sammlung, `AddDomainEvent()`, `ClearDomainEvents()`
- [ ] `IDomainEvent` Interface (mit Timestamp)
- [ ] `IBusinessRule` Interface mit `IsBroken()` und `Message`
- [ ] `BusinessRuleException` für Regelverletzungen
- [ ] `CheckRule()` in AggregateRoot als `protected static` Methode
- [ ] Unit Tests für alle Basisklassen

## Technische Details
- Namespace: `BauDoku.BuildingBlocks.Domain`
- Eine Datei pro Klasse, sealed wo möglich
- Keine externen Abhängigkeiten (Pure Domain)
EOF
)"
echo "Created BD-003"

gh issue create --title "BD-004 BuildingBlocks.Application implementieren" --label "phase-0,backend,building-blocks,application" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich den eigenen Dispatcher mit Command/Query-Interfaces und Behaviors, damit ich CQRS ohne MediatR umsetzen kann.

## Akzeptanzkriterien
- [ ] `ICommand` und `ICommand<TResult>` Interfaces
- [ ] `IQuery<TResult>` Interface
- [ ] `ICommandHandler<TCommand>` und `ICommandHandler<TCommand, TResult>`
- [ ] `IQueryHandler<TQuery, TResult>`
- [ ] `IDomainEventHandler<TEvent>`
- [ ] `IDispatcher` Interface mit Send(), Query(), Publish()
- [ ] `Dispatcher` Implementierung (DI-basiert über IServiceProvider)
- [ ] `ValidationBehavior` (FluentValidation Pipeline)
- [ ] `LoggingBehavior` (Serilog)
- [ ] `TransactionBehavior` (UnitOfWork)
- [ ] DI-Extension: `services.AddDispatcher(assemblies)` mit Scrutor
- [ ] Unit Tests für Dispatcher und Behaviors

## Technische Details
- Namespace: `BauDoku.BuildingBlocks.Application`
- Scrutor für automatisches Handler-Scanning
- Kein MediatR, kein Wolverine
EOF
)"
echo "Created BD-004"

gh issue create --title "BD-005 BuildingBlocks.Infrastructure implementieren" --label "phase-0,backend,building-blocks,infrastructure" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich eine BaseDbContext-Klasse und UnitOfWork, damit Domain Events bei SaveChanges automatisch dispatched werden.

## Akzeptanzkriterien
- [ ] `BaseDbContext` erbt von DbContext, dispatched Domain Events bei SaveChanges
- [ ] `IUnitOfWork` Interface
- [ ] `UnitOfWork` Implementierung
- [ ] Domain Events nach erfolgreichem SaveChanges über IDispatcher.Publish() verteilen
- [ ] Outbox-Pattern Vorbereitung (Events in Outbox-Tabelle)
- [ ] DI-Extension: `services.AddInfrastructure()`
- [ ] Unit Tests

## Technische Details
- Namespace: `BauDoku.BuildingBlocks.Infrastructure`
- EF Core 10, Npgsql
- Pattern: Domain Events sammeln → SaveChanges → Dispatch → Clear
EOF
)"
echo "Created BD-005"

gh issue create --title "BD-006 .NET Aspire AppHost konfigurieren" --label "phase-0,backend,devops" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich mit einem einzigen dotnet run alle Services inkl. Infrastruktur starten können.

## Akzeptanzkriterien
- [ ] AppHost startet PostgreSQL Container (mit PostGIS)
- [ ] AppHost startet Redis Container
- [ ] AppHost startet RabbitMQ Container
- [ ] AppHost startet Keycloak Container
- [ ] Alle drei Service-APIs werden gestartet (Projects, Documentation, Sync)
- [ ] API Gateway wird gestartet
- [ ] Aspire Dashboard zeigt alle Services
- [ ] Connection Strings werden automatisch an Services verteilt

## Technische Details
- Projekt: BauDoku.AppHost
- Aspire.Hosting 9.*
- Service Discovery über Aspire
EOF
)"
echo "Created BD-006"

gh issue create --title "BD-007 Health-Endpoint pro Service" --label "phase-0,backend,api" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich einen Health-Endpoint auf jedem Service, damit ich überprüfen kann, dass alle Services korrekt starten.

## Akzeptanzkriterien
- [ ] GET /health auf Projects.Api antwortet mit 200 OK
- [ ] GET /health auf Documentation.Api antwortet mit 200 OK
- [ ] GET /health auf Sync.Api antwortet mit 200 OK
- [ ] Health Checks prüfen DB-Verbindung
- [ ] Aspire Dashboard zeigt Health Status

## Technische Details
- ASP.NET Health Checks Middleware
- Minimal API: app.MapHealthChecks("/health")
EOF
)"
echo "Created BD-007"

gh issue create --title "BD-008 API Gateway (YARP) konfigurieren" --label "phase-0,backend,infrastructure" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich einen API Gateway, der alle Requests an die richtigen Backend-Services routet.

## Akzeptanzkriterien
- [ ] YARP Reverse Proxy konfiguriert
- [ ] Route: /api/projects/* → Projects.Api
- [ ] Route: /api/documentation/* → Documentation.Api
- [ ] Route: /api/sync/* → Sync.Api
- [ ] CORS konfiguriert für Expo Dev Server
- [ ] Health-Endpoint auf Gateway selbst
- [ ] Swagger/Scalar UI aggregiert alle Services

## Technische Details
- Projekt: BauDoku.ApiGateway
- Yarp.ReverseProxy 2.*
- Service Discovery über Aspire oder Konfiguration
EOF
)"
echo "Created BD-008"

gh issue create --title "BD-009 Architecture Tests (Dependency Rule)" --label "phase-0,backend,cross-cutting" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich automatisierte Tests, die die Dependency Rule der Clean Architecture prüfen.

## Akzeptanzkriterien
- [ ] Domain-Projekte referenzieren KEINE Application/Infrastructure/Api-Projekte
- [ ] Application-Projekte referenzieren nur Domain
- [ ] Infrastructure referenziert nur Application (und transitiv Domain)
- [ ] Tests sind automatisch in CI integriert

## Technische Details
- NetArchTest oder manuelle Assembly-Referenz-Prüfung
- xUnit Test-Projekt
- Prüfung aller drei Bounded Contexts
EOF
)"
echo "Created BD-009"

gh issue create --title "BD-010 CI Pipeline (GitHub Actions)" --label "phase-0,devops" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich eine CI Pipeline, die bei jedem Push automatisch baut und testet.

## Akzeptanzkriterien
- [ ] GitHub Actions Workflow: .github/workflows/backend.yml
- [ ] Trigger: Push auf main + Pull Requests
- [ ] Steps: Restore → Build → Test
- [ ] .NET 10 SDK Setup
- [ ] Test-Results als Artefakt
- [ ] Build-Badge im README

## Technische Details
- dotnet restore → dotnet build --no-restore → dotnet test --no-build
- Runner: Ubuntu latest
EOF
)"
echo "Created BD-010"

gh issue create --title "BD-011 Expo-Projekt initialisieren" --label "phase-0,frontend" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich ein konfiguriertes Expo-Projekt mit TypeScript und Router.

## Akzeptanzkriterien
- [ ] src/frontend/ mit Expo-Projekt (TypeScript)
- [ ] Expo Router konfiguriert (File-based Routing)
- [ ] npx expo start läuft fehlerfrei
- [ ] Basis-Ordnerstruktur: app/, components/, lib/, hooks/, constants/
- [ ] ESLint + Prettier konfiguriert
- [ ] .env Handling für API-URL

## Technische Details
- npx create-expo-app mit TypeScript-Template
- Expo SDK 52+, Expo Router v4
EOF
)"
echo "Created BD-011"

gh issue create --title "BD-012 ADRs schreiben (5 initiale Entscheidungen)" --label "phase-0,documentation" --milestone "Phase 0: Foundation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich die wichtigsten Architektur-Entscheidungen als ADRs dokumentiert haben.

## Akzeptanzkriterien
- [ ] ADR-001: Eigener Dispatcher statt MediatR
- [ ] ADR-002: sealed record ValueObjects (statt class-basiert)
- [ ] ADR-003: Aggregate Root Factory Pattern (statt Konstruktoren)
- [ ] ADR-004: React Native + Expo (statt Flutter / native)
- [ ] ADR-005: Dreistufiges GPS/GNSS-Konzept
- [ ] Format: Titel, Status, Kontext, Entscheidung, Konsequenzen

## Technische Details
- Ablage: docs/adr/
- Format: Markdown (Michael Nygard Template)
EOF
)"
echo "Created BD-012"

echo ""
echo "=== Creating Sprint 1 Issues ==="

gh issue create --title "BD-101 Project Aggregate Root implementieren" --label "sprint-1,backend,projects-bc,domain" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich ein Project Aggregate Root mit Factory Method und Zone als Child Entity.

## Akzeptanzkriterien
- [ ] `Project` als `sealed class : AggregateRoot<ProjectId>`
- [ ] Statische Factory Method `Project.Create(id, name, address, client)`
- [ ] `Zone` als Entity innerhalb des Project Aggregate
- [ ] `project.AddZone(zoneId, name, type, parentZoneId?)` Methode
- [ ] Domain Events: `ProjectCreated`, `ZoneAdded`
- [ ] Alle Properties über ValueObjects (keine Primitiven)
- [ ] Private Konstruktor für EF Core
- [ ] Unit Tests: Erstellung, Zone hinzufügen, Domain Events

## Story Points: 5

## Technische Details
- Namespace: BauDoku.Projects.Domain.Aggregates
- Zone-Hierarchie: Building → Floor → Room → Trench
- _zones als private List, Zones als IReadOnlyList
EOF
)"
echo "Created BD-101"

gh issue create --title "BD-102 ValueObjects für Projects BC" --label "sprint-1,backend,projects-bc,domain" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich typsichere ValueObjects für den Projects BC.

## Akzeptanzkriterien
- [ ] `ProjectId` (Guid, nicht Empty)
- [ ] `ProjectName` (string, max 200 Zeichen, nicht leer)
- [ ] `ProjectStatus` (Draft/Active/Completed/Archived – enum-artig)
- [ ] `ZoneId` (Guid)
- [ ] `ZoneName` (string, max 100 Zeichen)
- [ ] `ZoneType` (Building/Floor/Room/Trench – enum-artig)
- [ ] `Address` (Street, City, ZipCode – alle validated)
- [ ] `ClientInfo` (Name, ContactEmail, ContactPhone)
- [ ] Alle als sealed record : ValueObject mit Konstruktor-Validierung
- [ ] Unit Tests (Happy + Unhappy Path)

## Story Points: 3
EOF
)"
echo "Created BD-102"

gh issue create --title "BD-103 Business Rules für Project" --label "sprint-1,backend,projects-bc,domain" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich Business Rules im Domain Model für Projekte.

## Akzeptanzkriterien
- [ ] `ProjectMustHaveUniqueName` – IBusinessRule
- [ ] `ZoneNameMustBeUniqueWithinProject` – IBusinessRule
- [ ] Regeln werden via CheckRule() aufgerufen
- [ ] BusinessRuleException bei Verletzung
- [ ] Unit Tests für beide Regeln

## Story Points: 2
EOF
)"
echo "Created BD-103"

gh issue create --title "BD-104 CreateProject Command + Handler" --label "sprint-1,backend,projects-bc,application" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als API-Konsument möchte ich ein Projekt mit Name, Adresse und Auftraggeber anlegen.

## Akzeptanzkriterien
- [ ] `CreateProjectCommand` : ICommand<Guid>
- [ ] `CreateProjectHandler` : ICommandHandler<CreateProjectCommand, Guid>
- [ ] FluentValidation Validator für Command
- [ ] Handler erstellt Project über Factory Method
- [ ] Persistierung über IProjectRepository
- [ ] Gibt ProjectId (Guid) zurück
- [ ] Unit Tests mit NSubstitute Mock

## Story Points: 3
EOF
)"
echo "Created BD-104"

gh issue create --title "BD-105 AddZone Command + Handler" --label "sprint-1,backend,projects-bc,application" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als API-Konsument möchte ich einem Projekt eine Zone hinzufügen.

## Akzeptanzkriterien
- [ ] `AddZoneCommand` : ICommand (projectId, name, type, parentZoneId?)
- [ ] `AddZoneHandler` : ICommandHandler<AddZoneCommand>
- [ ] Handler lädt Project Aggregate, ruft project.AddZone() auf
- [ ] Validierung: Projekt muss existieren, Zone-Name eindeutig
- [ ] Unit Tests

## Story Points: 2
EOF
)"
echo "Created BD-105"

gh issue create --title "BD-106 GetProject + ListProjects Queries" --label "sprint-1,backend,projects-bc,application" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als API-Konsument möchte ich Projekte abrufen und paginiert auflisten.

## Akzeptanzkriterien
- [ ] `GetProjectQuery` : IQuery<ProjectDto> (projectId)
- [ ] `GetProjectHandler` gibt ProjectDto mit Zonen zurück
- [ ] `ListProjectsQuery` : IQuery<PagedResult<ProjectListItemDto>>
- [ ] `ListProjectsHandler` mit Paginierung und Textsuche
- [ ] DTOs sind flach (keine Domain-Objekte über API-Grenze)
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-106"

gh issue create --title "BD-107 EF Core Configuration + Migration" --label "sprint-1,backend,projects-bc,infrastructure" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich EF Core Mappings für das Project Aggregate.

## Akzeptanzkriterien
- [ ] `ProjectConfiguration` : IEntityTypeConfiguration<Project>
- [ ] `ZoneConfiguration` für Zone Entity
- [ ] Owned Types für Address, ClientInfo
- [ ] Value Conversions für alle IDs/ValueObjects
- [ ] Erste Migration generiert
- [ ] Seed-Daten: 1 Beispielprojekt mit 3 Zonen
- [ ] IProjectRepository + ProjectRepository

## Story Points: 3
EOF
)"
echo "Created BD-107"

gh issue create --title "BD-108 Minimal API Endpoints für Projects" --label "sprint-1,backend,projects-bc,api" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Frontend-Entwickler möchte ich REST-Endpoints für Projekte.

## Akzeptanzkriterien
- [ ] POST /api/projects → CreateProject (201 + ProjectId)
- [ ] GET /api/projects → ListProjects (200 + Paginiert)
- [ ] GET /api/projects/{id} → GetProject (200 + ProjectDto)
- [ ] POST /api/projects/{id}/zones → AddZone (201)
- [ ] Alle über IDispatcher
- [ ] MapProjectEndpoints() Extension Method
- [ ] RequireAuthorization() vorbereitet

## Story Points: 2
EOF
)"
echo "Created BD-108"

gh issue create --title "BD-109 Integration Test: Project CRUD" --label "sprint-1,backend,projects-bc,infrastructure" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich einen Integration Test für den kompletten Projekt-CRUD.

## Akzeptanzkriterien
- [ ] Testcontainers mit PostgreSQL
- [ ] Test: Projekt anlegen → laden → verifizieren
- [ ] Test: Zone hinzufügen → Projekt mit Zonen laden
- [ ] Domain Events werden korrekt dispatched
- [ ] Cleanup nach jedem Test

## Story Points: 3
EOF
)"
echo "Created BD-109"

gh issue create --title "BD-110 Expo Router Navigation Setup" --label "sprint-1,frontend" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich eine intuitive Navigation mit Tabs und Stack-Navigation.

## Akzeptanzkriterien
- [ ] Tab-Navigation: Projekte, Erfassung, Sync, Einstellungen
- [ ] Stack-Navigation innerhalb Projekte: Liste → Detail → Zonen
- [ ] Layout mit Header + Tab Bar
- [ ] Baustellentaugliche Icons (große Touch-Targets 48dp+)
- [ ] Typ-sichere Navigation mit Expo Router

## Story Points: 3
EOF
)"
echo "Created BD-110"

gh issue create --title "BD-111 Auth-Flow (Keycloak OIDC)" --label "sprint-1,frontend" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich mich mit meinem Firmen-Account einloggen können.

## Akzeptanzkriterien
- [ ] Login-Screen mit Anmelden-Button
- [ ] OAuth2/OIDC Flow mit expo-auth-session
- [ ] Token-Storage in expo-secure-store
- [ ] Auto-Token-Refresh
- [ ] Auth-Guard für geschützte Screens
- [ ] Logout-Funktion
- [ ] Loading-State während Auth-Check

## Story Points: 5
EOF
)"
echo "Created BD-111"

gh issue create --title "BD-112 Design-System Basis" --label "sprint-1,frontend" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich eine baustellentaugliche UI mit großen Touch-Targets und hohem Kontrast.

## Akzeptanzkriterien
- [ ] Theme: Farben, Typografie, Spacing
- [ ] Große Touch-Targets (48dp+), Handschuh-tauglich
- [ ] Hoher Kontrast für Sonnenlicht-Lesbarkeit
- [ ] Basis-Komponenten: Button, TextInput, Card, ListItem, Badge, EmptyState
- [ ] Konsistentes Styling über die gesamte App

## Story Points: 3
EOF
)"
echo "Created BD-112"

gh issue create --title "BD-113 Projekt-Liste Screen" --label "sprint-1,frontend,projects-bc" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich eine Übersicht meiner Projekte mit Suche und Pull-to-Refresh.

## Akzeptanzkriterien
- [ ] API-Anbindung mit @tanstack/react-query
- [ ] Pull-to-Refresh
- [ ] Suchfeld
- [ ] FAB Neues Projekt
- [ ] Ladezustand (Skeleton/Spinner)
- [ ] Fehlermeldung bei API-Fehler
- [ ] Empty State wenn keine Projekte

## Story Points: 3
EOF
)"
echo "Created BD-113"

gh issue create --title "BD-114 Projekt anlegen Screen" --label "sprint-1,frontend,projects-bc" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich ein neues Projekt mit Name, Adresse und Auftraggeber anlegen.

## Akzeptanzkriterien
- [ ] Formular mit zod-Validierung
- [ ] Felder: Name (Pflicht), Straße, PLZ, Ort, Auftraggeber, Kontakt
- [ ] Validierungsfehler inline
- [ ] Submit → POST /api/projects
- [ ] Erfolg → Zurück zur Liste
- [ ] Loading-State beim Submit
- [ ] Fehlerbehandlung

## Story Points: 3
EOF
)"
echo "Created BD-114"

gh issue create --title "BD-115 Projekt-Detail + Zonen Screen" --label "sprint-1,frontend,projects-bc" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich Projekt-Details und Zonen-Hierarchie einsehen und Zonen hinzufügen.

## Akzeptanzkriterien
- [ ] Projekt-Info anzeigen (Name, Adresse, Auftraggeber, Status)
- [ ] Zonen hierarchisch darstellen (Tree/verschachtelte Liste)
- [ ] Zone hinzufügen Button
- [ ] Typ-Auswahl: Gebäude, Stockwerk, Raum, Graben
- [ ] Parent-Zone auswählbar

## Story Points: 3
EOF
)"
echo "Created BD-115"

gh issue create --title "BD-116 API-Client Setup" --label "sprint-1,frontend" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich einen typsicheren API-Client mit automatischer Auth.

## Akzeptanzkriterien
- [ ] Axios-Instanz mit Base URL (konfigurierbar)
- [ ] Auth-Interceptor: Bearer Token aus SecureStore
- [ ] Error-Interceptor: 401 → Logout, Netzwerkfehler → Toast
- [ ] Type-safe API-Funktionen für Projects
- [ ] Response-Types passend zu Backend DTOs

## Story Points: 2
EOF
)"
echo "Created BD-116"

gh issue create --title "BD-117 OpenAPI Spec für Projects BC" --label "sprint-1,cross-cutting,projects-bc,api" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Frontend-Entwickler möchte ich eine OpenAPI Spec als Contract.

## Akzeptanzkriterien
- [ ] Swagger/Scalar UI auf Projects.Api
- [ ] OpenAPI 3.1 Spec generiert
- [ ] Alle Endpoints dokumentiert
- [ ] Erreichbar über API Gateway

## Story Points: 2
EOF
)"
echo "Created BD-117"

gh issue create --title "BD-118 E2E Smoke Test" --label "sprint-1,cross-cutting" --milestone "Sprint 1: Projects BC + Expo" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich einen E2E Test für den kompletten Flow.

## Akzeptanzkriterien
- [ ] Test: App starten → Login → Projekt anlegen → Liste → Öffnen → Zone hinzufügen
- [ ] Läuft gegen echtes Backend (Aspire)
- [ ] Detox oder ähnliches E2E Framework

## Story Points: 2
EOF
)"
echo "Created BD-118"

echo ""
echo "=== Creating Sprint 2 Issues ==="

gh issue create --title "BD-201 Installation Aggregate Root implementieren" --label "sprint-2,backend,documentation-bc,domain" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich ein Installation Aggregate Root für die Dokumentation elektrischer Installationen.

## Akzeptanzkriterien
- [ ] `Installation` als sealed class : AggregateRoot<InstallationId>
- [ ] Factory Method Installation.Create(id, projectId, zoneId, type, description, position, ...)
- [ ] Photo als Entity (vorbereitet für Sprint 3)
- [ ] Measurement als Entity (vorbereitet für Sprint 4)
- [ ] Domain Events: InstallationDocumented, InstallationUpdated
- [ ] Properties: Type, Manufacturer, Model, SerialNumber, CableSpec, Description, GpsPosition, Status
- [ ] Unit Tests

## Story Points: 5
EOF
)"
echo "Created BD-201"

gh issue create --title "BD-202 ValueObjects für Documentation BC" --label "sprint-2,backend,documentation-bc,domain" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich typsichere ValueObjects für den Documentation BC.

## Akzeptanzkriterien
- [ ] InstallationId (Guid)
- [ ] InstallationType (Cable, Socket, Switch, DistributionBoard, JunctionBox, etc.)
- [ ] InstallationStatus (InProgress, Completed, Inspected)
- [ ] GpsPosition (Lat, Lon, Alt, Accuracy, Source, CorrectionService, RtkFixStatus, SatelliteCount, Hdop, CorrectionAge)
- [ ] CableSpec (Type, CrossSection, Length, RoutePath)
- [ ] Depth (Value in mm, nicht negativ)
- [ ] Manufacturer, ModelName, SerialNumber
- [ ] Alle als sealed record : ValueObject
- [ ] Unit Tests

## Story Points: 5
EOF
)"
echo "Created BD-202"

gh issue create --title "BD-203 Business Rules für Installation" --label "sprint-2,backend,documentation-bc,domain" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich Geschäftsregeln für Installationen.

## Akzeptanzkriterien
- [ ] InstallationMustBelongToExistingProject
- [ ] InstallationMustHaveValidGpsPosition
- [ ] CompletedInstallationCannotBeModified
- [ ] Unit Tests

## Story Points: 2
EOF
)"
echo "Created BD-203"

gh issue create --title "BD-204 DocumentInstallation Command + Handler" --label "sprint-2,backend,documentation-bc,application" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich eine neue Installation dokumentieren.

## Akzeptanzkriterien
- [ ] DocumentInstallationCommand : ICommand<Guid>
- [ ] DocumentInstallationHandler mit FluentValidation
- [ ] Handler erstellt Installation über Factory Method
- [ ] IInstallationRepository Interface + Implementierung
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-204"

gh issue create --title "BD-205 GetInstallation + ListInstallations Queries" --label "sprint-2,backend,documentation-bc,application" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als API-Konsument möchte ich Installationen abrufen und filtern.

## Akzeptanzkriterien
- [ ] GetInstallationQuery → InstallationDto (mit Fotos + Messungen Übersicht)
- [ ] ListInstallationsQuery → paginiert, filterbar nach ProjectId, ZoneId, Type, Status
- [ ] DTOs mit allen relevanten Feldern
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-205"

gh issue create --title "BD-206 EF Core Configuration + Migration für Documentation BC" --label "sprint-2,backend,documentation-bc,infrastructure" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich EF Core Mappings für das Installation Aggregate.

## Akzeptanzkriterien
- [ ] InstallationConfiguration mit Value Conversions
- [ ] GpsPosition als Owned Type + PostGIS geometry-Spalte
- [ ] CableSpec als Owned Type
- [ ] Erste Migration
- [ ] InstallationRepository Implementierung

## Story Points: 3
EOF
)"
echo "Created BD-206"

gh issue create --title "BD-207 Minimal API Endpoints für Documentation" --label "sprint-2,backend,documentation-bc,api" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Frontend-Entwickler möchte ich REST-Endpoints für Installationen.

## Akzeptanzkriterien
- [ ] POST /api/documentation/installations → DocumentInstallation
- [ ] GET /api/documentation/installations → ListInstallations
- [ ] GET /api/documentation/installations/{id} → GetInstallation
- [ ] MapDocumentationEndpoints() Extension Method
- [ ] YARP-Routing aktualisiert

## Story Points: 2
EOF
)"
echo "Created BD-207"

gh issue create --title "BD-208 Installations-Liste Screen" --label "sprint-2,frontend,documentation-bc" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich alle Installationen eines Projekts/Zone sehen.

## Akzeptanzkriterien
- [ ] Liste mit Typ-Icon, Beschreibung, Status, Datum
- [ ] Filter nach Zone, Typ, Status
- [ ] Pull-to-Refresh
- [ ] FAB Neue Installation
- [ ] Navigation von Projekt-Detail aus

## Story Points: 3
EOF
)"
echo "Created BD-208"

gh issue create --title "BD-209 Installation erfassen Screen" --label "sprint-2,frontend,documentation-bc" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Elektriker möchte ich eine Installation mit allen Feldern schnell erfassen.

## Akzeptanzkriterien
- [ ] Formular: Typ (Picker), Hersteller, Modell, Seriennummer
- [ ] Kabelspezifikation: Typ, Querschnitt, Länge
- [ ] Freitext-Beschreibung
- [ ] GPS-Position automatisch erfasst und angezeigt
- [ ] Validierung mit zod
- [ ] Submit → POST /api/documentation/installations

## Story Points: 5
EOF
)"
echo "Created BD-209"

gh issue create --title "BD-210 GPS-Positionsanzeige Komponente" --label "sprint-2,frontend,documentation-bc" --milestone "Sprint 2: Documentation BC – Installation" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich die aktuelle GPS-Position mit Genauigkeitsanzeige sehen.

## Akzeptanzkriterien
- [ ] expo-location Integration
- [ ] Anzeige: Lat/Lon, Genauigkeit in Metern
- [ ] Visueller Genauigkeitsindikator (grün/gelb/rot)
- [ ] Position aktualisieren Button
- [ ] Position wird bei Installation-Erfassung mitgespeichert

## Story Points: 3
EOF
)"
echo "Created BD-210"

echo ""
echo "=== Creating Sprint 3 Issues ==="

gh issue create --title "BD-301 Photo Entity im Installation Aggregate" --label "sprint-3,backend,documentation-bc,domain" --milestone "Sprint 3: Foto-Erfassung" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich eine Photo Entity im Installation Aggregate.

## Akzeptanzkriterien
- [ ] Photo als Entity<PhotoId> innerhalb Installation Aggregate
- [ ] Properties: PhotoId, FileName, BlobUrl, ContentType, FileSize, Caption, PhotoType (Before/After/Detail), GpsPosition, TakenAt
- [ ] installation.AddPhoto() Methode
- [ ] Domain Event: PhotoAdded
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-301"

gh issue create --title "BD-302 AddPhoto Command + Handler" --label "sprint-3,backend,documentation-bc,application" --milestone "Sprint 3: Foto-Erfassung" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich Fotos zu einer Installation hinzufügen.

## Akzeptanzkriterien
- [ ] AddPhotoCommand : ICommand<Guid> (installationId, file, caption, photoType)
- [ ] Handler: Foto in BlobStorage hochladen, Photo Entity erstellen
- [ ] FluentValidation: Dateigröße, erlaubte Formate (jpg, png, heic)
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-302"

gh issue create --title "BD-303 PhotoBlobStorage Service" --label "sprint-3,backend,documentation-bc,infrastructure" --milestone "Sprint 3: Foto-Erfassung" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich eine Abstraktion für Foto-Speicherung in Blob Storage.

## Akzeptanzkriterien
- [ ] IPhotoStorage Interface (Upload, Download, Delete, GetUrl)
- [ ] AzureBlobPhotoStorage Implementierung
- [ ] Lokaler Fallback für Entwicklung (Dateisystem)
- [ ] Automatische Thumbnail-Generierung (optional)
- [ ] Unit Tests mit Mock

## Story Points: 3
EOF
)"
echo "Created BD-303"

gh issue create --title "BD-304 Photo Endpoints" --label "sprint-3,backend,documentation-bc,api" --milestone "Sprint 3: Foto-Erfassung" --body "$(cat <<'EOF'
## User Story
Als Frontend-Entwickler möchte ich Fotos hoch- und runterladen können.

## Akzeptanzkriterien
- [ ] POST /api/documentation/installations/{id}/photos → Upload (multipart)
- [ ] GET /api/documentation/installations/{id}/photos → Foto-Liste
- [ ] GET /api/documentation/photos/{photoId} → Download/URL
- [ ] DELETE /api/documentation/photos/{photoId} → Löschen

## Story Points: 2
EOF
)"
echo "Created BD-304"

gh issue create --title "BD-305 expo-camera Integration" --label "sprint-3,frontend,documentation-bc" --milestone "Sprint 3: Foto-Erfassung" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich direkt aus der App Fotos aufnehmen.

## Akzeptanzkriterien
- [ ] expo-camera/expo-image-picker Integration
- [ ] Kamera-Screen mit Auslöser
- [ ] Vorschau nach Aufnahme (Behalten/Verwerfen)
- [ ] Automatische EXIF-Metadaten (GPS, Zeitstempel)
- [ ] Foto-Typ Auswahl: Vorher / Nachher / Detail

## Story Points: 3
EOF
)"
echo "Created BD-305"

gh issue create --title "BD-306 Foto-Upload + Vorschau" --label "sprint-3,frontend,documentation-bc" --milestone "Sprint 3: Foto-Erfassung" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich aufgenommene Fotos mit Beschreibung hochladen.

## Akzeptanzkriterien
- [ ] Upload nach Aufnahme/Auswahl
- [ ] Fortschrittsanzeige während Upload
- [ ] Caption-Eingabefeld
- [ ] Fehlerbehandlung + Retry

## Story Points: 2
EOF
)"
echo "Created BD-306"

gh issue create --title "BD-307 Foto-Galerie pro Installation" --label "sprint-3,frontend,documentation-bc" --milestone "Sprint 3: Foto-Erfassung" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich alle Fotos einer Installation als Galerie sehen.

## Akzeptanzkriterien
- [ ] Thumbnail-Grid auf Installations-Detail
- [ ] Vollbild-Ansicht mit Wischgeste
- [ ] Foto-Typ Badge (Vorher/Nachher/Detail)
- [ ] Caption anzeigen
- [ ] Foto löschen (mit Bestätigung)

## Story Points: 3
EOF
)"
echo "Created BD-307"

echo ""
echo "=== Creating Sprint 4 Issues ==="

gh issue create --title "BD-401 Measurement Entity + ValueObjects" --label "sprint-4,backend,documentation-bc,domain" --milestone "Sprint 4: Messungen + Prüfungen" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich Messungen als Entities im Installation Aggregate.

## Akzeptanzkriterien
- [ ] Measurement als Entity<MeasurementId>
- [ ] MeasurementType (InsulationResistance, LoopImpedance, RcdTripTime, RcdTripCurrent, ContinuityResistance)
- [ ] MeasurementValue (Value, Unit, MinThreshold, MaxThreshold)
- [ ] MeasurementResult (Passed/Failed/Warning)
- [ ] installation.RecordMeasurement() Methode
- [ ] Domain Event: MeasurementRecorded
- [ ] Automatische Bewertung: Passed wenn innerhalb Grenzwerte
- [ ] Unit Tests

## Story Points: 5
EOF
)"
echo "Created BD-401"

gh issue create --title "BD-402 Business Rules für Messungen" --label "sprint-4,backend,documentation-bc,domain" --milestone "Sprint 4: Messungen + Prüfungen" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich Geschäftsregeln für Messwerte.

## Akzeptanzkriterien
- [ ] MeasurementValueMustBePositive
- [ ] MeasurementTypeMustMatchInstallationType
- [ ] Grenzwerte nach VDE-Normen als Referenz
- [ ] Unit Tests

## Story Points: 2
EOF
)"
echo "Created BD-402"

gh issue create --title "BD-403 RecordMeasurement Command + Handler" --label "sprint-4,backend,documentation-bc,application" --milestone "Sprint 4: Messungen + Prüfungen" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich Messwerte zu einer Installation erfassen.

## Akzeptanzkriterien
- [ ] RecordMeasurementCommand : ICommand<Guid>
- [ ] Handler: Installation laden, Messwert hinzufügen, speichern
- [ ] FluentValidation
- [ ] Unit Tests

## Story Points: 2
EOF
)"
echo "Created BD-403"

gh issue create --title "BD-404 GetMeasurements Query + Handler" --label "sprint-4,backend,documentation-bc,application" --milestone "Sprint 4: Messungen + Prüfungen" --body "$(cat <<'EOF'
## User Story
Als API-Konsument möchte ich Messwerte einer Installation abrufen.

## Akzeptanzkriterien
- [ ] GetMeasurementsQuery : IQuery<List<MeasurementDto>>
- [ ] MeasurementDto: Typ, Wert, Einheit, Ergebnis, Zeitstempel
- [ ] Unit Tests

## Story Points: 2
EOF
)"
echo "Created BD-404"

gh issue create --title "BD-405 Measurement Endpoints" --label "sprint-4,backend,documentation-bc,api" --milestone "Sprint 4: Messungen + Prüfungen" --body "$(cat <<'EOF'
## User Story
Als Frontend-Entwickler möchte ich REST-Endpoints für Messungen.

## Akzeptanzkriterien
- [ ] POST /api/documentation/installations/{id}/measurements → RecordMeasurement
- [ ] GET /api/documentation/installations/{id}/measurements → GetMeasurements
- [ ] Korrekte HTTP Status Codes

## Story Points: 2
EOF
)"
echo "Created BD-405"

gh issue create --title "BD-406 Messwert-Erfassung Screen" --label "sprint-4,frontend,documentation-bc" --milestone "Sprint 4: Messungen + Prüfungen" --body "$(cat <<'EOF'
## User Story
Als Elektriker möchte ich Messwerte schnell erfassen.

## Akzeptanzkriterien
- [ ] Messtyp-Auswahl (Picker)
- [ ] Numerisches Eingabefeld
- [ ] Einheit automatisch nach Typ
- [ ] Grenzwerte anzeigen
- [ ] Sofortige Bewertung: Bestanden / Nicht bestanden
- [ ] Submit → POST Endpoint
- [ ] Großer Speichern-Button (Handschuh-tauglich)

## Story Points: 3
EOF
)"
echo "Created BD-406"

gh issue create --title "BD-407 Bestanden/Nicht-bestanden Anzeige" --label "sprint-4,frontend,documentation-bc" --milestone "Sprint 4: Messungen + Prüfungen" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich auf einen Blick sehen, ob Messungen bestanden haben.

## Akzeptanzkriterien
- [ ] Grün/Rot Badge pro Messung
- [ ] Zusammenfassung: 3/4 Messungen bestanden
- [ ] Warnung wenn nicht bestanden
- [ ] Übersicht aller Messungen pro Installation

## Story Points: 2
EOF
)"
echo "Created BD-407"

echo ""
echo "=== Creating Sprint 5 Issues ==="

gh issue create --title "BD-501 SQLite Schema: Projects + Zones" --label "sprint-5,frontend" --milestone "Sprint 5: Offline-Datenbank" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich Projekte und Zonen offline nutzen.

## Akzeptanzkriterien
- [ ] Drizzle ORM Setup mit expo-sqlite
- [ ] projects Tabelle (id, name, street, city, zip, clientName, status, syncStatus, timestamps)
- [ ] zones Tabelle (id, projectId, name, type, parentZoneId, syncStatus)
- [ ] CRUD-Operationen auf SQLite
- [ ] Migration-System

## Story Points: 5
EOF
)"
echo "Created BD-501"

gh issue create --title "BD-502 SQLite Schema: Installations, Photos, Measurements" --label "sprint-5,frontend" --milestone "Sprint 5: Offline-Datenbank" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich Installationen, Fotos und Messungen offline erfassen.

## Akzeptanzkriterien
- [ ] installations Tabelle (alle Felder inkl. GPS als JSON)
- [ ] photos Tabelle (id, installationId, localUri, remoteUrl, caption, photoType, syncStatus)
- [ ] measurements Tabelle (id, installationId, type, value, unit, result, syncStatus)
- [ ] Fotos lokal im Dateisystem, Pfad in SQLite
- [ ] CRUD-Operationen

## Story Points: 5
EOF
)"
echo "Created BD-502"

gh issue create --title "BD-503 Outbox-Tabelle für Sync" --label "sprint-5,frontend,sync-bc" --milestone "Sprint 5: Offline-Datenbank" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich eine Outbox-Tabelle für Sync-Tracking.

## Akzeptanzkriterien
- [ ] sync_outbox (id, entityType, entityId, operation, payload, status, createdAt, syncedAt)
- [ ] Jede lokale CRUD-Operation schreibt Outbox-Eintrag
- [ ] Status: pending, syncing, synced, failed
- [ ] FIFO-Abarbeitung bei Sync

## Story Points: 3
EOF
)"
echo "Created BD-503"

gh issue create --title "BD-504 Offline-Repository Layer" --label "sprint-5,frontend" --milestone "Sprint 5: Offline-Datenbank" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich eine Repository-Abstraktion für offline/online.

## Akzeptanzkriterien
- [ ] Repository-Interface pro Entity
- [ ] Offline: Liest/schreibt SQLite
- [ ] Online: Liest Server, schreibt lokal + Outbox
- [ ] Zustand Store integriert

## Story Points: 5
EOF
)"
echo "Created BD-504"

gh issue create --title "BD-505 Offline-First Datenfluss" --label "sprint-5,frontend" --milestone "Sprint 5: Offline-Datenbank" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich die App nahtlos online und offline nutzen.

## Akzeptanzkriterien
- [ ] Zustand Store als Single Source of Truth
- [ ] TanStack Query mit SQLite als Cache-Layer
- [ ] Optimistic Updates bei Schreiboperationen
- [ ] Automatischer Fallback auf lokale Daten
- [ ] Seamless Transition online/offline

## Story Points: 5
EOF
)"
echo "Created BD-505"

gh issue create --title "BD-506 Offline-Indikator UI" --label "sprint-5,frontend" --milestone "Sprint 5: Offline-Datenbank" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich sehen, ob ich online/offline bin und ob es ungesyncte Daten gibt.

## Akzeptanzkriterien
- [ ] Statusleiste: Online/Offline Indikator
- [ ] Badge: Anzahl ungesyncter Änderungen
- [ ] Banner bei Offline-Wechsel
- [ ] Sync-Tab zeigt ausstehende Änderungen

## Story Points: 2
EOF
)"
echo "Created BD-506"

echo ""
echo "=== Creating Sprint 6 Issues ==="

gh issue create --title "BD-601 Sync Domain Model" --label "sprint-6,backend,sync-bc,domain" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich ein Domain Model für Offline-Synchronisation.

## Akzeptanzkriterien
- [ ] SyncBatch Aggregate Root (BatchId, DeviceId, Timestamp, Status)
- [ ] SyncDelta Entity (EntityType, EntityId, Operation, Payload, Version)
- [ ] ConflictRecord Entity (ConflictType, ServerVersion, ClientVersion, Resolution)
- [ ] ValueObjects: DeviceId, SyncStatus, ConflictType
- [ ] Domain Events: SyncBatchProcessed, ConflictDetected, ConflictResolved
- [ ] Unit Tests

## Story Points: 5
EOF
)"
echo "Created BD-601"

gh issue create --title "BD-602 ProcessSyncBatch Command + Handler" --label "sprint-6,backend,sync-bc,application" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Sync-Client möchte ich lokale Änderungen als Batch senden.

## Akzeptanzkriterien
- [ ] ProcessSyncBatchCommand : ICommand<SyncBatchResultDto>
- [ ] Handler: Batch empfangen, Konflikte erkennen, Änderungen anwenden
- [ ] Field-Level-Merge bei verschiedenen Feldern
- [ ] Konflikte zurückmelden bei gleichen Feldern
- [ ] Unit Tests

## Story Points: 5
EOF
)"
echo "Created BD-602"

gh issue create --title "BD-603 GetChangesSince Query + Handler" --label "sprint-6,backend,sync-bc,application" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Sync-Client möchte ich Server-Änderungen seit letztem Sync abrufen.

## Akzeptanzkriterien
- [ ] GetChangesSinceQuery : IQuery<DeltaSyncResultDto>
- [ ] Alle Änderungen seit Timestamp
- [ ] Paginierung bei großen Deltas
- [ ] Nur projektbezogene Entitäten
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-603"

gh issue create --title "BD-604 ResolveConflict Command + Handler" --label "sprint-6,backend,sync-bc,application" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich Sync-Konflikte manuell auflösen.

## Akzeptanzkriterien
- [ ] ResolveConflictCommand : ICommand (conflictId, resolution)
- [ ] Resolution: KeepServer / KeepClient / Merge
- [ ] Conflict als resolved markieren
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-604"

gh issue create --title "BD-605 Delta-Sync API Endpoints" --label "sprint-6,backend,sync-bc,api" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Frontend-Entwickler möchte ich REST-Endpoints für Synchronisation.

## Akzeptanzkriterien
- [ ] POST /api/sync/batch → ProcessSyncBatch
- [ ] GET /api/sync/changes?since={timestamp} → GetChangesSince
- [ ] POST /api/sync/conflicts/{id}/resolve → ResolveConflict
- [ ] GET /api/sync/conflicts → Offene Konflikte
- [ ] YARP-Routing konfiguriert

## Story Points: 2
EOF
)"
echo "Created BD-605"

gh issue create --title "BD-606 SyncManager Service (Frontend)" --label "sprint-6,frontend,sync-bc" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich einen SyncManager der Outbox abarbeitet.

## Akzeptanzkriterien
- [ ] SyncManager als Singleton-Service
- [ ] Push-Sync: Outbox als Batch an Server
- [ ] Pull-Sync: Server-Änderungen lokal einpflegen
- [ ] Bidirektionaler Sync
- [ ] Error Handling: Retry mit Backoff
- [ ] Sync-Status pro Entity

## Story Points: 5
EOF
)"
echo "Created BD-606"

gh issue create --title "BD-607 Automatischer Sync bei Reconnect" --label "sprint-6,frontend,sync-bc" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich automatischen Sync bei Netzwerk-Reconnect.

## Akzeptanzkriterien
- [ ] NetInfo-Listener
- [ ] Auto-Sync bei offline → online
- [ ] Periodischer Sync (5/15/30 Min konfigurierbar)
- [ ] Manueller Sync-Button

## Story Points: 3
EOF
)"
echo "Created BD-607"

gh issue create --title "BD-608 Konflikt-UI" --label "sprint-6,frontend,sync-bc" --milestone "Sprint 6: Sync-Engine" --body "$(cat <<'EOF'
## User Story
Als Benutzer möchte ich Sync-Konflikte verstehen und auflösen.

## Akzeptanzkriterien
- [ ] Konflikt-Liste Screen (Badge im Sync-Tab)
- [ ] Server vs. Client nebeneinander
- [ ] Geänderte Felder hervorgehoben
- [ ] Optionen: Server / Meine Version / Zusammenführen
- [ ] Bestätigungsdialog

## Story Points: 5
EOF
)"
echo "Created BD-608"

echo ""
echo "=== Creating Sprint 7 Issues ==="

gh issue create --title "BD-701 InitUpload Endpoint" --label "sprint-7,backend,documentation-bc,api" --milestone "Sprint 7: Chunked Photo Upload" --body "$(cat <<'EOF'
## User Story
Als Client möchte ich einen Foto-Upload initiieren.

## Akzeptanzkriterien
- [ ] POST /api/documentation/uploads/init → UploadSessionDto
- [ ] Session in Redis/DB (TTL: 24h)
- [ ] Metadaten: InstallationId, FileName, ContentType, TotalSize
- [ ] Validierung: Dateigröße, Formate

## Story Points: 3
EOF
)"
echo "Created BD-701"

gh issue create --title "BD-702 UploadChunk Endpoint" --label "sprint-7,backend,documentation-bc,api" --milestone "Sprint 7: Chunked Photo Upload" --body "$(cat <<'EOF'
## User Story
Als Client möchte ich ein Foto in Chunks hochladen.

## Akzeptanzkriterien
- [ ] PUT /api/documentation/uploads/{sessionId}/chunks/{index}
- [ ] Chunk in temporären Storage
- [ ] Validierung: Reihenfolge, Größe, Checksum
- [ ] Fortschritt tracken

## Story Points: 3
EOF
)"
echo "Created BD-702"

gh issue create --title "BD-703 CompleteUpload + Chunk Assembly" --label "sprint-7,backend,documentation-bc,api,infrastructure" --milestone "Sprint 7: Chunked Photo Upload" --body "$(cat <<'EOF'
## User Story
Als Client möchte ich einen Upload abschließen.

## Akzeptanzkriterien
- [ ] POST /api/documentation/uploads/{sessionId}/complete
- [ ] Chunks zusammenfügen
- [ ] In Blob Storage verschieben
- [ ] Photo Entity erstellen
- [ ] Temporäre Chunks aufräumen

## Story Points: 3
EOF
)"
echo "Created BD-703"

gh issue create --title "BD-704 Resume-Logik für abgebrochene Uploads" --label "sprint-7,backend,documentation-bc,application" --milestone "Sprint 7: Chunked Photo Upload" --body "$(cat <<'EOF'
## User Story
Als Client möchte ich abgebrochene Uploads fortsetzen.

## Akzeptanzkriterien
- [ ] GET /api/documentation/uploads/{sessionId}/status → Fehlende Chunks
- [ ] Client sendet fehlende Chunks nach
- [ ] Session-Timeout konfigurierbar (24h Default)
- [ ] Abgelaufene Sessions aufräumen (Background Job)

## Story Points: 3
EOF
)"
echo "Created BD-704"

gh issue create --title "BD-705 Foto-Upload mit Fortschrittsanzeige" --label "sprint-7,frontend,documentation-bc" --milestone "Sprint 7: Chunked Photo Upload" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich den Upload-Fortschritt sehen.

## Akzeptanzkriterien
- [ ] Chunked Upload Client (Init → Chunks → Complete)
- [ ] Fortschrittsbalken pro Foto
- [ ] Queue für mehrere Fotos
- [ ] Upload im Hintergrund
- [ ] Notification bei Fertigstellung

## Story Points: 5
EOF
)"
echo "Created BD-705"

gh issue create --title "BD-706 Hintergrund-Upload + Resume" --label "sprint-7,frontend,documentation-bc" --milestone "Sprint 7: Chunked Photo Upload" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich Uploads nach App-Neustart fortsetzen.

## Akzeptanzkriterien
- [ ] Upload-Queue in SQLite persistiert
- [ ] Resume nach App-Neustart
- [ ] Resume nach Netzwerkunterbrechung
- [ ] Fehlgeschlagene Uploads markieren + Retry
- [ ] Upload-Übersicht im Sync-Tab

## Story Points: 3
EOF
)"
echo "Created BD-706"

echo ""
echo "=== Creating Sprint 8 Issues ==="

gh issue create --title "BD-801 GPS-Metadaten in PostGIS persistieren" --label "sprint-8,backend,documentation-bc,infrastructure" --milestone "Sprint 8: GPS-Metadaten + MVP Release" --body "$(cat <<'EOF'
## User Story
Als Entwickler möchte ich GPS-Positionen als PostGIS-Geometrien speichern.

## Akzeptanzkriterien
- [ ] geometry(Point, 4326) Spalte in installations
- [ ] Alle GPS-Metadaten in separaten Spalten
- [ ] Migration aktualisiert
- [ ] Spatial Index
- [ ] NetTopologySuite Integration

## Story Points: 3
EOF
)"
echo "Created BD-801"

gh issue create --title "BD-802 Spatial Queries" --label "sprint-8,backend,documentation-bc,application" --milestone "Sprint 8: GPS-Metadaten + MVP Release" --body "$(cat <<'EOF'
## User Story
Als API-Konsument möchte ich Installationen nach Geo-Nähe suchen.

## Akzeptanzkriterien
- [ ] GetInstallationsInRadiusQuery (lat, lon, radius)
- [ ] GetInstallationsInBoundingBoxQuery
- [ ] GET /api/documentation/installations/nearby
- [ ] PostGIS ST_DWithin / ST_Within
- [ ] Unit Tests

## Story Points: 3
EOF
)"
echo "Created BD-802"

gh issue create --title "BD-803 GPS-Daten Validierung + Qualitäts-Scoring" --label "sprint-8,backend,documentation-bc,domain" --milestone "Sprint 8: GPS-Metadaten + MVP Release" --body "$(cat <<'EOF'
## User Story
Als System möchte ich GPS-Positionen nach Qualität bewerten.

## Akzeptanzkriterien
- [ ] GpsQualityScore: A (<1m) / B (1-5m) / C (5-30m) / D (>30m)
- [ ] Faktoren: Accuracy, HDOP, Satellites, CorrectionService
- [ ] Business Rule: Warnung bei Qualität D
- [ ] QualityScore wird mitgespeichert

## Story Points: 2
EOF
)"
echo "Created BD-803"

gh issue create --title "BD-804 GPS-Genauigkeitsanzeige (QualityIndicator)" --label "sprint-8,frontend,documentation-bc" --milestone "Sprint 8: GPS-Metadaten + MVP Release" --body "$(cat <<'EOF'
## User Story
Als Erfasser möchte ich die GPS-Qualität visuell einschätzen.

## Akzeptanzkriterien
- [ ] QualityIndicator Komponente
- [ ] Farbcodierung: Grün (A), Gelb (B), Orange (C), Rot (D)
- [ ] Anzeige: Genauigkeit, Satellitenanzahl, Quelle
- [ ] Warnung bei schlechter Qualität
- [ ] Live-Update

## Story Points: 3
EOF
)"
echo "Created BD-804"

gh issue create --title "BD-805 Mock Location Support (Android)" --label "sprint-8,frontend,documentation-bc" --milestone "Sprint 8: GPS-Metadaten + MVP Release" --body "$(cat <<'EOF'
## User Story
Als Erfasser mit externem GNSS-Empfänger möchte ich dessen Position nutzen.

## Akzeptanzkriterien
- [ ] Erkennung Mock Location Provider (Android)
- [ ] Automatische Nutzung der Mock Location
- [ ] Anzeige GPS-Quelle: Intern vs. Extern
- [ ] Einstellungen: Mock Location aktivieren/deaktivieren
- [ ] iOS-Hinweis (kein Mock Location)

## Story Points: 3
EOF
)"
echo "Created BD-805"

gh issue create --title "BD-806 MVP Stabilisierung + Release-Vorbereitung" --label "sprint-8,cross-cutting" --milestone "Sprint 8: GPS-Metadaten + MVP Release" --body "$(cat <<'EOF'
## User Story
Als Team möchten wir den MVP stabilisieren und releasen.

## Akzeptanzkriterien
- [ ] Alle kritischen Bugs behoben
- [ ] Performance: App-Start < 3s
- [ ] Error Handling durchgängig
- [ ] Logging ausreichend
- [ ] README mit Setup-Anleitung
- [ ] App-Icons und Splash Screen
- [ ] EAS Build konfiguriert (Android)
- [ ] Smoke Test auf echtem Gerät

## Story Points: 5
EOF
)"
echo "Created BD-806"

echo ""
echo "=== DONE: All issues created ==="
gh issue list --limit 100 --state open | wc -l
