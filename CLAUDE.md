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
// Basis: abstract record
public abstract record ValueObject;

// Konkretes ValueObject: sealed record mit Konstruktor-Properties + Validierung
public sealed record ProjectName : ValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    public ProjectName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Projektname darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Projektname darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}

// ID-ValueObject
public sealed record ProjectId : ValueObject
{
    public Guid Value { get; }
    public ProjectId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ID darf nicht leer sein.", nameof(value));
        Value = value;
    }
    public static ProjectId New() => new(Guid.NewGuid());
}

// Enum-artige ValueObjects (statt C# enums)
public sealed record InstallationStatus : ValueObject
{
    public static readonly InstallationStatus InProgress = new("in_progress");
    public static readonly InstallationStatus Completed = new("completed");
    public static readonly InstallationStatus Inspected = new("inspected");

    public string Value { get; }
    public InstallationStatus(string value) => Value = value ?? throw new ArgumentNullException(nameof(value));
}
```

### Entity + AggregateRoot

```csharp
public abstract class Entity<TId> where TId : ValueObject
{
    public TId Id { get; protected set; } = default!;
}

public abstract class AggregateRoot<TId> : Entity<TId> where TId : ValueObject
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken()) throw new BusinessRuleException(rule);
    }
}
```

### Aggregate Root Factory Pattern

Aggregate Roots werden NICHT über Konstruktoren erstellt, sondern über statische Factory Methods:

```csharp
public sealed class Project : AggregateRoot<ProjectId>
{
    private Project() { } // EF Core

    public static Project Create(ProjectId id, ProjectName name, Address address, ClientInfo client)
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
│   ├── BauDoku.BuildingBlocks.Domain/          # ValueObject, Entity, AggregateRoot, IDomainEvent, IBusinessRule
│   ├── BauDoku.BuildingBlocks.Application/     # Dispatcher, ICommand/IQuery, Behaviors (Validation, Logging, Transaction)
│   └── BauDoku.BuildingBlocks.Infrastructure/  # BaseDbContext, UnitOfWork, Messaging
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
└── BauDoku.sln
```

## Bounded Contexts

1. **Projects** – Projektverwaltung, Zonen-Hierarchie (Gebäude → Stockwerk → Raum → Graben)
2. **Documentation** – Installations-Erfassung, Fotos, Messungen, GPS-Positionen, Kabelspezifikationen
3. **Sync** – Offline-Synchronisation (Outbox Pattern, Delta-Sync, Konfliktauflösung)

## Coding-Standards

- **C#:** `sealed record` für ValueObjects, `sealed class` für Handlers, `private set` für Entity-Properties
- **Namespaces:** `BauDoku.{BC}.{Layer}` z.B. `BauDoku.Projects.Domain.Aggregates.Project`
- **Private fields:** `_camelCase` (z.B. `_domainEvents`, `_photos`)
- **Commits:** Conventional Commits (`feat(projects): add Project aggregate`)
- **Ticket-Prefix:** `BD-xxx`
- **Tests:** xUnit + AwesomeAssertions + NSubstitute + Testcontainers (PostgreSQL)
- **Keine Primitive im Domain:** `ProjectId` statt `Guid`, `ProjectName` statt `string`, `Depth` statt `int`

## NuGet Packages (Central Package Management)

- EntityFrameworkCore 10.*, Npgsql.EFCore.PostgreSQL 10.*, NetTopologySuite
- FluentValidation 11.*, Scrutor 5.*
- Polly 8.*, Serilog.AspNetCore 9.*
- RabbitMQ.Client 7.*, Azure.Storage.Blobs 12.*
- Yarp.ReverseProxy 2.*, Aspire.Hosting 9.*
- xUnit 2.*, AwesomeAssertions 9.*, Testcontainers.PostgreSql 4.*, NSubstitute 5.*

## GPS/GNSS-Kontext

Die App unterstützt ein dreistufiges Positionierungskonzept:
- **Stufe 1:** Internes Smartphone-GPS (~3–5 m) – immer verfügbar
- **Stufe 2:** Ext. DGNSS + SAPOS-EPS (~30–50 cm) – MVP-Ziel, ~150€ Hardware
- **Stufe 3:** Ext. RTK + SAPOS-HEPS (~1–3 cm) – professionelle Vermessung

`GpsPosition` ist ein komplexes ValueObject mit: Latitude, Longitude, Altitude, HorizontalAccuracy, Source, CorrectionService, RtkFixStatus, SatelliteCount, Hdop, CorrectionAge.

## Aktuelle Phase

**Phase 0: Foundation** – Repo-Grundgerüst, BuildingBlocks, .NET Solution mit allen Projekten, Aspire AppHost.

## Referenz-Architektur

Das Projekt folgt dem Stil der **Orange Car Rental Solution** (github.com/smartsolutionslab/orange-car-rental-solution) – ebenfalls .NET 10, DDD, Clean Architecture, eigener Dispatcher, Bounded Contexts als eigene Services.
