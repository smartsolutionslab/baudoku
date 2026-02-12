import React, { useState, useMemo } from "react";
import {
  View,
  Text,
  FlatList,
  TextInput,
  TouchableOpacity,
  StyleSheet,
} from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import { FontAwesome } from "@expo/vector-icons";
import { useInstallationsByProject } from "../../../src/hooks/useOfflineData";
import { StatusBadge } from "../../../src/components/common/StatusBadge";
import { EmptyState } from "../../../src/components/common/EmptyState";
import { Colors, Spacing, FontSize, Radius } from "../../../src/styles/tokens";
import type { Installation } from "../../../src/db/repositories/types";

const INSTALLATION_TYPES = [
  { value: "", label: "Alle Typen" },
  { value: "cable_tray", label: "Kabeltrasse" },
  { value: "junction_box", label: "Abzweigdose" },
  { value: "cable_pull", label: "Kabelzug" },
  { value: "conduit", label: "Leerrohr" },
  { value: "grounding", label: "Erdung" },
  { value: "lightning_protection", label: "Blitzschutz" },
  { value: "switchgear", label: "Schaltanlage" },
  { value: "transformer", label: "Trafo" },
  { value: "other", label: "Sonstige" },
];

const STATUS_FILTERS = [
  { value: "", label: "Alle Status" },
  { value: "planned", label: "Geplant" },
  { value: "in_progress", label: "In Arbeit" },
  { value: "completed", label: "Abgeschlossen" },
  { value: "inspected", label: "Geprüft" },
];

export default function InstallationsListScreen() {
  const { projectId } = useLocalSearchParams<{ projectId: string }>();
  const router = useRouter();
  const { data: installations } = useInstallationsByProject(projectId!);

  const [searchQuery, setSearchQuery] = useState("");
  const [typeFilter, setTypeFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const filtered = useMemo(() => {
    if (!installations) return [];
    let result = installations;

    if (typeFilter) {
      result = result.filter((i) => i.type === typeFilter);
    }
    if (statusFilter) {
      result = result.filter((i) => i.status === statusFilter);
    }
    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase();
      result = result.filter(
        (i) =>
          i.notes?.toLowerCase().includes(q) ||
          i.type.toLowerCase().includes(q) ||
          i.manufacturer?.toLowerCase().includes(q) ||
          i.model?.toLowerCase().includes(q) ||
          i.serialNumber?.toLowerCase().includes(q)
      );
    }

    return result;
  }, [installations, typeFilter, statusFilter, searchQuery]);

  const renderItem = ({ item }: { item: Installation }) => (
    <TouchableOpacity
      style={styles.itemCard}
      onPress={() =>
        router.push(`/(tabs)/projects/installation/${item.id}`)
      }
    >
      <View style={styles.itemHeader}>
        <Text style={styles.itemType}>{item.type}</Text>
        <StatusBadge status={item.status} />
      </View>
      {item.manufacturer || item.model ? (
        <Text style={styles.itemSubtitle}>
          {[item.manufacturer, item.model].filter(Boolean).join(" — ")}
        </Text>
      ) : null}
      {item.notes ? (
        <Text style={styles.itemNotes} numberOfLines={2}>
          {item.notes}
        </Text>
      ) : null}
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: "Alle Installationen" }} />

      {/* Search */}
      <View style={styles.searchContainer}>
        <FontAwesome
          name="search"
          size={14}
          color={Colors.textTertiary}
          style={styles.searchIcon}
        />
        <TextInput
          style={styles.searchInput}
          value={searchQuery}
          onChangeText={setSearchQuery}
          placeholder="Suchen..."
          placeholderTextColor={Colors.textTertiary}
        />
        {searchQuery.length > 0 && (
          <TouchableOpacity onPress={() => setSearchQuery("")}>
            <FontAwesome name="close" size={14} color={Colors.textTertiary} />
          </TouchableOpacity>
        )}
      </View>

      {/* Filters */}
      <View style={styles.filtersRow}>
        <FlatList
          horizontal
          showsHorizontalScrollIndicator={false}
          data={INSTALLATION_TYPES}
          keyExtractor={(item) => `type-${item.value}`}
          renderItem={({ item }) => (
            <TouchableOpacity
              style={[
                styles.filterChip,
                typeFilter === item.value && styles.filterChipActive,
              ]}
              onPress={() =>
                setTypeFilter(typeFilter === item.value ? "" : item.value)
              }
            >
              <Text
                style={[
                  styles.filterChipText,
                  typeFilter === item.value && styles.filterChipTextActive,
                ]}
              >
                {item.label}
              </Text>
            </TouchableOpacity>
          )}
        />
      </View>
      <View style={styles.filtersRow}>
        <FlatList
          horizontal
          showsHorizontalScrollIndicator={false}
          data={STATUS_FILTERS}
          keyExtractor={(item) => `status-${item.value}`}
          renderItem={({ item }) => (
            <TouchableOpacity
              style={[
                styles.filterChip,
                statusFilter === item.value && styles.filterChipActive,
              ]}
              onPress={() =>
                setStatusFilter(
                  statusFilter === item.value ? "" : item.value
                )
              }
            >
              <Text
                style={[
                  styles.filterChipText,
                  statusFilter === item.value && styles.filterChipTextActive,
                ]}
              >
                {item.label}
              </Text>
            </TouchableOpacity>
          )}
        />
      </View>

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
  searchContainer: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    borderRadius: Radius.md,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
  },
  searchIcon: {
    marginRight: Spacing.sm,
  },
  searchInput: {
    flex: 1,
    fontSize: FontSize.body,
    color: Colors.textPrimary,
    paddingVertical: Spacing.xs,
  },
  filtersRow: {
    paddingHorizontal: Spacing.lg,
    marginTop: Spacing.sm,
  },
  filterChip: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    borderRadius: 16,
    backgroundColor: Colors.card,
    marginRight: Spacing.xs,
    borderWidth: 1,
    borderColor: Colors.separator,
  },
  filterChipActive: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  filterChipText: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
  },
  filterChipTextActive: {
    color: "#fff",
    fontWeight: "600",
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
