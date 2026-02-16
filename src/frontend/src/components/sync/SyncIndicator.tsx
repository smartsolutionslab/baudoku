import React from "react";
import { View, Text, StyleSheet } from "react-native";
import { useSyncStatus } from "../../hooks/useSyncStatus";

export function SyncIndicator() {
  const { isOnline, unsyncedCount } = useSyncStatus();

  return (
    <View style={styles.container}>
      <View
        style={[styles.dot, isOnline ? styles.online : styles.offline]}
      />
      <Text style={styles.text}>
        {isOnline ? "Online" : "Offline"}
        {unsyncedCount > 0 && ` (${unsyncedCount})`}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    alignItems: "center",
    paddingHorizontal: 12,
    paddingVertical: 4,
  },
  dot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6,
  },
  online: {
    backgroundColor: "#34C759",
  },
  offline: {
    backgroundColor: "#FF3B30",
  },
  text: {
    fontSize: 13,
    color: "#666",
  },
});
