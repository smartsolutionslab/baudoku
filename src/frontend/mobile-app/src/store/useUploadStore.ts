import { create } from "zustand";
import type { PhotoId } from "../types/branded";

export type UploadStatus = "queued" | "uploading" | "completed" | "failed";

export type UploadItem = {
  photoId: PhotoId;
  status: UploadStatus;
  percentage: number;
  error?: string;
};

type UploadState = {
  queue: UploadItem[];
  isProcessing: boolean;

  enqueue: (photoId: PhotoId) => void;
  updateProgress: (photoId: PhotoId, percentage: number) => void;
  markCompleted: (photoId: PhotoId) => void;
  markFailed: (photoId: PhotoId, error: string) => void;
  setProcessing: (processing: boolean) => void;
  removeCompleted: () => void;
};

export const useUploadStore = create<UploadState>((set) => ({
  queue: [],
  isProcessing: false,

  enqueue: (photoId) =>
    set((state) => ({
      queue: [
        ...state.queue.filter((item) => item.photoId !== photoId),
        { photoId, status: "queued" as UploadStatus, percentage: 0 },
      ],
    })),

  updateProgress: (photoId, percentage) =>
    set((state) => ({
      queue: state.queue.map((item) =>
        item.photoId === photoId
          ? { ...item, status: "uploading" as UploadStatus, percentage }
          : item
      ),
    })),

  markCompleted: (photoId) =>
    set((state) => ({
      queue: state.queue.map((item) =>
        item.photoId === photoId
          ? { ...item, status: "completed" as UploadStatus, percentage: 100 }
          : item
      ),
    })),

  markFailed: (photoId, error) =>
    set((state) => ({
      queue: state.queue.map((item) =>
        item.photoId === photoId
          ? { ...item, status: "failed" as UploadStatus, error }
          : item
      ),
    })),

  setProcessing: (processing) => set({ isProcessing: processing }),

  removeCompleted: () =>
    set((state) => ({
      queue: state.queue.filter((item) => item.status !== "completed"),
    })),
}));
