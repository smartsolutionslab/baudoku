import { useState, useMemo } from "react";
import { View, Text, FlatList, TouchableOpacity, StyleSheet } from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import { useInstallationsByProject } from "@/hooks";
import { StatusBadge, EmptyState, SearchBar, FilterChips } from "@/components/common";
import { Colors, Spacing, FontSize, Radius } from "@/styles/tokens";
import { INSTALLATION_TYPE_OPTIONS, INSTALLATION_STATUS_OPTIONS } from "@/constants";
import type { Installation } from "@/db/repositories/types";
import { projectId as toProjectId } from "@baudoku/core";

export default function InstallationsListScreen() {
  const { projectId: rawProjectId } = useLocalSearchParams<{ projectId: string }>();
  const projectId = toProjectId(rawProjectId!);
  const router = useRouter();
  const { data: installations } = useInstallationsByProject(projectId);

  const openInstallation = (id: string) => router.push(`/(tabs)/projects/installation/${id}`);

  const [searchQuery, setSearchQuery] = useState("");
  const [typeFilter, setTypeFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const filtered = useMemo(() => {
    if (!installations) return [];
    let result = installations;

    if (typeFilter) {
      result = result.filter(({ type }) => type === typeFilter);
    }
    if (statusFilter) {
      result = result.filter(({ status }) => status === statusFilter);
    }
    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase();
      result = result.filter(
        ({ notes, type, manufacturer, model, serialNumber }) =>
          notes?.toLowerCase().includes(q) ||
          type.toLowerCase().includes(q) ||
          manufacturer?.toLowerCase().includes(q) ||
          model?.toLowerCase().includes(q) ||
          serialNumber?.toLowerCase().includes(q)
      );
    }

    return result;
  }, [installations, typeFilter, statusFilter, searchQuery]);

  const renderItem = ({ item: { id, type, status, manufacturer, model, notes } }: { item: Installation }) => (
    <TouchableOpacity style={styles.itemCard} onPress={() => openInstallation(id)}>
      <View style={styles.itemHeader}>
        <Text style={styles.itemType}>{type}</Text>
        <StatusBadge status={status} />
      </View>
      {manufacturer || model ? (
        <Text style={styles.itemSubtitle}>
          {[manufacturer, model].filter(Boolean).join(" — ")}
        </Text>
      ) : null}
      {notes ? (
        <Text style={styles.itemNotes} numberOfLines={2}>
          {notes}
        </Text>
      ) : null}
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: "Alle Installationen" }} />

      {/* Search */}
      <SearchBar value={searchQuery} onChangeText={setSearchQuery} autoFocus={false} />

      {/* Filters */}
      <FilterChips
        options={INSTALLATION_TYPE_OPTIONS}
        selected={typeFilter ? [typeFilter] : []}
        onToggle={(v) => setTypeFilter(typeFilter === v ? "" : v)}
      />
      <FilterChips
        options={INSTALLATION_STATUS_OPTIONS}
        selected={statusFilter ? [statusFilter] : []}
        onToggle={(v) => setStatusFilter(statusFilter === v ? "" : v)}
      />

      {/* Results count */}
      <Text style={styles.resultCount}>
        {filtered.length} {filtered.length === 1 ? "Installation" : "Installationen"}
      </Text>

      {/* List */}
      {filtered.length === 0 ? (
        <EmptyState
          icon="plug"
          title="Keine Installationen"
          subtitle={
            searchQuery || typeFilter || statusFilter
              ? "Versuche andere Filterkriterien."
              : "In diesem Projekt gibt es noch keine Installationen."
          }
        />
      ) : (
        <FlatList 
          data={filtered}
          keyExtractor={(item) => item.id}
          renderItem={renderItem}
          contentContainerStyle={styles.listContent}
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
  resultCount: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.sm,
    marginBottom: Spacing.xs,
  },
  listContent: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 40,
  },
  itemCard: {
    backgroundColor: Colors.card,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  itemHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: Spacing.xs,
  },
  itemType: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textPrimary,
    flex: 1,
    marginRight: Spacing.sm,
  },
  itemSubtitle: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    marginBottom: Spacing.xs,
  },
  itemNotes: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
  },
});
