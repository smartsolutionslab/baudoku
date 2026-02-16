import { useLocalSearchParams, useRouter } from "expo-router";
import { useCreateInstallation, type GpsPosition } from "../../../src/hooks";
import { InstallationForm } from "../../../src/components/installations";
import type { InstallationFormData } from "../../../src/validation/schemas";

export default function NewInstallationScreen() {
  const { projectId, zoneId } = useLocalSearchParams<{
    projectId: string;
    zoneId: string;
  }>();
  const router = useRouter();
  const createInstallation = useCreateInstallation();

  const handleSubmit = async (
    data: InstallationFormData,
    gps: GpsPosition | null
  ) => {
    try {
      const result = await createInstallation.mutateAsync({
        projectId: projectId!,
        zoneId: zoneId!,
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
        // GPS fields
        gpsLat: gps?.gpsLat ?? null,
        gpsLng: gps?.gpsLng ?? null,
        gpsAltitude: gps?.gpsAltitude ?? null,
        gpsAccuracy: gps?.gpsAccuracy ?? null,
        gpsSource: gps?.gpsSource ?? null,
        gpsCorrService: gps?.gpsCorrService ?? null,
        gpsRtkStatus: gps?.gpsRtkStatus ?? null,
        gpsSatCount: gps?.gpsSatCount ?? null,
        gpsHdop: gps?.gpsHdop ?? null,
        gpsCorrAge: gps?.gpsCorrAge ?? null,
      });
      router.replace(`/(tabs)/projects/installation/${result.id}`);
    } catch {
      // Global MutationCache.onError shows toast
    }
  };

  return (
    <InstallationForm
      onSubmit={handleSubmit}
      submitting={createInstallation.isPending}
    />
  );
}
