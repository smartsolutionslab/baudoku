import { z } from "zod";

export const installationSchema = z.object({
  type: z.string().min(1, "Typ erforderlich"),
  status: z.enum(["planned", "in_progress", "completed", "inspected"]),
  manufacturer: z.string().optional(),
  model: z.string().optional(),
  serialNumber: z.string().optional(),
  cableType: z.string().optional(),
  crossSectionMm2: z.coerce.number().positive().optional(),
  lengthM: z.coerce.number().positive().optional(),
  circuitId: z.string().optional(),
  fuseType: z.string().optional(),
  fuseRatingA: z.coerce.number().positive().optional(),
  voltageV: z.coerce.number().int().positive().optional(),
  phase: z.enum(["L1", "L2", "L3", "N", "PE"]).optional(),
  depthMm: z.coerce.number().int().positive().optional(),
  notes: z.string().optional(),
});

export type InstallationFormData = z.infer<typeof installationSchema>;

export const measurementSchema = z.object({
  type: z.string().min(1, "Messtyp erforderlich"),
  value: z.coerce.number({ message: "Messwert erforderlich" }),
  unit: z.string().min(1, "Einheit erforderlich"),
  minThreshold: z.coerce.number().optional(),
  maxThreshold: z.coerce.number().optional(),
  notes: z.string().optional(),
  measuredBy: z.string().min(1, "Prüfer erforderlich"),
});

export type MeasurementFormData = z.infer<typeof measurementSchema>;
