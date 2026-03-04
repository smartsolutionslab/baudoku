// ─── Zone Types ─────────────────────────────────────────────────

export const ZONE_TYPES = ['building', 'floor', 'room', 'trench'] as const;

export const ZONE_TYPE_LABELS: Record<(typeof ZONE_TYPES)[number], string> = {
  building: 'Gebäude',
  floor: 'Stockwerk',
  room: 'Raum',
  trench: 'Graben',
};

/** Format a zone's name with its type label, e.g. "Erdgeschoss (Stockwerk)" */
export function formatZoneLabel(name: string, type: string): string {
  return `${name} (${ZONE_TYPE_LABELS[type as keyof typeof ZONE_TYPE_LABELS] ?? type})`;
}

// ─── Project Status ─────────────────────────────────────────────

export const PROJECT_STATUSES = ['active', 'completed', 'archived'] as const;

export const PROJECT_STATUS_LABELS: Record<(typeof PROJECT_STATUSES)[number], string> = {
  active: 'Aktiv',
  completed: 'Abgeschlossen',
  archived: 'Archiviert',
};
