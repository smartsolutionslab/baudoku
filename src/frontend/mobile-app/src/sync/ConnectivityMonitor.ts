import NetInfo from "@react-native-community/netinfo";
import { useSyncStore } from "../store";

let unsubscribe: (() => void) | null = null;
let wasOffline = false;
let onReconnect: (() => void) | null = null;

export function setOnReconnect(callback: () => void): void {
  onReconnect = callback;
}

export function startConnectivityMonitor(): void {
  if (unsubscribe) return;

  unsubscribe = NetInfo.addEventListener((state) => {
    const isOnline = !!state.isConnected;
    const store = useSyncStore.getState();

    store.setOnline(isOnline);

    if (isOnline && wasOffline && onReconnect) {
      onReconnect();
    }

    wasOffline = !isOnline;
  });
}

export function stopConnectivityMonitor(): void {
  unsubscribe?.();
  unsubscribe = null;
  onReconnect = null;
}
