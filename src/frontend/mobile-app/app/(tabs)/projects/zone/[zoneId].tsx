import { useMemo, useCallback } from 'react';
import { View, Text, FlatList, StyleSheet } from 'react-native';
import { useLocalSearchParams, useRouter, Stack } from 'expo-router';
import { useZonesByProject, useInstallationsByZone, useDeleteZone, useUpdateZone, useConfirmDelete, useToggle } from '@/hooks';
import { InstallationCard } from '@/components/installations';
import { StatusBadge, EmptyState, FloatingActionButton, ActionBar } from '@/components/common';
import { ZoneQrSheet } from '@/components/projects';
import { encodeZoneQr, requiredParam } from '@/utils';
import { Colors, Spacing, FontSize, Radius } from '@/styles/tokens';
import type { Zone } from '@/db/repositories/types';
import { projectId as toProjectId, zoneId as toZoneId } from '@baudoku/core';
import type { ZoneId } from '@baudoku/core';

function buildBreadcrumb(zones: Zone[], id: ZoneId): string[] {
  const byId = new Map(zones.map((z) => [z.id as string, z] as const));
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
  const zoneId = toZoneId(requiredParam(rawZoneId));
  const projectId = toProjectId(requiredParam(rawProjectId));
  const router = useRouter();
  const { data: zones } = useZonesByProject(projectId);
  const { data: installations, isLoading, refetch } = useInstallationsByZone(zoneId);
  const deleteZone = useDeleteZone();
  const updateZone = useUpdateZone();
  const { confirmDelete } = useConfirmDelete();
  const { value: qrSheetVisible, open: openQrSheet, close: closeQrSheet } = useToggle();

  const openEditZone = () =>
    router.push(`/(tabs)/projects/zone/edit?zoneId=${zoneId}&projectId=${projectId}`);
  const openNewInstallation = () =>
    router.push(`/(tabs)/capture/new?projectId=${projectId}&zoneId=${zoneId}`);
  const openInstallation = (id: string) => router.push(`/(tabs)/projects/installation/${id}`);

  const zone = useMemo(() => zones?.find((z) => z.id === zoneId), [zones, zoneId]);

  const breadcrumb = useMemo(
    () => (zones && zoneId ? buildBreadcrumb(zones, zoneId) : []),
    [zones, zoneId],
  );

  const qrValue = useMemo(
    () => (projectId && zoneId ? encodeZoneQr(projectId, zoneId) : ''),
    [projectId, zoneId],
  );

  const handleQrPress = useCallback(async () => {
    try {
      if (zone && !zone.qrCode) {
        await updateZone.mutateAsync({ id: zoneId, data: { qrCode: qrValue } });
      }
      openQrSheet();
    } catch {
      // Global MutationCache.onError shows toast
    }
  }, [zone, zoneId, qrValue, updateZone, openQrSheet]);

  const handleDelete = () => {
    confirmDelete({
      title: 'Zone löschen',
      message: 'Diese Zone und alle zugehörigen Daten wirklich löschen?',
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

  const { name, type } = zone;

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: name }} />

      <View style={styles.header}>
        <View style={styles.headerRow}>
          <Text style={styles.title}>{name}</Text>
          <StatusBadge status={type} />
        </View>
        {breadcrumb.length > 1 && (
          <Text style={styles.breadcrumb} numberOfLines={1}>
            {breadcrumb.join(' > ')}
          </Text>
        )}
      </View>

      <ActionBar
        actions={[
          { icon: 'qrcode', label: 'QR-Code', onPress: handleQrPress },
          { icon: 'pencil', label: 'Bearbeiten', onPress: openEditZone },
          { icon: 'trash-o', label: 'Löschen', onPress: handleDelete, color: Colors.danger },
        ]}
      />

      <Text style={styles.sectionTitle}>Installationen</Text>

      {!installations || installations.length === 0 ? (
        <EmptyState
          icon="wrench"
          title="Noch keine Installationen"
          subtitle="Erfasse die erste Installation in dieser Zone."
          actionLabel="Installation erfassen"
          onAction={openNewInstallation}
        />
      ) : (
        <FlatList
          data={installations}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <InstallationCard installation={item} onPress={() => openInstallation(item.id)} />
          )}
          contentContainerStyle={styles.list}
          refreshing={isLoading}
          onRefresh={() => void refetch()}
        />
      )}

      <FloatingActionButton onPress={openNewInstallation} />

      <ZoneQrSheet
        visible={qrSheetVisible}
        onClose={closeQrSheet}
        qrValue={qrValue}
        zoneName={name}
        zoneType={type}
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
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  title: {
    fontSize: FontSize.headline,
    fontWeight: '600',
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
    fontWeight: '600',
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  list: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 80,
  },
});
