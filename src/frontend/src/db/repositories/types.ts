import type { InferSelectModel, InferInsertModel } from "drizzle-orm";
import type {
  projects,
  zones,
  installations,
  photos,
  measurements,
  syncOutbox,
} from "../schema";

export type Project = InferSelectModel<typeof projects>;
export type NewProject = InferInsertModel<typeof projects>;

export type Zone = InferSelectModel<typeof zones>;
export type NewZone = InferInsertModel<typeof zones>;

export type Installation = InferSelectModel<typeof installations>;
export type NewInstallation = InferInsertModel<typeof installations>;

export type Photo = InferSelectModel<typeof photos>;
export type NewPhoto = InferInsertModel<typeof photos>;

export type Measurement = InferSelectModel<typeof measurements>;
export type NewMeasurement = InferInsertModel<typeof measurements>;

export type SyncOutboxEntry = InferSelectModel<typeof syncOutbox>;
export type NewSyncOutboxEntry = InferInsertModel<typeof syncOutbox>;
