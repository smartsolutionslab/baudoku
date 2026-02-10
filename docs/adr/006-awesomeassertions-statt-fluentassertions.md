# ADR-006: AwesomeAssertions statt FluentAssertions

## Status

Accepted

## Kontext

FluentAssertions hat mit Version 8.0 (Januar 2025) die Lizenz von **Apache 2.0** auf die **Xceed Community License** geändert. Kommerzielle Nutzung erfordert seitdem eine kostenpflichtige Lizenz (~130$/Entwickler/Jahr). BauDoku ist ein kommerzielles Projekt – die Nutzung von FluentAssertions 8.x stellt ein Lizenzrisiko dar.

## Alternativen

| Option | Lizenz | API-Kompatibilität | Bewertung |
|--------|--------|-------------------|-----------|
| **AwesomeAssertions** | Apache 2.0 | Drop-in (identische API) | Gewählt |
| Shouldly | BSD 3-Clause | Andere Syntax | Zu viel Migrationsaufwand |
| FluentAssertions 7.x | Apache 2.0 | Identisch | End-of-Life, keine Updates |
| xUnit Built-in | MIT | Kein Fluent-Style | Weniger lesbar |

## Entscheidung

Wir ersetzen **FluentAssertions** durch **AwesomeAssertions** (Apache 2.0):

- Community-Fork von FluentAssertions 8.0
- Identische API – nur Namespace-Änderung nötig (`using AwesomeAssertions;`)
- Lizenz wird dauerhaft Apache 2.0 bleiben (Community-Commitment)
- Aktiv gepflegt, Version 9.3.0 verfügbar

## Konsequenzen

### Positiv
- Kein Lizenzrisiko für kommerzielle Nutzung
- Keinerlei Syntax-Änderung in Tests nötig (Drop-in-Replacement)
- Aktive Community-Wartung

### Negativ
- Kleinere Community als FluentAssertions
- Neue Features erscheinen evtl. verzögert
