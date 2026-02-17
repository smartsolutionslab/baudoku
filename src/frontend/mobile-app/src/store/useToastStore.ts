import { create } from "zustand";

export type ToastType = "success" | "error" | "warning" | "info";

export type ToastItem = {
  id: string;
  message: string;
  type: ToastType;
  duration: number;
};

type ToastState = {
  toasts: ToastItem[];
  show: (message: string, type?: ToastType, duration?: number) => void;
  dismiss: (id: string) => void;
};

export const useToastStore = create<ToastState>((set) => ({
  toasts: [],

  show: (message, type = "info", duration = 4000) =>
    set((state) => ({
      toasts: [
        ...state.toasts,
        {
          id: Date.now().toString(36) + Math.random().toString(36).slice(2),
          message,
          type,
          duration,
        },
      ],
    })),

  dismiss: (id) =>
    set((state) => ({
      toasts: state.toasts.filter((t) => t.id !== id),
    })),
}));
