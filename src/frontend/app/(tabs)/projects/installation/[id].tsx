import React from "react";
import { View, Text, ScrollView, FlatList, StyleSheet } from "react-native";
import { useLocalSearchParams, Stack } from "expo-router";
import {
  usePhotosByInstallation,
  useMeasurementsByInstallation,
} from "../../../../src/hooks/useOfflineData";
import { StatusBadge } from "../../../../src/components/common/StatusBadge";
import { EmptyState } from "../../../../src/components/common/EmptyState";
import { InstallationDetails } from "../../../../src/components/installations/InstallationDetails";
import { PhotoGallery } from "../../../../src/components/installations/PhotoGallery";
import { MeasurementCard } from "../../../../src/components/installations/MeasurementCard";
import { Colors, Spacing, FontSize } from "../../../../src/styles/tokens";
import * as installationRepo from "../../../../src/db/repositories/installationRepo";
import { useQuery } from "@tanstack/react-query";

export default function InstallationDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const { data: installation } = useQuery({
    queryKey: ["installation", id],
    queryFn: () => installationRepo.getById(id!),
    enabled: !!id,
  });
  const { data: photos } = usePhotosByInstallation(id!);
  const { data: measurements } = useMeasurementsByInstallation(id!);

  if (!installation) return null;

  const subtitle = [installation.manufacturer, installation.model]
    .filter(Boolean)
    .join(" — ");

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: installation.type }} />
      <ScrollView contentContainerStyle={styles.scroll}>
        {/* Basis-Info */}
        <View style={styles.card}>
          <View style={styles.headerRow}>
            <Text style={styles.type}>{installation.type}</Text>
            <StatusBadge status={installation.status} />
          </View>
          {subtitle ? (
            <Text style={styles.subtitle}>{subtitle}</Text>
          ) : null}
          {installation.serialNumber ? (
            <Text style={styles.detail}>
              SN: {installation.serialNumber}
            </Text>
          ) : null}
          {installation.notes ? (
            <Text style={styles.notes}>{installation.notes}</Text>
          ) : null}
        </View>

        {/* Technische Details */}
        <InstallationDetails installation={installation} />

        {/* Fotos */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Fotos</Text>
          <PhotoGallery photos={photos ?? []} />
        </View>

        {/* Messungen */}
        <View style={styles.measurementsSection}>
          <Text style={styles.cardTitle}>Messungen</Text>
          {!measurements || measurements.length === 0 ? (
            <EmptyState icon="bar-chart" title="Noch keine Messungen" />
          ) : (
            measurements.map((m) => (
              <MeasurementCard key={m.id} measurement={m} />
            ))
          )}
        </View>
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  scroll: {
    paddingBottom: 40,
  },
  card: {
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
    borderRadius: 12,
    padding: Spacing.lg,
  },
  headerRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: Spacing.xs,
  },
  type: {
    fontSize: FontSize.title,
    fontWeight: "700",
    color: Colors.textPrimary,
    flex: 1,
    marginRight: Spacing.sm,
  },
  subtitle: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
    marginBottom: Spacing.xs,
  },
  detail: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  notes: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
    marginTop: Spacing.sm,
  },
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginBottom: Spacing.md,
  },
  measurementsSection: {
    paddingHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
  },
});
