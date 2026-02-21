import { z } from "zod";

export const projectSchema = z.object({
  name: z.string().min(1, "Name erforderlich").max(200),
  status: z.enum(["active", "completed", "archived"]).default("active"),
  street: z.string().max(200).optional(),
  city: z.string().max(100).optional(),
  zipCode: z.string().max(10).optional(),
  clientName: z.string().max(200).optional(),
  clientContact: z.string().max(200).optional(),
});

export type ProjectFormData = z.infer<typeof projectSchema>;

export const zoneSchema = z.object({
  name: z.string().min(1, "Name erforderlich").max(100),
  type: z.enum(["building", "floor", "room", "trench"]),
  parentZoneId: z.string().nullable().optional().transform(v => v || null),
  sortOrder: z.coerce.number().int().min(0).optional(),
});

export type ZoneFormData = z.infer<typeof zoneSchema>;
