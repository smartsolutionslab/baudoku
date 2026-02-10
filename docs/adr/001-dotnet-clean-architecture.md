# ADR-001: .NET 10 mit Clean Architecture und DDD

## Status

Accepted

## Kontext

BauDoku benötigt ein Backend für die Verwaltung von Installationsdokumentationen, Projekten und Offline-Synchronisation. Die Architektur muss langfristig wartbar, testbar und erweiterbar sein.

## Entscheidung

Wir verwenden **.NET 10 Minimal APIs** mit **Domain-Driven Design (DDD)** und **Clean Architecture**:

- **4 Schichten pro Bounded Context:** Domain → Application → Infrastructure → Api
- **Dependency Rule:** Abhängigkeiten zeigen nur nach innen (Domain hat keine externen Abhängigkeiten)
- **Aggregate Roots** als Transaktionsgrenzen
- **ValueObjects** statt primitiver Typen im Domain Model
- **Minimal APIs** statt Controller

## Konsequenzen

### Positiv
- Klare Trennung der Verantwortlichkeiten
- Domain-Logik ist unabhängig von Framework und Infrastruktur testbar
- Einfache Erweiterbarkeit durch neue Bounded Contexts

### Negativ
- Höherer initialer Aufwand durch Boilerplate (ValueObjects, Handler, etc.)
- Lernkurve für Entwickler, die nicht mit DDD vertraut sind
