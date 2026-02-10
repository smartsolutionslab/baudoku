import React, { useEffect } from "react";
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  ActivityIndicator,
} from "react-native";
import { useRouter } from "expo-router";
import { useSyncStore } from "../../../src/store/useSyncStore";
import { useSyncStatus } from "../../../src/hooks/useSyncStatus";
import { useSyncManager } from "../../../src/hooks/useSyncManager";
import type { SyncOutboxEntry } from "../../../src/db/repositories/types";

function formatTimestamp(date: Date): string {
  return date.toLocaleString("de-DE", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function operationLabel(op: string): string {
  switch (op) {
    case "create":
      return "Erstellt";
    case "update":
      return "Aktualisiert";
    case "delete":
      return "Gelöscht";
    default:
      return op;
  }
}

function statusColor(status: string): string {
  switch (status) {
    case "pending":
      return "#FF9500";
    case "syncing":
      return "#007AFF";
    case "failed":
      return "#FF3B30";
    default:
      return "#8E8E93";
  }
}

function OutboxItem({ item }: { item: SyncOutboxEntry }) {
  return (
    <View style={itemStyles.container}>
      <View style={itemStyles.header}>
        <Text style={itemStyles.entityType}>{item.entityType}</Text>
        <View
          style={[
            itemStyles.statusBadge,
            { backgroundColor: statusColor(item.status) },
          ]}
        >
          <Text style={itemStyles.statusText}>{item.status}</Text>
        </View>
      </View>
      <Text style={itemStyles.operation}>
        {operationLabel(item.operation)}
      </Text>
      <Text style={itemStyles.timestamp}>
        {formatTimestamp(item.timestamp)}
      </Text>
      {item.retryCount != null && item.retryCount > 0 && (
        <Text style={itemStyles.retry}>
          Versuche: {item.retryCount}
        </Text>
      )}
    </View>
  );
}

export default function SyncScreen() {
  const router = useRouter();
  const { isOnline, unsyncedCount, lastSyncTimestamp } = useSyncStatus();
  const { pendingEntries, loadPendingEntries, conflicts } = useSyncStore();
  const { sync, isSyncing, syncError } = useSyncManager();

  useEffect(() => {
    loadPendingEntries();
  }, [loadPendingEntries]);

  return (
    <View style={styles.container}>
      <View style={styles.statusCard}>
        <View style={styles.statusRow}>
          <Text style={styles.label}>Status</Text>
          <View style={styles.statusValue}>
            <View
              style={[
                styles.dot,
                isOnline ? styles.online : styles.offline,
              ]}
            />
            <Text style={styles.value}>
              {isOnline ? "Online" : "Offline"}
            </Text>
          </View>
        </View>
        <View style={styles.statusRow}>
          <Text style={styles.label}>Ausstehend</Text>
          <Text style={styles.value}>
            {unsyncedCount} {unsyncedCount === 1 ? "Änderung" : "Änderungen"}
          </Text>
        </View>
        <View style={styles.statusRow}>
          <Text style={styles.label}>Letzte Sync</Text>
          <Text style={styles.value}>
            {lastSyncTimestamp ?? "Noch nicht synchronisiert"}
          </Text>
        </View>

        <TouchableOpacity
          style={[
            styles.syncButton,
            (!isOnline || isSyncing) && styles.syncButtonDisabled,
          ]}
          onPress={() => void sync()}
          disabled={!isOnline || isSyncing}
        >
          {isSyncing ? (
            <ActivityIndicator color="#fff" size="small" />
          ) : (
            <Text style={styles.syncButtonText}>Jetzt synchronisieren</Text>
          )}
        </TouchableOpacity>

        {syncError && (
          <Text style={styles.syncError}>{syncError}</Text>
        )}
      </View>

      {conflicts.length > 0 && (
        <TouchableOpacity
          style={styles.conflictCard}
          onPress={() => router.push("/(tabs)/sync/conflicts")}
        >
          <View style={styles.conflictBadge}>
            <Text style={styles.conflictBadgeText}>{conflicts.length}</Text>
          </View>
          <Text style={styles.conflictText}>
            {conflicts.length === 1
              ? "1 Konflikt zu lösen"
              : `${conflicts.length} Konflikte zu lösen`}
          </Text>
          <Text style={styles.chevron}>›</Text>
        </TouchableOpacity>
      )}

      <Text style={styles.sectionTitle}>Ausstehende Änderungen</Text>

      {pendingEntries.length === 0 ? (
        <View style={styles.emptyState}>
          <Text style={styles.emptyText}>
            Keine ausstehenden Änderungen
          </Text>
        </View>
      ) : (
        <FlatList
          data={pendingEntries}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => <OutboxItem item={item} />}
          contentContainerStyle={styles.list}
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#F2F2F7",
  },
  statusCard: {
    backgroundColor: "#fff",
    margin: 16,
    borderRadius: 12,
    padding: 16,
    gap: 12,
  },
  statusRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  statusValue: {
    flexDirection: "row",
    alignItems: "center",
  },
  label: {
    fontSize: 15,
    color: "#8E8E93",
  },
  value: {
    fontSize: 15,
    fontWeight: "500",
  },
  dot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6,
  },
  online: {
    backgroundColor: "#34C759",
  },
  offline: {
    backgroundColor: "#FF3B30",
  },
  syncButton: {
    backgroundColor: "#007AFF",
    paddingVertical: 12,
    borderRadius: 10,
    alignItems: "center",
    marginTop: 4,
  },
  syncButtonDisabled: {
    backgroundColor: "#C7C7CC",
  },
  syncButtonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  syncError: {
    color: "#FF3B30",
    fontSize: 13,
    textAlign: "center",
  },
  conflictCard: {
    backgroundColor: "#FFF3CD",
    marginHorizontal: 16,
    marginBottom: 16,
    borderRadius: 12,
    padding: 14,
    flexDirection: "row",
    alignItems: "center",
    gap: 10,
  },
  conflictBadge: {
    backgroundColor: "#FF9500",
    width: 28,
    height: 28,
    borderRadius: 14,
    justifyContent: "center",
    alignItems: "center",
  },
  conflictBadgeText: {
    color: "#fff",
    fontSize: 13,
    fontWeight: "700",
  },
  conflictText: {
    flex: 1,
    fontSize: 15,
    fontWeight: "500",
    color: "#856404",
  },
  chevron: {
    fontSize: 22,
    color: "#856404",
  },
  sectionTitle: {
    fontSize: 17,
    fontWeight: "600",
    marginHorizontal: 16,
    marginBottom: 8,
  },
  list: {
    paddingHorizontal: 16,
  },
  emptyState: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    paddingTop: 40,
  },
  emptyText: {
    fontSize: 15,
    color: "#8E8E93",
  },
});

const itemStyles = StyleSheet.create({
  container: {
    backgroundColor: "#fff",
    borderRadius: 10,
    padding: 12,
    marginBottom: 8,
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 4,
  },
  entityType: {
    fontSize: 15,
    fontWeight: "600",
    textTransform: "capitalize",
  },
  statusBadge: {
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 10,
  },
  statusText: {
    color: "#fff",
    fontSize: 11,
    fontWeight: "600",
    textTransform: "uppercase",
  },
  operation: {
    fontSize: 13,
    color: "#666",
  },
  timestamp: {
    fontSize: 12,
    color: "#8E8E93",
    marginTop: 2,
  },
  retry: {
    fontSize: 12,
    color: "#FF3B30",
    marginTop: 2,
  },
});
