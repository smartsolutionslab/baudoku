import { View, Text, StyleSheet } from "react-native";
import { useSyncStatus } from "../../hooks";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

export function SyncIndicator() {
  const { isOnline, unsyncedCount } = useSyncStatus();

  return (
    <View style={styles.container}>
      <View style={[styles.dot, isOnline ? styles.online : styles.offline]}/>
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
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
  },
  dot: {
    width: 8,
    height: 8,
    borderRadius: Radius.xs,
    marginRight: 6,
  },
  online: {
    backgroundColor: Colors.success,
  },
  offline: {
    backgroundColor: Colors.danger,
  },
  text: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
  },
});
