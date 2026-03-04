import { useNavigate } from '@tanstack/react-router';
import { useZones, useCreateZone, useProjectIdParam } from '@/hooks';
import { ZoneForm } from '@/components/projects';
import type { ZoneFormData } from '@baudoku/projects';

export function ZoneNewPage() {
  const projectId = useProjectIdParam();
  const navigate = useNavigate();
  const { data: zones } = useZones(projectId);
  const createZone = useCreateZone(projectId);

  const handleCancel = () => navigate({ to: '/projects/$projectId', params: { projectId } });

  const handleSubmit = async (data: ZoneFormData) => {
    await createZone.mutateAsync(data);
    navigate({
      to: '/projects/$projectId',
      params: { projectId },
    });
  };

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="text-2xl font-bold text-gray-900">Neue Zone</h1>
      <p className="mt-1 text-sm text-gray-500">Fügen Sie eine neue Zone zum Projekt hinzu.</p>

      <div className="mt-6">
        <ZoneForm
          zones={zones ?? []}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          isSubmitting={createZone.isPending}
        />
      </div>
    </div>
  );
}
