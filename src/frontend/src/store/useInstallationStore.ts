import { create } from "zustand";
import * as installationRepo from "../db/repositories/installationRepo";
import * as photoRepo from "../db/repositories/photoRepo";
import * as measurementRepo from "../db/repositories/measurementRepo";
import type {
  Installation,
  Photo,
  Measurement,
  NewInstallation,
  NewPhoto,
  NewMeasurement,
} from "../db/repositories/types";
import {
  type ProjectId,
  type ZoneId,
  type InstallationId,
  type PhotoId,
  type MeasurementId,
  zoneId as toZoneId,
  installationId as toInstallationId,
} from "../types/branded";

type InstallationState = {
  installations: Installation[];
  photos: Photo[];
  measurements: Measurement[];
  loading: boolean;

  loadByZone: (zoneId: ZoneId) => Promise<void>;
  loadByProject: (projectId: ProjectId) => Promise<void>;
  loadDetails: (installationId: InstallationId) => Promise<void>;

  createInstallation: (
    data: Omit<NewInstallation, "id" | "createdAt" | "updatedAt" | "version">
  ) => Promise<Installation>;
  updateInstallation: (
    id: InstallationId,
    data: Partial<
      Omit<
        NewInstallation,
        "id" | "createdAt" | "updatedAt" | "version" | "projectId" | "zoneId"
      >
    >
  ) => Promise<Installation | undefined>;
  deleteInstallation: (id: InstallationId) => Promise<void>;

  addPhoto: (data: Omit<NewPhoto, "id" | "version">) => Promise<Photo>;
  deletePhoto: (id: PhotoId) => Promise<void>;

  addMeasurement: (
    data: Omit<NewMeasurement, "id" | "version" | "result">
  ) => Promise<Measurement>;
  updateMeasurement: (
    id: MeasurementId,
    data: Partial<
      Omit<NewMeasurement, "id" | "version" | "installationId" | "result">
    >
  ) => Promise<Measurement | undefined>;
  deleteMeasurement: (id: MeasurementId) => Promise<void>;
};

export const useInstallationStore = create<InstallationState>((set) => ({
  installations: [],
  photos: [],
  measurements: [],
  loading: false,

  loadByZone: async (zoneId) => {
    set({ loading: true });
    const installations = await installationRepo.getByZoneId(zoneId);
    set({ installations, loading: false });
  },

  loadByProject: async (projectId) => {
    set({ loading: true });
    const installations = await installationRepo.getByProjectId(projectId);
    set({ installations, loading: false });
  },

  loadDetails: async (installationId) => {
    const [photos, measurements] = await Promise.all([
      photoRepo.getByInstallationId(installationId),
      measurementRepo.getByInstallationId(installationId),
    ]);
    set({ photos, measurements });
  },

  createInstallation: async (data) => {
    const installation = await installationRepo.create(data);
    const installations = await installationRepo.getByZoneId(toZoneId(data.zoneId));
    set({ installations });
    return installation;
  },

  updateInstallation: async (id, data) => {
    const installation = await installationRepo.update(id, data);
    if (installation) {
      const installations = await installationRepo.getByZoneId(
        installation.zoneId
      );
      set({ installations });
    }
    return installation;
  },

  deleteInstallation: async (id) => {
    const installation = await installationRepo.getById(id);
    await installationRepo.remove(id);
    if (installation) {
      const installations = await installationRepo.getByZoneId(
        installation.zoneId
      );
      set({ installations });
    }
  },

  addPhoto: async (data) => {
    const photo = await photoRepo.create(data);
    const photos = await photoRepo.getByInstallationId(toInstallationId(data.installationId));
    set({ photos });
    return photo;
  },

  deletePhoto: async (id) => {
    const photo = await photoRepo.getById(id);
    await photoRepo.remove(id);
    if (photo) {
      const photos = await photoRepo.getByInstallationId(
        photo.installationId
      );
      set({ photos });
    }
  },

  addMeasurement: async (data) => {
    const measurement = await measurementRepo.create(data);
    const measurements = await measurementRepo.getByInstallationId(
      toInstallationId(data.installationId)
    );
    set({ measurements });
    return measurement;
  },

  updateMeasurement: async (id, data) => {
    const measurement = await measurementRepo.update(id, data);
    if (measurement) {
      const measurements = await measurementRepo.getByInstallationId(
        measurement.installationId
      );
      set({ measurements });
    }
    return measurement;
  },

  deleteMeasurement: async (id) => {
    const measurement = await measurementRepo.getById(id);
    await measurementRepo.remove(id);
    if (measurement) {
      const measurements = await measurementRepo.getByInstallationId(
        measurement.installationId
      );
      set({ measurements });
    }
  },
}));
