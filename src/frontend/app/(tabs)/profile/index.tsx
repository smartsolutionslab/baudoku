import React from "react";
import { View, Text, ScrollView, StyleSheet } from "react-native";
import { useProjects } from "../../../src/hooks/useOfflineData";
import { useSyncStatus } from "../../../src/hooks/useSyncStatus";
import { Colors, Spacing, FontSize } from "../../../src/styles/tokens";

export default function ProfileScreen() {
  const { data: projects } = useProjects();
  const { isOnline, unsyncedCount, lastSyncTimestamp } = useSyncStatus();

  const projectCount = projects?.length ?? 0;

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.screenTitle}>Profil</Text>

      {/* App-Info */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>App-Info</Text>
        <Row label="App" value="BauDoku" />
        <Row label="Version" value="0.9.0" />
        <Row label="Build" value="Sprint 9" />
      </View>

      {/* Gerät */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Gerät</Text>
        <View style={styles.row}>
          <Text style={styles.label}>Online-Status</Text>
          <View style={styles.statusValue}>
            <View
              style={[
                styles.dot,
                { backgroundColor: isOnline ? Colors.success : Colors.danger },
              ]}
            />
            <Text style={styles.value}>
              {isOnline ? "Online" : "Offline"}
            </Text>
          </View>
        </View>
      </View>

      {/* Datenbank */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Datenbank</Text>
        <Row label="Projekte" value={String(projectCount)} />
      </View>

      {/* Sync */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Synchronisation</Text>
        <Row label="Ausstehend" value={`${unsyncedCount} Änderungen`} />
        <Row
          label="Letzte Sync"
          value={lastSyncTimestamp ?? "Noch nicht synchronisiert"}
        />
      </View>

      {/* Login placeholder */}
      <View style={[styles.card, styles.cardDisabled]}>
        <Text style={styles.cardTitle}>Anmeldung</Text>
        <Text style={styles.disabledText}>
          Anmeldung kommt in einem zukünftigen Update.
        </Text>
      </View>
    </ScrollView>
  );
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <View style={styles.row}>
      <Text style={styles.label}>{label}</Text>
      <Text style={styles.value}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  content: {
    padding: Spacing.lg,
    paddingBottom: 40,
  },
  screenTitle: {
    fontSize: FontSize.title,
    fontWeight: "700",
    marginBottom: Spacing.lg,
  },
  card: {
    backgroundColor: Colors.card,
    borderRadius: 12,
    padding: Spacing.lg,
    marginBottom: Spacing.lg,
    gap: Spacing.md,
  },
  cardDisabled: {
    opacity: 0.5,
  },
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
  },
  row: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  label: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
  value: {
    fontSize: FontSize.body,
    fontWeight: "500",
    color: Colors.textPrimary,
  },
  statusValue: {
    flexDirection: "row",
    alignItems: "center",
  },
  dot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6,
  },
  disabledText: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
    fontStyle: "italic",
  },
});
