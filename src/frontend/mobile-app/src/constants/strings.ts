// ─── Default User ────────────────────────────────────────────────
export const LOCAL_USER = 'local-user';

// ─── Alert / Error Titles ────────────────────────────────────────
export const ERROR_TITLE = 'Fehler';

// ─── HTTP / API Error Messages ───────────────────────────────────
export const API_ERRORS = {
  conflict: 'Konflikt: Daten wurden zwischenzeitlich geändert',
  validation: 'Validierungsfehler',
  server: 'Serverfehler — bitte später erneut versuchen',
  unknown: 'Ein unbekannter Fehler ist aufgetreten',
} as const;

// ─── Entity Mutation Error Messages ──────────────────────────────
export const MUTATION_ERRORS = {
  projectCreate: 'Projekt konnte nicht erstellt werden',
  projectUpdate: 'Projekt konnte nicht aktualisiert werden',
  projectDelete: 'Projekt konnte nicht gelöscht werden',
  projectSave: 'Projekt konnte nicht gespeichert werden.',
  zoneCreate: 'Zone konnte nicht erstellt werden',
  zoneUpdate: 'Zone konnte nicht aktualisiert werden',
  zoneDelete: 'Zone konnte nicht gelöscht werden',
  zoneSave: 'Zone konnte nicht gespeichert werden.',
  installationCreate: 'Installation konnte nicht erstellt werden',
  installationUpdate: 'Installation konnte nicht aktualisiert werden',
  installationDelete: 'Installation konnte nicht gelöscht werden',
  installationSave: 'Installation konnte nicht gespeichert werden.',
  photoAdd: 'Foto konnte nicht hinzugefügt werden',
  photoDelete: 'Foto konnte nicht gelöscht werden',
  photoAnnotation: 'Foto-Anmerkung konnte nicht aktualisiert werden',
  measurementAdd: 'Messung konnte nicht hinzugefügt werden',
  measurementDelete: 'Messung konnte nicht gelöscht werden',
  measurementSave: 'Messung konnte nicht gespeichert werden.',
  conflictResolve: 'Konflikt konnte nicht aufgelöst werden',
} as const;

// ─── GPS Messages ────────────────────────────────────────────────
export const GPS_MESSAGES = {
  permissionDenied: 'Standort-Berechtigung wurde verweigert.',
  mockDisabled: 'Externes GPS ist in den Einstellungen deaktiviert.',
  captureFailed: 'GPS-Position konnte nicht erfasst werden.',
} as const;

// ─── Sync Messages ───────────────────────────────────────────────
export const SYNC_MESSAGES = {
  unknownError: 'Unbekannter Fehler',
  photoUploadFailed: 'Foto-Upload fehlgeschlagen',
} as const;
