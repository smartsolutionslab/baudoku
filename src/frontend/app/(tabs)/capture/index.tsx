import React, { useState } from "react";
import { View, Text, FlatList, TouchableOpacity, StyleSheet } from "react-native";
import { useRouter } from "expo-router";
import {
  useProjects,
  useZonesByProject,
} from "../../../src/hooks/useOfflineData";
import { ProjectCard } from "../../../src/components/projects/ProjectCard";
import { EmptyState } from "../../../src/components/common/EmptyState";
import { Colors, Spacing, FontSize } from "../../../src/styles/tokens";

function ZonePicker({
  projectId,
  onSelect,
}: {
  projectId: string;
  onSelect: (zoneId: string) => void;
}) {
  const { data: zones } = useZonesByProject(projectId);

  if (!zones || zones.length === 0) {
    return (
      <View style={styles.zoneEmpty}>
        <Text style={styles.zoneEmptyText}>
          Keine Zonen vorhanden. Erstelle zuerst Zonen im Projekt.
        </Text>
      </View>
    );
  }

  return (
    <View style={styles.zoneList}>
      <Text style={styles.zoneTitle}>Zone wählen:</Text>
      {zones.map((zone) => (
        <TouchableOpacity
          key={zone.id}
          style={styles.zoneItem}
          onPress={() => onSelect(zone.id)}
        >
          <Text style={styles.zoneName}>{zone.name}</Text>
          <Text style={styles.zoneType}>{zone.type}</Text>
        </TouchableOpacity>
      ))}
    </View>
  );
}

export default function CaptureScreen() {
  const router = useRouter();
  const { data: allProjects } = useProjects();
  const [selectedProjectId, setSelectedProjectId] = useState<string | null>(
    null
  );

  const projects = allProjects?.filter((p) => p.status === "active") ?? [];

  const handleZoneSelect = (zoneId: string) => {
    router.push(
      `/(tabs)/capture/new?projectId=${selectedProjectId}&zoneId=${zoneId}`
    );
  };

  return (
    <View style={styles.container}>
      <Text style={styles.header}>Installation erfassen</Text>
      <Text style={styles.subtitle}>
        Wähle ein aktives Projekt und eine Zone.
      </Text>

      {projects.length === 0 ? (
        <EmptyState
          icon="building"
          title="Keine aktiven Projekte"
          subtitle="Erstelle zuerst ein Projekt im Projekte-Tab."
        />
      ) : (
        <FlatList
          data={projects}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <View>
              <ProjectCard
                project={item}
                onPress={() =>
                  setSelectedProjectId(
                    selectedProjectId === item.id ? null : item.id
                  )
                }
              />
              {selectedProjectId === item.id && (
                <ZonePicker
                  projectId={item.id}
                  onSelect={handleZoneSelect}
                />
              )}
            </View>
          )}
          contentContainerStyle={styles.list}
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    fontSize: FontSize.title,
    fontWeight: "700",
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
  },
  subtitle: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.xs,
    marginBottom: Spacing.lg,
  },
  list: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 40,
  },
  zoneList: {
    marginLeft: Spacing.xl,
    marginBottom: Spacing.md,
  },
  zoneTitle: {
    fontSize: FontSize.caption,
    fontWeight: "600",
    color: Colors.textSecondary,
    marginBottom: Spacing.sm,
  },
  zoneItem: {
    backgroundColor: Colors.card,
    borderRadius: 10,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  zoneName: {
    fontSize: FontSize.body,
    fontWeight: "500",
    color: Colors.textPrimary,
  },
  zoneType: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    textTransform: "capitalize",
  },
  zoneEmpty: {
    marginLeft: Spacing.xl,
    marginBottom: Spacing.md,
    padding: Spacing.md,
  },
  zoneEmptyText: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    fontStyle: "italic",
  },
});
