export type GpsPosition = {
  latitude: number;
  longitude: number;
  altitude: number | null;
  horizontalAccuracy: number;
  gpsSource: string;
  correctionService: string | null;
  rtkFixStatus: string | null;
  satelliteCount: number | null;
  hdop: number | null;
  correctionAge: number | null;
};

export interface Installation {
  id: string;
  projectId: string;
  zoneId: string;
  type: string;
  status: InstallationStatus;

  // GPS / GNSS
  position: GpsPosition | null;
  depthMm: number | null;
  positionOnPlan: string | null;

  // Component
  manufacturer: string | null;
  model: string | null;
  serialNumber: string | null;
  cableType: string | null;
  crossSectionMm2: number | null;
  lengthM: number | null;

  // Electrical
  circuitId: string | null;
  fuseType: string | null;
  fuseRatingA: number | null;
  voltageV: number | null;
  phase: Phase | null;

  // Meta
  notes: string | null;
  installedBy: string | null;
  installedAt: string | null;
  inspectedBy: string | null;
  inspectedAt: string | null;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export type InstallationStatus =
  | "planned"
  | "in_progress"
  | "completed"
  | "inspected";

export type GpsSource = "internal_gps" | "external_dgnss" | "external_rtk";

export type GpsCorrectionService =
  | "none"
  | "sapos_eps"
  | "sapos_heps"
  | "sapos_gpps";

export type RtkFixStatus =
  | "no_fix"
  | "autonomous"
  | "dgps"
  | "rtk_float"
  | "rtk_fixed";

export type Phase = "L1" | "L2" | "L3" | "N" | "PE";

export interface Photo {
  id: string;
  installationId: string;
  localPath: string;
  remotePath: string | null;
  thumbnailPath: string | null;
  type: PhotoType;
  annotations: string | null;
  position: GpsPosition | null;
  caption: string | null;
  exifLatitude: number | null;
  exifLongitude: number | null;
  exifDateTime: string | null;
  exifCameraModel: string | null;
  takenAt: string;
  uploadStatus: UploadStatus;
  version: number;
}

export type PhotoType = "before" | "after" | "detail" | "overview";

export type UploadStatus = "pending" | "uploading" | "uploaded" | "failed";

export interface Measurement {
  id: string;
  installationId: string;
  type: string;
  value: number;
  unit: string;
  minThreshold: number | null;
  maxThreshold: number | null;
  result: MeasurementResult | null;
  notes: string | null;
  measuredAt: string;
  measuredBy: string;
  version: number;
}

export type MeasurementResult = "passed" | "failed" | "warning";
