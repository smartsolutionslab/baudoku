import { create } from "zustand";

export type UploadStatus = "queued" | "uploading" | "completed" | "failed";

export interface UploadItem {
  photoId: string;
  status: UploadStatus;
  percentage: number;
  error?: string;
}

export interface QueueSummary {
  total: number;
  completed: number;
  failed: number;
  uploading: number;
}

interface UploadState {
  queue: UploadItem[];
  isProcessing: boolean;

  enqueue: (photoId: string) => void;
  updateProgress: (photoId: string, percentage: number) => void;
  markCompleted: (photoId: string) => void;
  markFailed: (photoId: string, error: string) => void;
  setProcessing: (processing: boolean) => void;
  removeCompleted: () => void;
  getQueueSummary: () => QueueSummary;
}

export const useUploadStore = create<UploadState>((set, get) => ({
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

  getQueueSummary: () => {
    const { queue } = get();
    return {
      total: queue.length,
      completed: queue.filter((i) => i.status === "completed").length,
      failed: queue.filter((i) => i.status === "failed").length,
      uploading: queue.filter(
        (i) => i.status === "uploading" || i.status === "queued"
      ).length,
    };
  },
}));
