import React from "react";
import { useLocalSearchParams, useRouter } from "expo-router";
import {
  useZonesByProject,
  useCreateZone,
} from "../../../../src/hooks/useOfflineData";
import { ZoneForm } from "../../../../src/components/projects/ZoneForm";
import type { ZoneFormData } from "../../../../src/validation/schemas";

export default function NewZoneScreen() {
  const { projectId, parentZoneId } = useLocalSearchParams<{
    projectId: string;
    parentZoneId?: string;
  }>();
  const router = useRouter();
  const { data: zones } = useZonesByProject(projectId!);
  const createZone = useCreateZone();

  const handleSubmit = async (data: ZoneFormData) => {
    await createZone.mutateAsync({
      projectId: projectId!,
      name: data.name,
      type: data.type,
      parentZoneId: data.parentZoneId ?? null,
      sortOrder: data.sortOrder ?? 0,
    });
    router.back();
  };

  return (
    <ZoneForm
      zones={zones}
      defaultParentZoneId={parentZoneId}
      onSubmit={handleSubmit}
      submitting={createZone.isPending}
    />
  );
}
