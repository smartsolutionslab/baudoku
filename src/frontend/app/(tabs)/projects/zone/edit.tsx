import React, { useMemo } from "react";
import { View, Text, ActivityIndicator, StyleSheet } from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import {
  useZonesByProject,
  useUpdateZone,
} from "../../../../src/hooks/useOfflineData";
import { ZoneForm } from "../../../../src/components/projects/ZoneForm";
import { Colors, Spacing, FontSize } from "../../../../src/styles/tokens";
import type { ZoneFormData } from "../../../../src/validation/schemas";
import {
  projectId as toProjectId,
  zoneId as toZoneId,
} from "../../../../src/types/branded";

export default function ZoneEditScreen() {
  const { zoneId: rawZoneId, projectId: rawProjectId } = useLocalSearchParams<{
    zoneId: string;
    projectId: string;
  }>();
  const zoneId = toZoneId(rawZoneId!);
  const projectId = toProjectId(rawProjectId!);
  const router = useRouter();
  const { data: zones, isLoading } = useZonesByProject(projectId);
  const updateZone = useUpdateZone();

  const zone = useMemo(
    () => zones?.find((z) => z.id === zoneId),
    [zones, zoneId]
  );

  const otherZones = useMemo(
    () => zones?.filter((z) => z.id !== zoneId) ?? [],
    [zones, zoneId]
  );

  const handleSubmit = async (data: ZoneFormData) => {
    await updateZone.mutateAsync({
      id: zoneId,
      data: {
        name: data.name,
        type: data.type,
        parentZoneId: data.parentZoneId ?? null,
        sortOrder: data.sortOrder ?? 0,
      },
    });
    router.back();
  };

  if (isLoading || !zone) {
    return (
      <View style={styles.loading}>
        <Stack.Screen options={{ title: "Zone bearbeiten" }} />
        <ActivityIndicator size="large" color={Colors.primary} />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: `${zone.name} bearbeiten` }} />
      <ZoneForm
        zones={otherZones}
        initialValues={{
          name: zone.name,
          type: zone.type as ZoneFormData["type"],
          parentZoneId: zone.parentZoneId,
          sortOrder: zone.sortOrder ?? undefined,
        }}
        submitLabel="Aktualisieren"
        onSubmit={handleSubmit}
        submitting={updateZone.isPending}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  loading: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: Colors.background,
  },
});
