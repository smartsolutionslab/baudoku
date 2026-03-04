import { useState, useMemo } from "react";
import { View, FlatList, TouchableOpacity, StyleSheet } from "react-native";
import { useRouter, Stack } from "expo-router";
import { FontAwesome } from "@expo/vector-icons";
import { useProjects } from "@/hooks";
import { ProjectCard } from "@/components/projects";
import { EmptyState, FloatingActionButton, SearchBar } from "@/components/common";
import { Colors, Spacing } from "@/styles/tokens";

export default function ProjectsScreen() {
  const router = useRouter();
  const { data: projects, isLoading, refetch } = useProjects();
  const [search, setSearch] = useState("");

  const openSearch = () => router.push("/(tabs)/projects/search");
  const openDashboard = () => router.push("/(tabs)/projects/dashboard");
  const openNewProject = () => router.push("/(tabs)/projects/new");
  const openProject = (id: string) => router.push(`/(tabs)/projects/${id}`);

  const filtered = useMemo(() => {
    if (!projects) return [];
    if (!search.trim()) return projects;
    const q = search.toLowerCase();
    return projects.filter(
      ({ name, city, clientName }) =>
        name.toLowerCase().includes(q) ||
        city?.toLowerCase().includes(q) ||
        clientName?.toLowerCase().includes(q)
    );
  }, [projects, search]);

  return (
    <View style={styles.container}>
      <Stack.Screen
        options={{
          headerRight: () => (
            <View style={styles.headerActions}>
              <TouchableOpacity onPress={openSearch} >
                <FontAwesome name="search" size={20} color={Colors.primary} />
              </TouchableOpacity>
              <TouchableOpacity onPress={openDashboard} >
                <FontAwesome name="bar-chart" size={20} color={Colors.primary} />
              </TouchableOpacity>
            </View>
          ),
        }}
      />
      <SearchBar value={search} onChangeText={setSearch} placeholder="Projekte suchen..." autoFocus={false} />
      {filtered.length === 0 && !isLoading ? (
        <EmptyState
          icon="building"
          title="Noch keine Projekte"
          subtitle="Erstelle dein erstes Projekt, um loszulegen."
          actionLabel="Projekt anlegen"
          onAction={openNewProject}
        />
      ) : (
        <FlatList
          data={filtered}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <ProjectCard project={item} onPress={() => openProject(item.id)} />
          )}
          contentContainerStyle={styles.list}
          refreshing={isLoading}
          onRefresh={() => void refetch()}
        />
      )}
      <FloatingActionButton onPress={openNewProject} testID="fab-button" />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  headerActions: {
    flexDirection: "row",
    gap: Spacing.lg,
    marginRight: Spacing.sm,
  },
  list: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 80,
  },
});
