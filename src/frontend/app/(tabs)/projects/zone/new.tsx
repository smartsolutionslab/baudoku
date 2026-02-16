import { useLocalSearchParams, useRouter } from "expo-router";
import {
  useZonesByProject,
  useCreateZone,
} from "../../../../src/hooks";
import { ZoneForm } from "../../../../src/components/projects";
import type { ZoneFormData } from "../../../../src/validation/schemas";
import { projectId as toProjectId } from "../../../../src/types/branded";

export default function NewZoneScreen() {
  const { projectId: rawProjectId, parentZoneId } = useLocalSearchParams<{
    projectId: string;
    parentZoneId?: string;
  }>();
  const projectId = toProjectId(rawProjectId!);
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
