import { useLocalSearchParams, useRouter } from 'expo-router';
import { useZonesByProject, useCreateZone, useProjectIdParam } from '@/hooks';
import { ZoneForm } from '@/components/projects';
import type { ZoneFormData } from '@/validation/schemas';
import { zoneId as toZoneId } from '@baudoku/core';

export default function NewZoneScreen() {
  const { parentZoneId: rawParentZoneId } = useLocalSearchParams<{
    parentZoneId?: string;
  }>();
  const projectId = useProjectIdParam('projectId');
  const parentZoneId = rawParentZoneId ? toZoneId(rawParentZoneId) : undefined;
  const router = useRouter();
  const { data: zones } = useZonesByProject(projectId);
  const createZone = useCreateZone();

  const handleSubmit = async (data: ZoneFormData) => {
    try {
      await createZone.mutateAsync({
        projectId,
        name: data.name,
        type: data.type,
        parentZoneId: data.parentZoneId ?? null,
        sortOrder: data.sortOrder ?? 0,
      });
      router.back();
    } catch {
      // Global MutationCache.onError shows toast
    }
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
