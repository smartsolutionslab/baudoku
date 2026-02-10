import { create } from "zustand";
import * as projectRepo from "../db/repositories/projectRepo";
import * as zoneRepo from "../db/repositories/zoneRepo";
import type { Project, Zone, NewProject, NewZone } from "../db/repositories/types";

interface ProjectState {
  projects: Project[];
  zones: Zone[];
  loading: boolean;

  loadProjects: () => Promise<void>;
  loadZonesByProject: (projectId: string) => Promise<void>;

  createProject: (
    data: Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version">
  ) => Promise<Project>;
  updateProject: (
    id: string,
    data: Partial<Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version" | "createdBy">>
  ) => Promise<Project | undefined>;
  deleteProject: (id: string) => Promise<void>;

  createZone: (data: Omit<NewZone, "id" | "version">) => Promise<Zone>;
  updateZone: (
    id: string,
    data: Partial<Omit<NewZone, "id" | "version" | "projectId">>
  ) => Promise<Zone | undefined>;
  deleteZone: (id: string) => Promise<void>;
}

export const useProjectStore = create<ProjectState>((set) => ({
  projects: [],
  zones: [],
  loading: false,

  loadProjects: async () => {
    set({ loading: true });
    const projects = await projectRepo.getAll();
    set({ projects, loading: false });
  },

  loadZonesByProject: async (projectId) => {
    const zones = await zoneRepo.getByProjectId(projectId);
    set({ zones });
  },

  createProject: async (data) => {
    const project = await projectRepo.create(data);
    const projects = await projectRepo.getAll();
    set({ projects });
    return project;
  },

  updateProject: async (id, data) => {
    const project = await projectRepo.update(id, data);
    const projects = await projectRepo.getAll();
    set({ projects });
    return project;
  },

  deleteProject: async (id) => {
    await projectRepo.remove(id);
    const projects = await projectRepo.getAll();
    set({ projects });
  },

  createZone: async (data) => {
    const zone = await zoneRepo.create(data);
    const zones = await zoneRepo.getByProjectId(data.projectId);
    set({ zones });
    return zone;
  },

  updateZone: async (id, data) => {
    const zone = await zoneRepo.update(id, data);
    if (zone) {
      const zones = await zoneRepo.getByProjectId(zone.projectId);
      set({ zones });
    }
    return zone;
  },

  deleteZone: async (id) => {
    const zone = await zoneRepo.getById(id);
    await zoneRepo.remove(id);
    if (zone) {
      const zones = await zoneRepo.getByProjectId(zone.projectId);
      set({ zones });
    }
  },
}));
