/**
 * Branded type system for BauDoku frontend.
 *
 * Prevents accidental mixing of primitive types (e.g. passing a ZoneId
 * where a ProjectId is expected).  Branded types are structurally
 * compatible with their base type in covariant positions -- a ProjectId
 * can be used wherever a string is expected -- but the reverse requires
 * an explicit constructor call that validates the value.
 *
 * @module branded
 */

declare const brand: unique symbol;
type Brand<T, B extends string> = T & { readonly [brand]: B };

// ============ ID Types ============

export type ProjectId = Brand<string, "ProjectId">;
export type ZoneId = Brand<string, "ZoneId">;
export type InstallationId = Brand<string, "InstallationId">;
export type PhotoId = Brand<string, "PhotoId">;
export type MeasurementId = Brand<string, "MeasurementId">;
export type SyncOutboxEntryId = Brand<string, "SyncOutboxEntryId">;

// ============ Value Types ============

export type ProjectName = Brand<string, "ProjectName">;
export type ZoneName = Brand<string, "ZoneName">;
export type Latitude = Brand<number, "Latitude">;
export type Longitude = Brand<number, "Longitude">;
export type DepthMm = Brand<number, "DepthMm">;

// ============ Constructor Functions ============

export function projectId(value: string): ProjectId {
  if (!value) throw new Error("ProjectId must not be empty");
  return value as ProjectId;
}

export function zoneId(value: string): ZoneId {
  if (!value) throw new Error("ZoneId must not be empty");
  return value as ZoneId;
}

export function installationId(value: string): InstallationId {
  if (!value) throw new Error("InstallationId must not be empty");
  return value as InstallationId;
}

export function photoId(value: string): PhotoId {
  if (!value) throw new Error("PhotoId must not be empty");
  return value as PhotoId;
}

export function measurementId(value: string): MeasurementId {
  if (!value) throw new Error("MeasurementId must not be empty");
  return value as MeasurementId;
}

export function syncOutboxEntryId(value: string): SyncOutboxEntryId {
  if (!value) throw new Error("SyncOutboxEntryId must not be empty");
  return value as SyncOutboxEntryId;
}

export function projectName(value: string): ProjectName {
  if (!value?.trim()) throw new Error("ProjectName must not be empty");
  if (value.length > 200)
    throw new Error("ProjectName must be at most 200 characters");
  return value as ProjectName;
}

export function zoneName(value: string): ZoneName {
  if (!value?.trim()) throw new Error("ZoneName must not be empty");
  return value as ZoneName;
}

export function latitude(value: number): Latitude {
  if (value < -90 || value > 90)
    throw new Error("Latitude must be between -90 and 90");
  return value as Latitude;
}

export function longitude(value: number): Longitude {
  if (value < -180 || value > 180)
    throw new Error("Longitude must be between -180 and 180");
  return value as Longitude;
}

export function depthMm(value: number): DepthMm {
  if (value < 0) throw new Error("DepthMm must not be negative");
  if (!Number.isInteger(value))
    throw new Error("DepthMm must be an integer");
  return value as DepthMm;
}

// ============ Unsafe Cast (for DB results / deserialization) ============

/**
 * Bypass validation and cast a raw value to a branded type.
 * Use only at the boundary where data comes from trusted sources
 * (e.g. SQLite / Drizzle ORM results, server API responses).
 */
export function unsafeBrand<T>(value: unknown): T {
  return value as T;
}
