# ADR-002: Eigener Dispatcher statt MediatR

## Status

Accepted

## Kontext

Für die Kommunikation zwischen API-Endpunkten und Application-Layer-Handlern wird ein Dispatcher/Mediator-Pattern benötigt. MediatR und Wolverine sind populäre Libraries dafür.

## Entscheidung

Wir implementieren einen **eigenen Dispatcher** basierend auf `IServiceProvider`:

- Commands, Queries und Domain Events werden über `IDispatcher` versendet
- Handler werden per **Scrutor Assembly-Scanning** automatisch registriert
- Behaviors (Validation, Logging) werden als Decorators implementiert

## Alternativen

- **MediatR:** Weit verbreitet, aber kostenpflichtig seit v12, und wir brauchen nur einen Bruchteil der Features
- **Wolverine:** Mächtiger, aber zu viel Overhead für unseren Use Case

## Konsequenzen

### Positiv
- Volle Kontrolle über das Verhalten des Dispatchers
- Keine externe Lizenzabhängigkeit
- Einfach zu verstehen und zu debuggen
- Leichtgewichtig (~100 Zeilen Code)

### Negativ
- Muss selbst gewartet werden
- Keine Community-Support wie bei MediatR
