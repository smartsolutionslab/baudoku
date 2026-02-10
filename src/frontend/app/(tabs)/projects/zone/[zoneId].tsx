import React, { useMemo } from "react";
import { View, Text, FlatList, TouchableOpacity, StyleSheet } from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import { FontAwesome } from "@expo/vector-icons";
import {
  useZonesByProject,
  useInstallationsByZone,
  useDeleteZone,
} from "../../../../src/hooks/useOfflineData";
import { useConfirmDelete } from "../../../../src/hooks/useConfirmDelete";
import { InstallationCard } from "../../../../src/components/installations/InstallationCard";
import { StatusBadge } from "../../../../src/components/common/StatusBadge";
import { EmptyState } from "../../../../src/components/common/EmptyState";
import { FloatingActionButton } from "../../../../src/components/common/FloatingActionButton";
import { Colors, Spacing, FontSize } from "../../../../src/styles/tokens";
import type { Zone } from "../../../../src/db/repositories/types";

function buildBreadcrumb(zones: Zone[], zoneId: string): string[] {
  const byId = new Map(zones.map((z) => [z.id, z]));
  const parts: string[] = [];
  let current = byId.get(zoneId);
  while (current) {
    parts.unshift(current.name);
    current = current.parentZoneId ? byId.get(current.parentZoneId) : undefined;
  }
  return parts;
}

export default function ZoneDetailScreen() {
  const { zoneId, projectId } = useLocalSearchParams<{
    zoneId: string;
    projectId: string;
  }>();
  const router = useRouter();
  const { data: zones } = useZonesByProject(projectId!);
  const { data: installations, isLoading, refetch } =
    useInstallationsByZone(zoneId!);
  const deleteZone = useDeleteZone();
  const { confirmDelete } = useConfirmDelete();

  const zone = useMemo(
    () => zones?.find((z) => z.id === zoneId),
    [zones, zoneId]
  );

  const breadcrumb = useMemo(
    () => (zones && zoneId ? buildBreadcrumb(zones, zoneId) : []),
    [zones, zoneId]
  );

  if (!zone) return null;

  const handleDelete = () => {
    confirmDelete({
      title: "Zone löschen",
      message: "Diese Zone und alle zugehörigen Daten wirklich löschen?",
      onConfirm: async () => {
        await deleteZone.mutateAsync(zoneId!);
        router.back();
      },
    });
  };

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: zone.name }} />

      <View style={styles.header}>
        <View style={styles.headerRow}>
          <Text style={styles.title}>{zone.name}</Text>
          <StatusBadge status={zone.type} />
        </View>
        {breadcrumb.length > 1 && (
          <Text style={styles.breadcrumb} numberOfLines={1}>
            {breadcrumb.join(" > ")}
          </Text>
        )}
        <TouchableOpacity style={styles.deleteRow} onPress={handleDelete}>
          <FontAwesome name="trash-o" size={14} color={Colors.danger} />
          <Text style={styles.deleteText}>Zone löschen</Text>
        </TouchableOpacity>
      </View>

      <Text style={styles.sectionTitle}>Installationen</Text>

      {!installations || installations.length === 0 ? (
        <EmptyState
          icon="wrench"
          title="Noch keine Installationen"
          subtitle="Erfasse die erste Installation in dieser Zone."
          actionLabel="Installation erfassen"
          onAction={() =>
            router.push(
              `/(tabs)/capture/new?projectId=${projectId}&zoneId=${zoneId}`
            )
          }
        />
      ) : (
        <FlatList
          data={installations}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <InstallationCard
              installation={item}
              onPress={() =>
                router.push(`/(tabs)/projects/installation/${item.id}`)
              }
            />
          )}
          contentContainerStyle={styles.list}
          refreshing={isLoading}
          onRefresh={() => void refetch()}
        />
      )}

      <FloatingActionButton
        onPress={() =>
          router.push(
            `/(tabs)/capture/new?projectId=${projectId}&zoneId=${zoneId}`
          )
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    backgroundColor: Colors.card,
    margin: Spacing.lg,
    borderRadius: 12,
    padding: Spacing.lg,
  },
  headerRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  title: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    color: Colors.textPrimary,
    flex: 1,
    marginRight: Spacing.sm,
  },
  breadcrumb: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    marginTop: Spacing.sm,
  },
  deleteRow: {
    flexDirection: "row",
    alignItems: "center",
    marginTop: Spacing.md,
    paddingTop: Spacing.sm,
    borderTopWidth: StyleSheet.hairlineWidth,
    borderTopColor: Colors.separator,
  },
  deleteText: {
    fontSize: FontSize.caption,
    color: Colors.danger,
    marginLeft: Spacing.xs,
  },
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginHorizontal: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  list: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 80,
  },
});
