import { sqliteTable, text, integer, real } from "drizzle-orm/sqlite-core";

// ─── BD-501: Projects + Zones ───────────────────────────────────

export const projects = sqliteTable("projects", {
  id: text("id").primaryKey(),
  name: text("name").notNull(),
  street: text("street"),
  city: text("city"),
  zipCode: text("zip_code"),
  gpsLat: real("gps_lat"),
  gpsLng: real("gps_lng"),
  clientName: text("client_name"),
  clientContact: text("client_contact"),
  status: text("status", {
    enum: ["active", "completed", "archived"],
  }).notNull(),
  createdBy: text("created_by").notNull(),
  createdAt: integer("created_at", { mode: "timestamp" }).notNull(),
  updatedAt: integer("updated_at", { mode: "timestamp" }).notNull(),
  version: integer("version").notNull().default(1),
});

export const zones = sqliteTable("zones", {
  id: text("id").primaryKey(),
  projectId: text("project_id")
    .notNull()
    .references(() => projects.id),
  parentZoneId: text("parent_zone_id"),
  name: text("name").notNull(),
  type: text("type", {
    enum: ["building", "floor", "room", "section", "trench"],
  }).notNull(),
  qrCode: text("qr_code"),
  sortOrder: integer("sort_order").default(0),
  version: integer("version").notNull().default(1),
});

// ─── BD-502: Installations, Photos, Measurements ────────────────

export const installations = sqliteTable("installations", {
  id: text("id").primaryKey(),
  projectId: text("project_id")
    .notNull()
    .references(() => projects.id),
  zoneId: text("zone_id")
    .notNull()
    .references(() => zones.id),
  type: text("type").notNull(),
  status: text("status", {
    enum: ["planned", "in_progress", "completed", "inspected"],
  }).notNull(),

  // GPS / GNSS
  gpsLat: real("gps_lat"),
  gpsLng: real("gps_lng"),
  gpsAltitude: real("gps_altitude"),
  gpsAltitudeMsl: real("gps_altitude_msl"),
  gpsAccuracy: real("gps_accuracy"),
  gpsSource: text("gps_source", {
    enum: ["internal_gps", "external_dgnss", "external_rtk"],
  }),
  gpsCorrService: text("gps_corr_service", {
    enum: ["none", "sapos_eps", "sapos_heps", "sapos_gpps"],
  }),
  gpsRtkStatus: text("gps_rtk_status", {
    enum: ["no_fix", "autonomous", "dgps", "rtk_float", "rtk_fixed"],
  }),
  gpsSatCount: integer("gps_sat_count"),
  gpsHdop: real("gps_hdop"),
  gpsCorrAge: real("gps_corr_age"),
  depthMm: integer("depth_mm"),
  positionOnPlan: text("position_on_plan"), // JSON {x, y}

  // Component
  manufacturer: text("manufacturer"),
  model: text("model"),
  serialNumber: text("serial_number"),
  cableType: text("cable_type"),
  crossSectionMm2: real("cross_section_mm2"),
  lengthM: real("length_m"),

  // Electrical
  circuitId: text("circuit_id"),
  fuseType: text("fuse_type"),
  fuseRatingA: real("fuse_rating_a"),
  voltageV: integer("voltage_v"),
  phase: text("phase", { enum: ["L1", "L2", "L3", "N", "PE"] }),

  // Meta
  notes: text("notes"),
  installedBy: text("installed_by"),
  installedAt: integer("installed_at", { mode: "timestamp" }),
  inspectedBy: text("inspected_by"),
  inspectedAt: integer("inspected_at", { mode: "timestamp" }),
  createdAt: integer("created_at", { mode: "timestamp" }).notNull(),
  updatedAt: integer("updated_at", { mode: "timestamp" }).notNull(),
  version: integer("version").notNull().default(1),
});

export const photos = sqliteTable("photos", {
  id: text("id").primaryKey(),
  installationId: text("installation_id")
    .notNull()
    .references(() => installations.id),
  localPath: text("local_path").notNull(),
  remotePath: text("remote_path"),
  thumbnailPath: text("thumbnail_path"),
  type: text("type", {
    enum: ["before", "after", "detail", "overview"],
  }).notNull(),
  annotations: text("annotations"), // JSON
  gpsLat: real("gps_lat"),
  gpsLng: real("gps_lng"),
  gpsAccuracy: real("gps_accuracy"),
  gpsSource: text("gps_source", {
    enum: ["internal_gps", "external_dgnss", "external_rtk"],
  }),
  gpsCorrService: text("gps_corr_service", {
    enum: ["none", "sapos_eps", "sapos_heps"],
  }),
  caption: text("caption"),
  exifLatitude: real("exif_latitude"),
  exifLongitude: real("exif_longitude"),
  exifDateTime: text("exif_date_time"),
  exifCameraModel: text("exif_camera_model"),
  takenAt: integer("taken_at", { mode: "timestamp" }).notNull(),
  uploadStatus: text("upload_status", {
    enum: ["pending", "uploading", "uploaded", "failed"],
  }).notNull(),
  version: integer("version").notNull().default(1),
});

export const measurements = sqliteTable("measurements", {
  id: text("id").primaryKey(),
  installationId: text("installation_id")
    .notNull()
    .references(() => installations.id),
  type: text("type").notNull(),
  value: real("value").notNull(),
  unit: text("unit").notNull(),
  minThreshold: real("min_threshold"),
  maxThreshold: real("max_threshold"),
  result: text("result", {
    enum: ["passed", "failed", "warning"],
  }),
  notes: text("notes"),
  measuredAt: integer("measured_at", { mode: "timestamp" }).notNull(),
  measuredBy: text("measured_by").notNull(),
  version: integer("version").notNull().default(1),
});

// ─── BD-503: Sync Outbox + Meta ─────────────────────────────────

export const syncOutbox = sqliteTable("sync_outbox", {
  id: text("id").primaryKey(),
  entityType: text("entity_type").notNull(),
  entityId: text("entity_id").notNull(),
  operation: text("operation", {
    enum: ["create", "update", "delete"],
  }).notNull(),
  payload: text("payload").notNull(), // JSON
  timestamp: integer("timestamp", { mode: "timestamp" }).notNull(),
  deviceId: text("device_id").notNull(),
  retryCount: integer("retry_count").default(0),
  status: text("status", {
    enum: ["pending", "syncing", "synced", "failed"],
  }).notNull(),
});

export const syncMeta = sqliteTable("sync_meta", {
  key: text("key").primaryKey(),
  value: text("value").notNull(),
});
