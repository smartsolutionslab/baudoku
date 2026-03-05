import type { GpsSource, GpsCorrectionService, RtkFixStatus } from '@baudoku/documentation';
import type { GpsPosition } from '../hooks/useGpsCapture';
import type { Installation } from '../db/repositories/types';

export function toGpsPosition(installation: Installation): GpsPosition | null {
  if (installation.gpsLat == null || installation.gpsLng == null) return null;
  return {
    latitude: installation.gpsLat,
    longitude: installation.gpsLng,
    altitude: installation.gpsAltitude ?? null,
    horizontalAccuracy: installation.gpsAccuracy ?? 0,
    gpsSource: (installation.gpsSource as GpsSource) ?? 'internal_gps',
    correctionService: (installation.gpsCorrService as GpsCorrectionService) ?? 'none',
    rtkFixStatus: (installation.gpsRtkStatus as RtkFixStatus) ?? 'autonomous',
    satelliteCount: installation.gpsSatCount ?? null,
    hdop: installation.gpsHdop ?? null,
    correctionAge: installation.gpsCorrAge ?? null,
    isMocked:
      installation.gpsSource === 'external_dgnss' || installation.gpsSource === 'external_rtk',
  };
}
