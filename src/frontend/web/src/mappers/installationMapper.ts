import type { ProjectId } from '@baudoku/core';
import type { InstallationFormData } from '@baudoku/documentation';
import type { GpsFormData } from '@/components/installations/GpsPositionSelector';

export type CreateInstallationInput = InstallationFormData & { zoneId: string; gps: GpsFormData | null };

export function toBackendRequest(data: CreateInstallationInput, projectId: ProjectId) {
  return {
    projectId,
    zoneId: data.zoneId || null,
    type: data.type,
    position: data.gps
      ? {
          latitude: data.gps.latitude,
          longitude: data.gps.longitude,
          altitude: data.gps.altitude,
          horizontalAccuracy: data.gps.accuracy,
          gpsSource: data.gps.source === 'browser' ? 'internal_gps' : 'internal_gps',
          correctionService: null,
          rtkFixStatus: null,
          satelliteCount: null,
          hdop: null,
          correctionAge: null,
        }
      : null,
    description: data.notes || null,
    cableType: data.cableType || null,
    crossSection: data.crossSectionMm2 ?? null,
    cableColor: null,
    conductorCount: null,
    depthMm: data.depthMm ?? null,
    manufacturer: data.manufacturer || null,
    modelName: data.model || null,
    serialNumber: data.serialNumber || null,
  };
}
