import NetInfo from "@react-native-community/netinfo";
import { useSyncStore } from "../store/useSyncStore";

let unsubscribe: (() => void) | null = null;

export function startConnectivityMonitor(): void {
  if (unsubscribe) return;

  unsubscribe = NetInfo.addEventListener((state) => {
    useSyncStore.getState().setOnline(!!state.isConnected);
  });
}

export function stopConnectivityMonitor(): void {
  unsubscribe?.();
  unsubscribe = null;
}
