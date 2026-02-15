import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import * as projectRepo from "../db/repositories/projectRepo";
import type { NewProject } from "../db/repositories/types";
import type { ProjectId } from "../types/branded";

export function useProjects() {
  return useQuery({
    queryKey: ["projects"],
    queryFn: () => projectRepo.getAll(),
  });
}

export function useProject(id: ProjectId) {
  return useQuery({
    queryKey: ["projects", id],
    queryFn: () => projectRepo.getById(id),
  });
}

export function useCreateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (
      data: Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version">
    ) => projectRepo.create(data),
    meta: { errorMessage: "Projekt konnte nicht erstellt werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useUpdateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: ProjectId;
      data: Partial<
        Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version" | "createdBy">
      >;
    }) => projectRepo.update(id, data),
    meta: { errorMessage: "Projekt konnte nicht aktualisiert werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useDeleteProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: ProjectId) => projectRepo.remove(id),
    meta: { errorMessage: "Projekt konnte nicht gelöscht werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}
