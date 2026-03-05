import type {
  ProjectId,
  ZoneId,
  InstallationId,
  PhotoId,
  MeasurementId,
  Latitude,
  Longitude,
  DepthMm,
} from '@baudoku/core';

export type GpsPosition = {
  latitude: Latitude;
  longitude: Longitude;
  altitude: number | null;
  horizontalAccuracy: number;
  gpsSource: GpsSource;
  correctionService: GpsCorrectionService | null;
  rtkFixStatus: RtkFixStatus | null;
  satelliteCount: number | null;
  hdop: number | null;
  correctionAge: number | null;
};

export type Installation = {
  id: InstallationId;
  projectId: ProjectId;
  zoneId: ZoneId;
  type: string;
  status: InstallationStatus;

  // GPS / GNSS
  position: GpsPosition | null;
  depthMm: DepthMm | null;
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
};

export type InstallationStatus = 'planned' | 'in_progress' | 'completed' | 'inspected';

export type GpsSource = 'internal_gps' | 'external_dgnss' | 'external_rtk';

export type GpsCorrectionService = 'none' | 'sapos_eps' | 'sapos_heps' | 'sapos_gpps';

export type RtkFixStatus = 'no_fix' | 'autonomous' | 'dgps' | 'rtk_float' | 'rtk_fixed';

export type Phase = 'L1' | 'L2' | 'L3' | 'N' | 'PE';

export type Photo = {
  id: PhotoId;
  installationId: InstallationId;
  localPath: string;
  remotePath: string | null;
  thumbnailPath: string | null;
  type: PhotoType;
  annotations: string | null;
  position: GpsPosition | null;
  caption: string | null;
  exifLatitude: Latitude | null;
  exifLongitude: Longitude | null;
  exifDateTime: string | null;
  exifCameraModel: string | null;
  takenAt: string;
  uploadStatus: UploadStatus;
  version: number;
};

export type PhotoType = 'before' | 'after' | 'detail' | 'overview';

export type UploadStatus = 'pending' | 'uploading' | 'uploaded' | 'failed';

export type Measurement = {
  id: MeasurementId;
  installationId: InstallationId;
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
};

export type MeasurementResult = 'passed' | 'failed' | 'warning';
