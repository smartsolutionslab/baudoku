import React from "react";
import {
  View,
  Text,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
  StyleSheet,
} from "react-native";
import { useRouter, Stack } from "expo-router";
import { FontAwesome } from "@expo/vector-icons";
import { useDashboardStats } from "../../../src/hooks/useDashboardStats";
import { DashboardCard } from "../../../src/components/common/DashboardCard";
import { Colors, Spacing, FontSize, Radius } from "../../../src/styles/tokens";

export default function DashboardScreen() {
  const router = useRouter();
  const { data: stats, isLoading } = useDashboardStats();

  if (isLoading || !stats) {
    return (
      <View style={styles.loading}>
        <Stack.Screen options={{ title: "Dashboard" }} />
        <ActivityIndicator size="large" color={Colors.primary} />
      </View>
    );
  }

  const failedCount = stats.measurementsByResult.failed ?? 0;
  const passedCount = stats.measurementsByResult.passed ?? 0;
  const totalMeasurements = failedCount + passedCount + (stats.measurementsByResult.warning ?? 0);
  const inProgressCount = stats.installationsByStatus.in_progress ?? 0;

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Stack.Screen options={{ title: "Dashboard" }} />

      <Text style={styles.sectionTitle}>Gesamt</Text>
      <View style={styles.row}>
        <DashboardCard
          title="Projekte"
          value={stats.projectCount}
        />
        <View style={styles.gap} />
        <DashboardCard
          title="Installationen"
          value={stats.installationCount}
        />
      </View>

      <View style={styles.row}>
        <DashboardCard
          title="Fotos"
          value={stats.photoCount}
        />
        <View style={styles.gap} />
        <DashboardCard
          title="Messungen"
          value={totalMeasurements}
        />
      </View>

      <Text style={styles.sectionTitle}>Status</Text>
      <View style={styles.row}>
        <DashboardCard
          title="In Arbeit"
          value={inProgressCount}
          color={Colors.warning}
        />
        <View style={styles.gap} />
        <DashboardCard
          title="Fertig"
          value={stats.installationsByStatus.completed ?? 0}
          color={Colors.success}
        />
      </View>

      <View style={styles.row}>
        <DashboardCard
          title="Bestanden"
          value={passedCount}
          color={Colors.success}
          subtitle="Messungen"
        />
        <View style={styles.gap} />
        <DashboardCard
          title="Fehlgeschlagen"
          value={failedCount}
          color={Colors.danger}
          subtitle="Messungen"
        />
      </View>

      <View style={styles.row}>
        <DashboardCard
          title="Nicht synchronisiert"
          value={stats.unsyncedCount}
          color={stats.unsyncedCount > 0 ? Colors.warning : Colors.success}
        />
        <View style={styles.gap} />
        <View style={{ flex: 1 }} />
      </View>

      <Text style={styles.sectionTitle}>Schnellzugriff</Text>

      <TouchableOpacity
        style={styles.quickLink}
        onPress={() => router.push("/(tabs)/projects/")}
      >
        <FontAwesome name="building" size={18} color={Colors.primary} />
        <Text style={styles.quickLinkText}>Alle Projekte</Text>
        <FontAwesome name="chevron-right" size={14} color={Colors.textTertiary} />
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.quickLink}
        onPress={() => router.push("/(tabs)/projects/search")}
      >
        <FontAwesome name="search" size={18} color={Colors.primary} />
        <Text style={styles.quickLinkText}>Installationen suchen</Text>
        <FontAwesome name="chevron-right" size={14} color={Colors.textTertiary} />
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.quickLink}
        onPress={() => router.push("/(tabs)/sync")}
      >
        <FontAwesome name="refresh" size={18} color={Colors.primary} />
        <Text style={styles.quickLinkText}>Synchronisation</Text>
        <FontAwesome name="chevron-right" size={14} color={Colors.textTertiary} />
      </TouchableOpacity>
    </ScrollView>
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
  loading: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: Colors.background,
  },
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    color: Colors.textPrimary,
    marginTop: Spacing.lg,
    marginBottom: Spacing.md,
  },
  row: {
    flexDirection: "row",
    marginBottom: Spacing.md,
  },
  gap: {
    width: Spacing.md,
  },
  quickLink: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    padding: Spacing.lg,
    marginBottom: Spacing.sm,
    gap: Spacing.md,
  },
  quickLinkText: {
    flex: 1,
    fontSize: FontSize.body,
    fontWeight: "500",
    color: Colors.textPrimary,
  },
});
