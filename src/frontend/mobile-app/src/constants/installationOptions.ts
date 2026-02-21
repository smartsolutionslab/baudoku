import { INSTALLATION_STATUS_LABELS } from "@baudoku/documentation";

export const INSTALLATION_STATUS_OPTIONS = Object.entries(
  INSTALLATION_STATUS_LABELS
).map(([value, label]) => ({ value, label }));

export const INSTALLATION_TYPE_OPTIONS = [
  { value: "cable_tray", label: "Kabeltrasse" },
  { value: "junction_box", label: "Abzweigdose" },
  { value: "cable_pull", label: "Kabelzug" },
  { value: "conduit", label: "Leerrohr" },
  { value: "grounding", label: "Erdung" },
  { value: "lightning_protection", label: "Blitzschutz" },
  { value: "switchgear", label: "Schaltanlage" },
  { value: "transformer", label: "Trafo" },
  { value: "other", label: "Sonstige" },
];
