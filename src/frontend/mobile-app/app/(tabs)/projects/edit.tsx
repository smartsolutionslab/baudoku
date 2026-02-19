import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import { useProject, useUpdateProject } from "@/hooks";
import { ProjectForm } from "@/components/projects";
import type { ProjectFormData } from "@/validation/schemas";
import { projectId } from "@/types/branded";

export default function EditProjectScreen() {
  const { id: rawId } = useLocalSearchParams<{ id: string }>();
  const id = projectId(rawId!);
  const router = useRouter();
  const { data: project } = useProject(id);
  const updateProject = useUpdateProject();

  if (!project) return null;

  const initialValues: Partial<ProjectFormData> = {
    name: project.name,
    status: project.status as ProjectFormData["status"],
    street: project.street ?? undefined,
    city: project.city ?? undefined,
    zipCode: project.zipCode ?? undefined,
    clientName: project.clientName ?? undefined,
    clientContact: project.clientContact ?? undefined,
  };

  const handleSubmit = async (data: ProjectFormData) => {
    try {
      await updateProject.mutateAsync({ id, data });
      router.back();
    } catch {
      // Global MutationCache.onError shows toast
    }
  };

  return (
    <>
      <Stack.Screen options={{ title: "Projekt bearbeiten" }} />
      <ProjectForm
        onSubmit={handleSubmit}
        submitting={updateProject.isPending}
        initialValues={initialValues}
        submitLabel="Aktualisieren"
      />
    </>
  );
}
