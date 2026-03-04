import { useLocalSearchParams, useRouter } from 'expo-router';
import { useCreateInstallation, type GpsPosition } from '@/hooks';
import { InstallationForm } from '@/components/installations';
import type { InstallationFormData } from '@/validation/schemas';
import { requiredParam } from '@/utils';

export default function NewInstallationScreen() {
  const { projectId, zoneId } = useLocalSearchParams<{
    projectId: string;
    zoneId: string;
  }>();
  const pid = requiredParam(projectId);
  const zid = requiredParam(zoneId);
  const router = useRouter();
  const createInstallation = useCreateInstallation();

  const handleSubmit = async (data: InstallationFormData, gps: GpsPosition | null) => {
    try {
      const result = await createInstallation.mutateAsync({
        projectId: pid,
        zoneId: zid,
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
        // GPS fields (map CapturedGpsPosition → flat DB columns)
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
      });
      router.replace(`/(tabs)/projects/installation/${result.id}`);
    } catch {
      // Global MutationCache.onError shows toast
    }
  };

  return <InstallationForm onSubmit={handleSubmit} submitting={createInstallation.isPending} />;
}
