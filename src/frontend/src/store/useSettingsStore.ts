import { create } from "zustand";
import * as SecureStore from "expo-secure-store";

const MOCK_LOCATION_KEY = "baudoku_settings_mockLocation";

interface SettingsState {
  allowMockLocation: boolean;
  hydrated: boolean;
  setAllowMockLocation: (allow: boolean) => void;
  hydrate: () => Promise<void>;
}

export const useSettingsStore = create<SettingsState>((set) => ({
  allowMockLocation: true,
  hydrated: false,

  setAllowMockLocation: (allow) => {
    set({ allowMockLocation: allow });
    try {
      SecureStore.setItemAsync(MOCK_LOCATION_KEY, JSON.stringify(allow));
    } catch {
      // non-critical setting, ignore write errors
    }
  },

  hydrate: async () => {
    try {
      const stored = await SecureStore.getItemAsync(MOCK_LOCATION_KEY);
      if (stored !== null) {
        set({ allowMockLocation: JSON.parse(stored) });
      }
    } catch {
      // non-critical setting, ignore read errors
    }
    set({ hydrated: true });
  },
}));
