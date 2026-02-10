# ADR-003: Bounded Contexts als eigenständige Services

## Status

Accepted

## Kontext

BauDoku hat drei fachliche Domänen: Projektverwaltung, Installationsdokumentation und Offline-Synchronisation. Diese müssen strukturiert und voneinander abgegrenzt werden.

## Entscheidung

Jeder **Bounded Context** wird als eigener Service mit eigenem API-Projekt implementiert:

- **Projects:** Projektverwaltung, Zonen-Hierarchie
- **Documentation:** Installationserfassung, Fotos, Messungen, GPS-Positionen
- **Sync:** Offline-Synchronisation, Outbox Pattern, Konfliktauflösung

Jeder Service hat 4 Layer-Projekte (Domain, Application, Infrastructure, Api) und eine eigene Datenbank.

## Konsequenzen

### Positiv
- Klare fachliche Grenzen
- Services können unabhängig deployt und skaliert werden
- Eigene Datenbank pro Service verhindert Kopplung auf Datenebene
- Einfacher Übergang zu Microservices wenn nötig

### Negativ
- Kommunikation zwischen BCs benötigt Messaging (RabbitMQ)
- Höhere Komplexität durch verteiltes System
- Lokale Entwicklung braucht Orchestrierung (.NET Aspire)
