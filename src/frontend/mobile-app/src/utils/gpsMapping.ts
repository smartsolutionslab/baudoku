import type { GpsPosition, GpsSource, GpsCorrService, GpsRtkStatus } from '../hooks/useGpsCapture';
import type { Installation } from '../db/repositories/types';

export function toGpsPosition(installation: Installation): GpsPosition | null {
  if (installation.gpsLat == null || installation.gpsLng == null) return null;
  return {
    latitude: installation.gpsLat,
    longitude: installation.gpsLng,
    altitude: installation.gpsAltitude ?? null,
    horizontalAccuracy: installation.gpsAccuracy ?? 0,
    gpsSource: (installation.gpsSource as GpsSource) ?? 'internal_gps',
    correctionService: (installation.gpsCorrService as GpsCorrService) ?? 'none',
    rtkFixStatus: (installation.gpsRtkStatus as GpsRtkStatus) ?? 'autonomous',
    satelliteCount: installation.gpsSatCount ?? null,
    hdop: installation.gpsHdop ?? null,
    correctionAge: installation.gpsCorrAge ?? null,
    isMocked:
      installation.gpsSource === 'external_dgnss' || installation.gpsSource === 'external_rtk',
  };
}
