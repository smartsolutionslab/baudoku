import { useParams, useNavigate } from "@tanstack/react-router";
import { useZones } from "@/hooks/useProjects";
import { useCreateInstallation } from "@/hooks/useInstallations";
import { InstallationForm } from "@/components/installations/InstallationForm";
import type { InstallationFormData } from "@baudoku/shared-validation";

export function InstallationNewPage() {
  const { projectId } = useParams({ strict: false }) as { projectId: string };
  const navigate = useNavigate();
  const { data: zones } = useZones(projectId);
  const createInstallation = useCreateInstallation(projectId);

  const handleSubmit = async (
    data: InstallationFormData & { zoneId: string }
  ) => {
    await createInstallation.mutateAsync(data);
    navigate({
      to: "/projects/$projectId/installations",
      params: { projectId },
    });
  };

  return (
    <div className="mx-auto max-w-3xl">
      <h1 className="text-2xl font-bold text-gray-900">Neue Installation</h1>
      <p className="mt-1 text-sm text-gray-500">
        Dokumentieren Sie eine neue Installation.
      </p>

      <div className="mt-6">
        <InstallationForm
          zones={zones ?? []}
          onSubmit={handleSubmit}
          onCancel={() =>
            navigate({
              to: "/projects/$projectId/installations",
              params: { projectId },
            })
          }
          isSubmitting={createInstallation.isPending}
        />
      </div>
    </div>
  );
}
