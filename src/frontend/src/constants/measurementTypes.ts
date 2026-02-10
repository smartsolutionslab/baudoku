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
