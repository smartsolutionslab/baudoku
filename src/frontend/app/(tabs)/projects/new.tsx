import { useRouter } from "expo-router";
import { useCreateProject } from "../../../src/hooks/useOfflineData";
import { ProjectForm } from "../../../src/components/projects/ProjectForm";
import type { ProjectFormData } from "../../../src/validation/schemas";

export default function NewProjectScreen() {
  const router = useRouter();
  const createProject = useCreateProject();

  const handleSubmit = async (data: ProjectFormData) => {
    try {
      await createProject.mutateAsync({
        name: data.name,
        status: data.status,
        street: data.street || null,
        city: data.city || null,
        zipCode: data.zipCode || null,
        clientName: data.clientName || null,
        clientContact: data.clientContact || null,
        createdBy: "local-user",
      });
      router.back();
    } catch {
      // Global MutationCache.onError shows toast
    }
  };

  return (
    <ProjectForm onSubmit={handleSubmit} submitting={createProject.isPending} />
  );
}
