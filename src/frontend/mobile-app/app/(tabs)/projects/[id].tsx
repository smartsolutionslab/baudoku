import { View, Text, ScrollView, StyleSheet } from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import {
  useProject,
  useZonesByProject,
  useDeleteProject,
  useZoneTree,
  useConfirmDelete,
} from "../../../src/hooks";
import { ZoneTree } from "../../../src/components/projects";
import { StatusBadge, EmptyState, FloatingActionButton, ActionBar } from "../../../src/components/common";
import { Colors, Spacing, FontSize, Radius } from "../../../src/styles/tokens";
import { formatDate } from "../../../src/utils";
import { projectId } from "../../../src/types/branded";

export default function ProjectDetailScreen() {
  const { id: rawId } = useLocalSearchParams<{ id: string }>();
  const id = projectId(rawId!);
  const router = useRouter();
  const { data: project } = useProject(id);
  const { data: zones } = useZonesByProject(id);
  const tree = useZoneTree(zones);
  const deleteProject = useDeleteProject();
  const { confirmDelete } = useConfirmDelete();

  if (!project) return null;

  const address = [project.street, project.zipCode, project.city]
    .filter(Boolean)
    .join(", ");

  const handleDelete = () => {
    confirmDelete({
      title: "Projekt löschen",
      message:
        "Dieses Projekt und alle zugehörigen Daten wirklich löschen?",
      onConfirm: async () => {
        try {
          await deleteProject.mutateAsync(id);
          router.replace("/(tabs)/projects/");
        } catch {
          // Global MutationCache.onError shows toast
        }
      },
    });
  };

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: project.name }} />
      <ScrollView contentContainerStyle={styles.scroll}>
        <View style={styles.card}>
          <View style={styles.row}>
            <Text style={styles.label}>Status</Text>
            <StatusBadge status={project.status} />
          </View>
          {address ? (
            <View style={styles.row}>
              <Text style={styles.label}>Adresse</Text>
              <Text style={styles.value}>{address}</Text>
            </View>
          ) : null}
          {project.clientName ? (
            <View style={styles.row}>
              <Text style={styles.label}>Auftraggeber</Text>
              <Text style={styles.value}>{project.clientName}</Text>
            </View>
          ) : null}
          {project.clientContact ? (
            <View style={styles.row}>
              <Text style={styles.label}>Kontakt</Text>
              <Text style={styles.value}>{project.clientContact}</Text>
            </View>
          ) : null}
          <View style={styles.row}>
            <Text style={styles.label}>Erstellt am</Text>
            <Text style={styles.value}>{formatDate(project.createdAt)}</Text>
          </View>
        </View>

        <ActionBar
          actions={[
            {
              icon: "list",
              label: "Installationen",
              onPress: () =>
                router.push(
                  `/(tabs)/projects/installations?projectId=${id}`
                ),
            },
            {
              icon: "pencil",
              label: "Bearbeiten",
              onPress: () =>
                router.push(`/(tabs)/projects/edit?id=${id}`),
            },
            {
              icon: "trash",
              label: "Löschen",
              onPress: handleDelete,
              color: Colors.danger,
            },
          ]}
        />

        <Text style={styles.sectionTitle}>Zonen</Text>

        {tree.length === 0 ? (
          <EmptyState
            icon="sitemap"
            title="Noch keine Zonen"
            subtitle="Erstelle Gebäude, Stockwerke oder Gräben."
            actionLabel="Neue Zone"
            onAction={() =>
              router.push(`/(tabs)/projects/zone/new?projectId=${id}`)
            }
          />
        ) : (
          <View style={styles.treeContainer}>
            <ZoneTree
              nodes={tree}
              onZonePress={(zoneId) =>
                router.push(
                  `/(tabs)/projects/zone/${zoneId}?projectId=${id}`
                )
              }
            />
          </View>
        )}
      </ScrollView>

      <FloatingActionButton
        onPress={() =>
          router.push(`/(tabs)/projects/zone/new?projectId=${id}`)
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
  scroll: {
    paddingBottom: 80,
  },
  card: {
    backgroundColor: Colors.card,
    margin: Spacing.lg,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    gap: Spacing.md,
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
    flex: 1,
    textAlign: "right",
    marginLeft: Spacing.lg,
  },
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  treeContainer: {
    paddingHorizontal: Spacing.lg,
  },
});
