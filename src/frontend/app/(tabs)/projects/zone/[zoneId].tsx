import React, { useMemo, useState, useCallback } from "react";
import { View, Text, FlatList, StyleSheet } from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import {
  useZonesByProject,
  useInstallationsByZone,
  useDeleteZone,
  useUpdateZone,
} from "../../../../src/hooks/useOfflineData";
import { useConfirmDelete } from "../../../../src/hooks/useConfirmDelete";
import { InstallationCard } from "../../../../src/components/installations/InstallationCard";
import { StatusBadge } from "../../../../src/components/common/StatusBadge";
import { EmptyState } from "../../../../src/components/common/EmptyState";
import { FloatingActionButton } from "../../../../src/components/common/FloatingActionButton";
import { ActionBar } from "../../../../src/components/common/ActionBar";
import { ZoneQrSheet } from "../../../../src/components/projects/ZoneQrSheet";
import { encodeZoneQr } from "../../../../src/utils/qrCode";
import { Colors, Spacing, FontSize, Radius } from "../../../../src/styles/tokens";
import type { Zone } from "../../../../src/db/repositories/types";
import {
  projectId as toProjectId,
  zoneId as toZoneId,
} from "../../../../src/types/branded";
import type { ZoneId } from "../../../../src/types/branded";

function buildBreadcrumb(zones: Zone[], id: ZoneId): string[] {
  const byId = new Map(zones.map((z) => [z.id as string, z]));
  const parts: string[] = [];
  let current = byId.get(id);
  while (current) {
    parts.unshift(current.name);
    current = current.parentZoneId ? byId.get(current.parentZoneId) : undefined;
  }
  return parts;
}

export default function ZoneDetailScreen() {
  const { zoneId: rawZoneId, projectId: rawProjectId } = useLocalSearchParams<{
    zoneId: string;
    projectId: string;
  }>();
  const zoneId = toZoneId(rawZoneId!);
  const projectId = toProjectId(rawProjectId!);
  const router = useRouter();
  const { data: zones } = useZonesByProject(projectId);
  const { data: installations, isLoading, refetch } =
    useInstallationsByZone(zoneId);
  const deleteZone = useDeleteZone();
  const updateZone = useUpdateZone();
  const { confirmDelete } = useConfirmDelete();
  const [qrSheetVisible, setQrSheetVisible] = useState(false);

  const zone = useMemo(
    () => zones?.find((z) => z.id === zoneId),
    [zones, zoneId]
  );

  const breadcrumb = useMemo(
    () => (zones && zoneId ? buildBreadcrumb(zones, zoneId) : []),
    [zones, zoneId]
  );

  const qrValue = useMemo(
    () => (projectId && zoneId ? encodeZoneQr(projectId, zoneId) : ""),
    [projectId, zoneId]
  );

  const handleQrPress = useCallback(async () => {
    try {
      if (zone && !zone.qrCode) {
        await updateZone.mutateAsync({
          id: zoneId,
          data: { qrCode: qrValue },
        });
      }
      setQrSheetVisible(true);
    } catch {
      // Global MutationCache.onError shows toast
    }
  }, [zone, zoneId, qrValue, updateZone]);

  const handleDelete = () => {
    confirmDelete({
      title: "Zone löschen",
      message: "Diese Zone und alle zugehörigen Daten wirklich löschen?",
      onConfirm: async () => {
        try {
          await deleteZone.mutateAsync(zoneId);
          router.back();
        } catch {
          // Global MutationCache.onError shows toast
        }
      },
    });
  };

  if (!zone) return null;

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
      </View>

      <ActionBar
        actions={[
          { icon: "qrcode", label: "QR-Code", onPress: handleQrPress },
          {
            icon: "pencil",
            label: "Bearbeiten",
            onPress: () =>
              router.push(
                `/(tabs)/projects/zone/edit?zoneId=${zoneId}&projectId=${projectId}`
              ),
          },
          {
            icon: "trash-o",
            label: "Löschen",
            onPress: handleDelete,
            color: Colors.danger,
          },
        ]}
      />

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

      <ZoneQrSheet
        visible={qrSheetVisible}
        onClose={() => setQrSheetVisible(false)}
        qrValue={qrValue}
        zoneName={zone.name}
        zoneType={zone.type}
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
    marginBottom: 0,
    borderRadius: Radius.lg,
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
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  list: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 80,
  },
});
