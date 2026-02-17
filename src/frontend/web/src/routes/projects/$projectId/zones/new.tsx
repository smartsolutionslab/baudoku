import { useParams, useNavigate } from "@tanstack/react-router";
import { useZones, useCreateZone } from "../../../../hooks/useProjects";
import { ZoneForm } from "../../../../components/projects/ZoneForm";
import type { ZoneFormData } from "@baudoku/shared-validation";

export function ZoneNewPage() {
  const { projectId } = useParams({ strict: false }) as { projectId: string };
  const navigate = useNavigate();
  const { data: zones } = useZones(projectId);
  const createZone = useCreateZone(projectId);

  const handleSubmit = async (data: ZoneFormData) => {
    await createZone.mutateAsync(data);
    navigate({
      to: "/projects/$projectId",
      params: { projectId },
    });
  };

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="text-2xl font-bold text-gray-900">Neue Zone</h1>
      <p className="mt-1 text-sm text-gray-500">
        Fügen Sie eine neue Zone zum Projekt hinzu.
      </p>

      <div className="mt-6">
        <ZoneForm
          zones={zones ?? []}
          onSubmit={handleSubmit}
          onCancel={() =>
            navigate({
              to: "/projects/$projectId",
              params: { projectId },
            })
          }
          isSubmitting={createZone.isPending}
        />
      </div>
    </div>
  );
}
