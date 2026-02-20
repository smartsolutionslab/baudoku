// ─── Measurement Type Presets ────────────────────────────────────

export interface MeasurementTypePreset {
  type: string;
  unit: string;
  minThreshold?: number;
  maxThreshold?: number;
}

export const MEASUREMENT_TYPES: MeasurementTypePreset[] = [
  {
    type: "Isolationswiderstand",
    unit: "MΩ",
    minThreshold: 1.0,
  },
  {
    type: "Schleifenimpedanz",
    unit: "Ω",
    maxThreshold: 1.85,
  },
  {
    type: "RCD-Auslösezeit",
    unit: "ms",
    maxThreshold: 300,
  },
  {
    type: "Erdungswiderstand",
    unit: "Ω",
    maxThreshold: 100,
  },
  {
    type: "Durchgangswiderstand",
    unit: "Ω",
    maxThreshold: 1.0,
  },
  {
    type: "Spannungsfall",
    unit: "%",
    maxThreshold: 5,
  },
  {
    type: "Kurzschlussstrom",
    unit: "A",
  },
  {
    type: "Berührungsspannung",
    unit: "V",
    maxThreshold: 50,
  },
];

// ─── Installation Types ─────────────────────────────────────────

export const INSTALLATION_TYPES = [
  "Kabelverlegung",
  "Muffe",
  "Verteiler",
  "Schaltschrank",
  "Steckdose",
  "Schalter",
  "Leuchte",
  "Erdung",
  "Blitzschutz",
  "Sonstiges",
] as const;

// ─── Installation Status ────────────────────────────────────────

export const INSTALLATION_STATUSES = [
  "planned",
  "in_progress",
  "completed",
  "inspected",
] as const;

export const INSTALLATION_STATUS_LABELS: Record<
  (typeof INSTALLATION_STATUSES)[number],
  string
> = {
  planned: "Geplant",
  in_progress: "In Arbeit",
  completed: "Abgeschlossen",
  inspected: "Geprüft",
};

// ─── Photo Types ────────────────────────────────────────────────

export const PHOTO_TYPES = ["before", "after", "detail", "overview"] as const;

export const PHOTO_TYPE_LABELS: Record<
  (typeof PHOTO_TYPES)[number],
  string
> = {
  before: "Vorher",
  after: "Nachher",
  detail: "Detail",
  overview: "Übersicht",
};

// ─── GPS Source Types ───────────────────────────────────────────

export const GPS_SOURCES = [
  "internal_gps",
  "external_dgnss",
  "external_rtk",
] as const;

export const GPS_SOURCE_LABELS: Record<
  (typeof GPS_SOURCES)[number],
  string
> = {
  internal_gps: "Internes GPS",
  external_dgnss: "Ext. DGNSS",
  external_rtk: "Ext. RTK",
};

// ─── Measurement Units ──────────────────────────────────────────

export const MEASUREMENT_UNITS = ["Ω", "MΩ", "V", "ms", "%", "A"] as const;
