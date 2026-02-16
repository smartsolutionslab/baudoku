import React, { useState, useMemo } from "react";
import { View, TextInput, FlatList, TouchableOpacity, StyleSheet } from "react-native";
import { useRouter, Stack } from "expo-router";
import { FontAwesome } from "@expo/vector-icons";
import { useProjects } from "../../../src/hooks/useOfflineData";
import { ProjectCard } from "../../../src/components/projects/ProjectCard";
import { EmptyState } from "../../../src/components/common/EmptyState";
import { FloatingActionButton } from "../../../src/components/common/FloatingActionButton";
import { Colors, Spacing } from "../../../src/styles/tokens";

export default function ProjectsScreen() {
  const router = useRouter();
  const { data: projects, isLoading, refetch } = useProjects();
  const [search, setSearch] = useState("");

  const filtered = useMemo(() => {
    if (!projects) return [];
    if (!search.trim()) return projects;
    const q = search.toLowerCase();
    return projects.filter(
      (p) =>
        p.name.toLowerCase().includes(q) ||
        p.city?.toLowerCase().includes(q) ||
        p.clientName?.toLowerCase().includes(q)
    );
  }, [projects, search]);

  return (
    <View style={styles.container}>
      <Stack.Screen
        options={{
          headerRight: () => (
            <View style={{ flexDirection: "row", gap: 16, marginRight: 8 }}>
              <TouchableOpacity
                onPress={() => router.push("/(tabs)/projects/search")}
              >
                <FontAwesome name="search" size={20} color={Colors.primary} />
              </TouchableOpacity>
              <TouchableOpacity
                onPress={() => router.push("/(tabs)/projects/dashboard")}
              >
                <FontAwesome name="bar-chart" size={20} color={Colors.primary} />
              </TouchableOpacity>
            </View>
          ),
        }}
      />
      <TextInput
        style={styles.search}
        placeholder="Projekte suchen..."
        placeholderTextColor={Colors.textTertiary}
        value={search}
        onChangeText={setSearch}
        clearButtonMode="while-editing"
      />
      {filtered.length === 0 && !isLoading ? (
        <EmptyState
          icon="building"
          title="Noch keine Projekte"
          subtitle="Erstelle dein erstes Projekt, um loszulegen."
          actionLabel="Projekt anlegen"
          onAction={() => router.push("/(tabs)/projects/new")}
        />
      ) : (
        <FlatList
          data={filtered}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <ProjectCard
              project={item}
              onPress={() => router.push(`/(tabs)/projects/${item.id}`)}
            />
          )}
          contentContainerStyle={styles.list}
          refreshing={isLoading}
          onRefresh={() => void refetch()}
        />
      )}
      <FloatingActionButton
        onPress={() => router.push("/(tabs)/projects/new")}
        testID="fab-button"
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  search: {
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
    borderRadius: 10,
    paddingHorizontal: Spacing.md,
    paddingVertical: 10,
    fontSize: 15,
    color: Colors.textPrimary,
  },
  list: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 80,
  },
});
