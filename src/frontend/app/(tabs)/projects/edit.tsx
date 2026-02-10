import React from "react";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import {
  useProject,
  useUpdateProject,
} from "../../../src/hooks/useOfflineData";
import { ProjectForm } from "../../../src/components/projects/ProjectForm";
import type { ProjectFormData } from "../../../src/validation/schemas";

export default function EditProjectScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const router = useRouter();
  const { data: project } = useProject(id!);
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
    await updateProject.mutateAsync({ id: id!, data });
    router.back();
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
