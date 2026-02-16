# CLAUDE.md – BauDoku Project Context

## Was ist BauDoku?

Mobile App zur Dokumentation elektrischer Installationen auf Baustellen. Zielgruppe: Elektriker, Tiefbauer, Elektroplaner, Bauleiter. Offline-fähige Erfassung von Elektrokomponenten mit Fotos, GPS-Position, Maßangaben und Prüfprotokollen.

## Tech-Stack

- **Frontend:** React Native + Expo (TypeScript), Expo Router, SQLite (Drizzle ORM), Zustand, TanStack Query
- **Backend:** .NET 10 Minimal API (C#), DDD + Clean Architecture, eigener Dispatcher (KEIN MediatR/Wolverine)
- **Datenbank:** PostgreSQL + PostGIS (Backend), SQLite (Mobile Offline)
- **Auth:** Keycloak (OAuth2 + OIDC)
- **Messaging:** RabbitMQ
- **Orchestrierung:** .NET Aspire (lokal), Kubernetes (Production)
- **CI/CD:** GitHub Actions

## Architektur-Prinzipien

### DDD + Clean Architecture (Backend)

- **Keine primitiven Typen im Domain Model** – nur ValueObjects
- **`sealed record` für ValueObjects** mit Konstruktor-Properties (NICHT positional records)
- **Domain Events** in Vergangenheitsform: `InstallationDocumented`, `PhotoAdded`, `MeasurementRecorded`
- **Aggregate Roots** als Transaktionsgrenzen
- **Eine Datei pro Klasse**
- **Eigener Dispatcher** statt MediatR – DI-basiert über `IServiceProvider`
- **Dependency Rule:** Domain → ∅, Application → Domain, Infrastructure → Application, Api → Infrastructure

### ValueObject-Pattern

```csharp
// Basis: abstract record mit IValueObject Marker-Interface
public interface IValueObject;
public abstract record ValueObject : IValueObject;

// Konkretes ValueObject: sealed record, private Konstruktor, statische Factory-Methode, Guard-Validierung
public sealed record ProjectName : ValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    private ProjectName(string value) => Value = value;

    public static ProjectName From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Projektname darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Projektname darf max. {MaxLength} Zeichen lang sein.");
        return new ProjectName(value);
    }
}

// ID-ValueObject (Namenskonvention: *Identifier, nicht *Id)
public sealed record ProjectIdentifier : ValueObject
{
    public Guid Value { get; }
    private ProjectIdentifier(Guid value) => Value = value;

    public static ProjectIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Projekt-ID darf nicht leer sein.");
        return new ProjectIdentifier(value);
    }
    public static ProjectIdentifier New() => new(Guid.NewGuid());
}

// Enum-artige ValueObjects (statt C# enums)
public sealed record InstallationStatus : ValueObject
{
    private static readonly HashSet<string> ValidValues = ["in_progress", "completed", "inspected"];

    public static readonly InstallationStatus InProgress = new("in_progress");
    public static readonly InstallationStatus Completed = new("completed");
    public static readonly InstallationStatus Inspected = new("inspected");

    public string Value { get; }
    private InstallationStatus(string value) => Value = value;

    public static InstallationStatus From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace().IsOneOf(ValidValues);
        return new InstallationStatus(value);
    }
}
```

### Guard/Ensure-Pattern (Fluent Validierung für ValueObjects)

```csharp
// Zero-Allocation ref structs mit [CallerArgumentExpression]
// Deklariert in: BuildingBlocks/BauDoku.BuildingBlocks.Domain/Guards/
Ensure.That(value).IsNotNullOrWhiteSpace().MaxLengthIs(200);     // StringGuard
Ensure.That(id).IsNotEmpty();                                      // GuidGuard
Ensure.That(latitude).IsBetween(-90.0, 90.0);                    // NumericGuard
Ensure.That(reference).IsNotNull();                                // ReferenceGuard
```

### ValueObject JSON-Serialisierung

```csharp
// ValueObjectJsonConverterFactory in BuildingBlocks.Infrastructure/Serialization/
// Erkennt IValueObject-Typen, liest/schreibt die Value-Property, ruft From() zur Deserialisierung
// Registrierung in jeder API Program.cs:
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new ValueObjectJsonConverterFactory()));
```

### Entity + AggregateRoot

```csharp
public abstract class Entity<TId> where TId : ValueObject
{
    public TId Id { get; protected set; } = default!;
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : ValueObject
{
    private readonly List<IDomainEvent> domainEvents = [];  // kein Underscore-Prefix!
    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();
    protected void AddDomainEvent(IDomainEvent domainEvent) => domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => domainEvents.Clear();

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken()) throw new BusinessRuleException(rule);
    }
}
```

### Aggregate Root Factory Pattern

Aggregate Roots werden NICHT über Konstruktoren erstellt, sondern über statische Factory Methods:

```csharp
public sealed class Project : AggregateRoot<ProjectIdentifier>
{
    private readonly List<Zone> zones = [];

    public ProjectName Name { get; private set; } = default!;
    public IReadOnlyList<Zone> Zones => zones.AsReadOnly();

    private Project() { } // EF Core

    public static Project Create(ProjectIdentifier id, ProjectName name, Address address, ClientInfo client)
    {
        var project = new Project { Id = id, Name = name, ... };
        project.AddDomainEvent(new ProjectCreated(id, name, DateTime.UtcNow));
        return project;
    }
}
```

### Eigener Dispatcher (KEIN MediatR)

```csharp
public interface IDispatcher
{
    Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken ct = default);
    Task Send(ICommand command, CancellationToken ct = default);
    Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken ct = default);
    Task Publish(IDomainEvent domainEvent, CancellationToken ct = default);
}

// Handler werden per DI aufgelöst (Scrutor Assembly-Scanning)
// Registration: services.AddDispatcher(typeof(Program).Assembly, ...);
```

### Minimal API Endpoints (KEINE Controller)

```csharp
public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects").RequireAuthorization();
        group.MapPost("/", async (CreateProjectCommand cmd, IDispatcher dispatcher, CancellationToken ct) => ...);
        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) => ...);
    }
}
```

## Solution-Struktur

```
src/backend/
├── BuildingBlocks/
│   ├── BauDoku.BuildingBlocks.Domain/          # ValueObject, IValueObject, Entity, AggregateRoot, Guards/Ensure, IDomainEvent, IBusinessRule
│   ├── BauDoku.BuildingBlocks.Application/     # Dispatcher, ICommand/IQuery, Behaviors (Validation, Logging, Transaction)
│   ├── BauDoku.BuildingBlocks.Infrastructure/  # BaseDbContext, UnitOfWork, Messaging, ValueObjectJsonConverterFactory
│   └── BauDoku.ServiceDefaults/                # Aspire ServiceDefaults (OpenTelemetry, Health Checks, Service Discovery)
├── Services/
│   ├── Documentation/                          # BC: Installationsdokumentation
│   │   ├── BauDoku.Documentation.Domain/       # Installation (AggregateRoot), Photo, Measurement (Entities), GpsPosition, CableSpec, Depth (ValueObjects)
│   │   ├── BauDoku.Documentation.Application/  # Commands + Queries + Handlers + Validators + EventHandlers
│   │   ├── BauDoku.Documentation.Infrastructure/ # DbContext, EF Configs, Repositories, BlobStorage
│   │   └── BauDoku.Documentation.Api/          # Minimal API Endpoints
│   ├── Projects/                               # BC: Projektverwaltung
│   │   ├── BauDoku.Projects.Domain/            # Project (AggregateRoot), Zone (Entity), ProjectName, Address, ZoneType (ValueObjects)
│   │   ├── BauDoku.Projects.Application/
│   │   ├── BauDoku.Projects.Infrastructure/
│   │   └── BauDoku.Projects.Api/
│   └── Sync/                                   # BC: Offline-Synchronisation
│       ├── BauDoku.Sync.Domain/
│       ├── BauDoku.Sync.Application/
│       ├── BauDoku.Sync.Infrastructure/
│       └── BauDoku.Sync.Api/
├── ApiGateway/BauDoku.ApiGateway/              # YARP Reverse Proxy
├── AppHost/BauDoku.AppHost/                    # .NET Aspire
└── BauDoku.slnx
```

## Bounded Contexts

1. **Projects** – Projektverwaltung, Zonen-Hierarchie (Gebäude → Stockwerk → Raum → Graben)
2. **Documentation** – Installations-Erfassung, Fotos, Messungen, GPS-Positionen, Kabelspezifikationen
3. **Sync** – Offline-Synchronisation (Outbox Pattern, Delta-Sync, Konfliktauflösung)

## Coding-Standards

- **C#:** `sealed record` für ValueObjects, `sealed class` für Handlers, `private set` für Entity-Properties
- **ValueObjects:** Private Konstruktoren + statische Factory-Methoden (`From()`, `Create()`, `New()`), Guard-Validierung, `IValueObject` Marker-Interface
- **ID-Naming:** `*Identifier` (z.B. `ProjectIdentifier`, `InstallationIdentifier`), NICHT `*Id`
- **Namespaces:** `BauDoku.{BC}.{Layer}` z.B. `BauDoku.Projects.Domain.Aggregates.Project`
- **Private fields:** `camelCase` OHNE Underscore-Prefix (z.B. `domainEvents`, `photos`, `zones`)
- **Commits:** Conventional Commits (`feat(projects): add Project aggregate`)
- **Ticket-Prefix:** `BD-xxx`
- **Tests:** xUnit + AwesomeAssertions + NSubstitute + Testcontainers (PostgreSQL)
- **Keine Primitive im Domain:** `ProjectIdentifier` statt `Guid`, `ProjectName` statt `string`, `Depth` statt `int`
- **Repository-Variablen** wie Collections benennen: `IProjectRepository` → `projects`, `IInstallationRepository` → `installations` (nicht `projectRepository`)
- **Frontend:** Branded Types (`src/frontend/src/types/branded.ts`), `type` statt `interface` für Daten-Shapes

## NuGet Packages (Central Package Management)

- EntityFrameworkCore 10.0.2, Npgsql.EFCore.PostgreSQL 10.0.0, NetTopologySuite
- FluentValidation 12.1.1, Scrutor 7.0.0
- Polly 8.*, Serilog.AspNetCore 10.0.0
- RabbitMQ.Client 7.*, Azure.Storage.Blobs 12.*
- Yarp.ReverseProxy 2.3.0, Aspire.Hosting.* 13.1.0
- xUnit 2.9.3, AwesomeAssertions 9.3.0, Testcontainers.PostgreSql 4.*, NSubstitute 5.*

## GPS/GNSS-Kontext

Die App unterstützt ein dreistufiges Positionierungskonzept:
- **Stufe 1:** Internes Smartphone-GPS (~3–5 m) – immer verfügbar
- **Stufe 2:** Ext. DGNSS + SAPOS-EPS (~30–50 cm) – MVP-Ziel, ~150€ Hardware
- **Stufe 3:** Ext. RTK + SAPOS-HEPS (~1–3 cm) – professionelle Vermessung

`GpsPosition` ist ein komplexes ValueObject mit: Latitude, Longitude, Altitude, HorizontalAccuracy, Source, CorrectionService, RtkFixStatus, SatelliteCount, Hdop, CorrectionAge.

## Aktuelle Phase

**Sprint 7+: Refactoring & Observability** – ValueObject-Pattern mit Guard-Validierung, IValueObject-Interface, *Identifier-Naming, Serilog-Logging, Aspire-Dashboard-Monitoring, Health Checks, Business Metrics.

## Referenz-Architektur

Das Projekt folgt dem Stil der **Orange Car Rental Solution** (github.com/smartsolutionslab/orange-car-rental-solution) – ebenfalls .NET 10, DDD, Clean Architecture, eigener Dispatcher, Bounded Contexts als eigene Services.
