import type { InferSelectModel, InferInsertModel } from "drizzle-orm";
import type { projects, zones, installations, photos, measurements, syncOutbox } from "../schema";
import type { ProjectId, ZoneId, InstallationId, PhotoId, MeasurementId, SyncOutboxEntryId, ProjectName, ZoneName, Latitude, Longitude, DepthMm } from "../../types/branded";

// ─── Raw DB types (Drizzle-inferred) ─────────────────────────────
// These are used internally by the ORM layer.

type RawProject = InferSelectModel<typeof projects>;
type RawZone = InferSelectModel<typeof zones>;
type RawInstallation = InferSelectModel<typeof installations>;
type RawPhoto = InferSelectModel<typeof photos>;
type RawMeasurement = InferSelectModel<typeof measurements>;
type RawSyncOutboxEntry = InferSelectModel<typeof syncOutbox>;

// ─── Branded domain types ────────────────────────────────────────
// Overlay branded ID and value types on top of Drizzle-inferred types.

export type Project = Omit<RawProject, "id" | "name" | "gpsLat" | "gpsLng"> & {
  id: ProjectId;
  name: ProjectName;
  gpsLat: Latitude | null;
  gpsLng: Longitude | null;
};

export type Zone = Omit<RawZone, "id" | "projectId" | "parentZoneId" | "name"> & {
  id: ZoneId;
  projectId: ProjectId;
  parentZoneId: ZoneId | null;
  name: ZoneName;
};

export type Installation = Omit<
  RawInstallation,
  "id" | "projectId" | "zoneId" | "gpsLat" | "gpsLng" | "depthMm"
> & {
  id: InstallationId;
  projectId: ProjectId;
  zoneId: ZoneId;
  gpsLat: Latitude | null;
  gpsLng: Longitude | null;
  depthMm: DepthMm | null;
};

export type Photo = Omit<
  RawPhoto,
  "id" | "installationId" | "gpsLat" | "gpsLng"
> & {
  id: PhotoId;
  installationId: InstallationId;
  gpsLat: Latitude | null;
  gpsLng: Longitude | null;
};

export type Measurement = Omit<RawMeasurement, "id" | "installationId"> & {
  id: MeasurementId;
  installationId: InstallationId;
};

export type SyncOutboxEntry = Omit<RawSyncOutboxEntry, "id"> & {
  id: SyncOutboxEntryId;
};

// ─── Insert types (keep raw — IDs are generated internally) ──────

export type NewProject = InferInsertModel<typeof projects>;
export type NewZone = InferInsertModel<typeof zones>;
export type NewInstallation = InferInsertModel<typeof installations>;
export type NewPhoto = InferInsertModel<typeof photos>;
export type NewMeasurement = InferInsertModel<typeof measurements>;
export type NewSyncOutboxEntry = InferInsertModel<typeof syncOutbox>;
