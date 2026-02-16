import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import { useQuery } from "@tanstack/react-query";
import * as installationRepo from "../../../../src/db/repositories/installationRepo";
import { useUpdateInstallation } from "../../../../src/hooks/useOfflineData";
import { InstallationForm } from "../../../../src/components/installations/InstallationForm";
import type { InstallationFormData } from "../../../../src/validation/schemas";
import type { GpsPosition, GpsSource, GpsCorrService, GpsRtkStatus } from "../../../../src/hooks/useGpsCapture";
import { installationId } from "../../../../src/types/branded";

export default function EditInstallationScreen() {
  const { id: rawId } = useLocalSearchParams<{ id: string }>();
  const id = installationId(rawId!);
  const router = useRouter();
  const { data: installation } = useQuery({
    queryKey: ["installation", id],
    queryFn: () => installationRepo.getById(id),
    enabled: !!id,
  });
  const updateInstallation = useUpdateInstallation();

  if (!installation) return null;

  const initialValues: Partial<InstallationFormData> = {
    type: installation.type,
    status: installation.status as InstallationFormData["status"],
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
    phase: (installation.phase as InstallationFormData["phase"]) ?? undefined,
    depthMm: installation.depthMm ?? undefined,
    notes: installation.notes ?? undefined,
  };

  const initialGps: GpsPosition | null =
    installation.gpsLat != null && installation.gpsLng != null
      ? {
          gpsLat: installation.gpsLat,
          gpsLng: installation.gpsLng,
          gpsAltitude: installation.gpsAltitude ?? null,
          gpsAccuracy: installation.gpsAccuracy ?? 0,
          gpsSource: (installation.gpsSource as GpsSource) ?? "internal_gps",
          gpsCorrService: (installation.gpsCorrService as GpsCorrService) ?? "none",
          gpsRtkStatus: (installation.gpsRtkStatus as GpsRtkStatus) ?? "autonomous",
          gpsSatCount: installation.gpsSatCount ?? null,
          gpsHdop: installation.gpsHdop ?? null,
          gpsCorrAge: installation.gpsCorrAge ?? null,
          isMocked: installation.gpsSource === "external_dgnss" || installation.gpsSource === "external_rtk",
        }
      : null;

  const handleSubmit = async (
    data: InstallationFormData,
    gps: GpsPosition | null
  ) => {
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
        },
      });
      router.back();
    } catch {
      // Global MutationCache.onError shows toast
    }
  };

  return (
    <>
      <Stack.Screen options={{ title: "Installation bearbeiten" }} />
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
