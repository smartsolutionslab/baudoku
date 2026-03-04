import { useLocalSearchParams, useRouter, Stack } from 'expo-router';
import { useQuery } from '@tanstack/react-query';
import * as installationRepo from '@/db/repositories/installationRepo';
import { useUpdateInstallation, type GpsPosition } from '@/hooks';
import { InstallationForm } from '@/components/installations';
import type { InstallationFormData } from '@/validation/schemas';
import { installationId } from '@baudoku/core';
import { toGpsPosition, requiredParam } from '@/utils';

export default function EditInstallationScreen() {
  const { id: rawId } = useLocalSearchParams<{ id: string }>();
  const id = installationId(requiredParam(rawId));
  const router = useRouter();
  const { data: installation } = useQuery({
    queryKey: ['installation', id],
    queryFn: () => installationRepo.getById(id),
    enabled: !!id,
  });
  const updateInstallation = useUpdateInstallation();

  if (!installation) return null;

  const initialValues: Partial<InstallationFormData> = {
    type: installation.type,
    status: installation.status as InstallationFormData['status'],
    manufacturer: installation.manufacturer ?? undefined,
    model: installation.model ?? undefined,
    serialNumber: installation.serialNumber ?? undefined,
    cableType: installation.cableType ?? undefined,
    crossSectionMm2: installation.crossSectionMm2 ?? undefined,
    lengthM: installation.lengthM ?? undefined,
    circuitId: installation.circuitId ?? undefined,
    fuseType: installation.fuseType ?? undefined,
    fuseRatingA: installation.fuseRatingA ?? undefined,
    voltageV: installation.voltageV ?? undefined,
    phase: (installation.phase as InstallationFormData['phase']) ?? undefined,
    depthMm: installation.depthMm ?? undefined,
    notes: installation.notes ?? undefined,
  };

  const initialGps: GpsPosition | null = toGpsPosition(installation);

  const handleSubmit = async (data: InstallationFormData, gps: GpsPosition | null) => {
    try {
      await updateInstallation.mutateAsync({
        id,
        data: {
          type: data.type,
          status: data.status,
          manufacturer: data.manufacturer || null,
          model: data.model || null,
          serialNumber: data.serialNumber || null,
          cableType: data.cableType || null,
          crossSectionMm2: data.crossSectionMm2 ?? null,
          lengthM: data.lengthM ?? null,
          circuitId: data.circuitId || null,
          fuseType: data.fuseType || null,
          fuseRatingA: data.fuseRatingA ?? null,
          voltageV: data.voltageV ?? null,
          phase: data.phase || null,
          depthMm: data.depthMm ?? null,
          notes: data.notes || null,
          gpsLat: gps?.latitude ?? null,
          gpsLng: gps?.longitude ?? null,
          gpsAltitude: gps?.altitude ?? null,
          gpsAccuracy: gps?.horizontalAccuracy ?? null,
          gpsSource: gps?.gpsSource ?? null,
          gpsCorrService: gps?.correctionService ?? null,
          gpsRtkStatus: gps?.rtkFixStatus ?? null,
          gpsSatCount: gps?.satelliteCount ?? null,
          gpsHdop: gps?.hdop ?? null,
          gpsCorrAge: gps?.correctionAge ?? null,
        },
      });
      router.back();
    } catch {
      // Global MutationCache.onError shows toast
    }
  };

  return (
    <>
      <Stack.Screen options={{ title: 'Installation bearbeiten' }} />
      <InstallationForm
        onSubmit={handleSubmit}
        submitting={updateInstallation.isPending}
        initialValues={initialValues}
        initialGps={initialGps}
        submitLabel="Aktualisieren"
      />
    </>
  );
}
