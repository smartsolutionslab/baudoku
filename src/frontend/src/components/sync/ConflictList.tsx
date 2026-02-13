import React from "react";
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
} from "react-native";
import type { ConflictDto } from "../../sync/syncApi";

type ConflictListProps = {
  conflicts: ConflictDto[];
  onSelect: (conflict: ConflictDto) => void;
};

function ConflictItem({
  item,
  onPress,
}: {
  item: ConflictDto;
  onPress: () => void;
}) {
  return (
    <TouchableOpacity style={styles.item} onPress={onPress}>
      <View style={styles.itemHeader}>
        <View style={styles.typeBadge}>
          <Text style={styles.typeBadgeText}>{item.entityType}</Text>
        </View>
        <View style={styles.statusBadge}>
          <Text style={styles.statusBadgeText}>{item.status}</Text>
        </View>
      </View>
      <Text style={styles.entityId} numberOfLines={1}>
        {item.entityId}
      </Text>
      <Text style={styles.versions}>
        Client v{item.clientVersion} / Server v{item.serverVersion}
      </Text>
    </TouchableOpacity>
  );
}

export function ConflictList({ conflicts, onSelect }: ConflictListProps) {
  if (conflicts.length === 0) {
    return (
      <View style={styles.empty}>
        <Text style={styles.emptyText}>Keine Konflikte vorhanden</Text>
      </View>
    );
  }

  return (
    <FlatList
      data={conflicts}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => (
        <ConflictItem item={item} onPress={() => onSelect(item)} />
      )}
      contentContainerStyle={styles.list}
    />
  );
}

const styles = StyleSheet.create({
  list: {
    padding: 16,
  },
  item: {
    backgroundColor: "#fff",
    borderRadius: 10,
    padding: 12,
    marginBottom: 8,
  },
  itemHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 4,
  },
  typeBadge: {
    backgroundColor: "#007AFF",
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 10,
  },
  typeBadgeText: {
    color: "#fff",
    fontSize: 11,
    fontWeight: "600",
    textTransform: "capitalize",
  },
  statusBadge: {
    backgroundColor: "#FF9500",
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 10,
  },
  statusBadgeText: {
    color: "#fff",
    fontSize: 11,
    fontWeight: "600",
    textTransform: "capitalize",
  },
  entityId: {
    fontSize: 13,
    color: "#8E8E93",
    fontFamily: "SpaceMono",
    marginBottom: 2,
  },
  versions: {
    fontSize: 12,
    color: "#666",
  },
  empty: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    paddingTop: 60,
  },
  emptyText: {
    fontSize: 15,
    color: "#8E8E93",
  },
});
