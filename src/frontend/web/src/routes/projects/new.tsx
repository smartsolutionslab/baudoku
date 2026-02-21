import { useNavigate } from "@tanstack/react-router";
import { useCreateProject } from "@/hooks/useProjects";
import { ProjectForm } from "@/components/projects/ProjectForm";
import type { ProjectFormData } from "@baudoku/projects";

export function ProjectNewPage() {
  const navigate = useNavigate();
  const createProject = useCreateProject();

  const handleSubmit = async (data: ProjectFormData) => {
    const project = await createProject.mutateAsync(data);
    navigate({ to: "/projects/$projectId", params: { projectId: project.id } });
  };

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="text-2xl font-bold text-gray-900">Neues Projekt</h1>
      <p className="mt-1 text-sm text-gray-500">
        Erstellen Sie ein neues Bauprojekt.
      </p>

      <div className="mt-6">
        <ProjectForm
          onSubmit={handleSubmit}
          isSubmitting={createProject.isPending}
        />
      </div>
    </div>
  );
}
