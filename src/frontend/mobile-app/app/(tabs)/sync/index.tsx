import { useEffect } from "react";
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
} from "react-native";
import { useRouter } from "expo-router";
import { useSyncStore } from "../../../src/store";
import { useSyncStatus, useSyncManager } from "../../../src/hooks";
import { UploadQueueCard } from "../../../src/components/sync";
import { Button } from "../../../src/components/core";
import { StatusBadge } from "../../../src/components/common";
import { Colors, Spacing, FontSize, Radius } from "../../../src/styles/tokens";
import { formatDateTime } from "../../../src/utils";
import type { SyncOutboxEntry } from "../../../src/db/repositories/types";

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

function OutboxItem({ item }: { item: SyncOutboxEntry }) {
  return (
    <View style={itemStyles.container}>
      <View style={itemStyles.header}>
        <Text style={itemStyles.entityType}>{item.entityType}</Text>
        <StatusBadge status={item.status} />
      </View>
      <Text style={itemStyles.operation}>
        {operationLabel(item.operation)}
      </Text>
      <Text style={itemStyles.timestamp}>
        {formatDateTime(item.timestamp)}
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

        <Button
          title="Jetzt synchronisieren"
          onPress={() => void sync()}
          loading={isSyncing}
          disabled={!isOnline}
          style={{ marginTop: Spacing.xs }}
        />

        {syncError && (
          <Text style={styles.syncError}>{syncError}</Text>
        )}
      </View>

      <UploadQueueCard />

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
    backgroundColor: Colors.background,
  },
  statusCard: {
    backgroundColor: Colors.card,
    margin: Spacing.lg,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    gap: Spacing.md,
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
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
  value: {
    fontSize: FontSize.body,
    fontWeight: "500",
  },
  dot: {
    width: 8,
    height: 8,
    borderRadius: Radius.xs,
    marginRight: 6,
  },
  online: {
    backgroundColor: Colors.success,
  },
  offline: {
    backgroundColor: Colors.danger,
  },
  syncError: {
    color: Colors.danger,
    fontSize: FontSize.caption,
    textAlign: "center",
  },
  conflictCard: {
    backgroundColor: Colors.diffHighlight,
    marginHorizontal: Spacing.lg,
    marginBottom: Spacing.lg,
    borderRadius: Radius.lg,
    padding: Spacing.md,
    flexDirection: "row",
    alignItems: "center",
    gap: Spacing.sm,
  },
  conflictBadge: {
    backgroundColor: Colors.warning,
    width: 28,
    height: 28,
    borderRadius: 14,
    justifyContent: "center",
    alignItems: "center",
  },
  conflictBadgeText: {
    color: Colors.white,
    fontSize: FontSize.caption,
    fontWeight: "700",
  },
  conflictText: {
    flex: 1,
    fontSize: FontSize.body,
    fontWeight: "500",
    color: Colors.warningText,
  },
  chevron: {
    fontSize: 22,
    color: Colors.warningText,
  },
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginHorizontal: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  list: {
    paddingHorizontal: Spacing.lg,
  },
  emptyState: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    paddingTop: 40,
  },
  emptyText: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
});

const itemStyles = StyleSheet.create({
  container: {
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: Spacing.xs,
  },
  entityType: {
    fontSize: FontSize.body,
    fontWeight: "600",
    textTransform: "capitalize",
  },
  operation: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
  },
  timestamp: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
    marginTop: 2,
  },
  retry: {
    fontSize: FontSize.footnote,
    color: Colors.danger,
    marginTop: 2,
  },
});
