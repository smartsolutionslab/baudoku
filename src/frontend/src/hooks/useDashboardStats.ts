import { useQuery } from "@tanstack/react-query";
import * as projectRepo from "../db/repositories/projectRepo";
import * as installationRepo from "../db/repositories/installationRepo";
import * as photoRepo from "../db/repositories/photoRepo";
import * as measurementRepo from "../db/repositories/measurementRepo";
import * as syncRepo from "../db/repositories/syncRepo";

export type DashboardStats = {
  projectCount: number;
  installationCount: number;
  installationsByStatus: Record<string, number>;
  photoCount: number;
  measurementsByResult: Record<string, number>;
  unsyncedCount: number;
};

export function useDashboardStats() {
  return useQuery({
    queryKey: ["dashboardStats"],
    queryFn: async (): Promise<DashboardStats> => {
      const [
        projects,
        allInstallations,
        installationsByStatus,
        photoCount,
        measurementsByResult,
        unsyncedCount,
      ] = await Promise.all([
        projectRepo.getAll(),
        installationRepo.getAll(),
        installationRepo.getCountByStatus(),
        photoRepo.getCount(),
        measurementRepo.getCountByResult(),
        syncRepo.getUnsyncedCount(),
      ]);

      return {
        projectCount: projects.length,
        installationCount: allInstallations.length,
        installationsByStatus,
        photoCount,
        measurementsByResult,
        unsyncedCount,
      };
    },
  });
}
