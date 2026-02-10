# ADR-005: Offline-First mit SQLite und Delta-Sync

## Status

Accepted

## Kontext

Elektriker und Tiefbauer arbeiten häufig auf Baustellen ohne stabile Internetverbindung. Die App muss vollständig offline funktionieren und Daten bei Wiederherstellung der Verbindung synchronisieren.

## Entscheidung

Wir implementieren eine **Offline-First-Architektur**:

- **Mobile:** SQLite (via Drizzle ORM) als lokale Datenbank
- **Sync:** Delta-Sync-Protokoll mit Outbox Pattern
- **Konflikte:** Last-Writer-Wins als Standard, manuelle Auflösung bei kritischen Konflikten
- **Backend:** PostgreSQL als Source of Truth

### Sync-Flow
1. Mobile erfasst Daten lokal in SQLite
2. Änderungen werden in einer Outbox-Tabelle gesammelt
3. Bei Verbindung: Delta-Sync sendet nur geänderte Records
4. Server verarbeitet Batch, erkennt Konflikte, sendet Ergebnis zurück

## Konsequenzen

### Positiv
- App ist voll funktionsfähig ohne Internet
- Datenerfassung wird nie durch Verbindungsprobleme blockiert
- Effiziente Synchronisation durch Delta-Sync

### Negativ
- Komplexe Synchronisationslogik
- Konfliktauflösung kann schwierig sein
- Zwei Datenmodelle (SQLite + PostgreSQL) müssen synchron gehalten werden
