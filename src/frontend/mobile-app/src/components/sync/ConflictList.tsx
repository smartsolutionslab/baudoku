import { View, Text, FlatList, TouchableOpacity, StyleSheet } from 'react-native';
import { Colors, Spacing, FontSize, Radius, FontFamily } from '../../styles/tokens';
import type { ConflictDto } from '../../sync/syncApi';

type ConflictListProps = {
  conflicts: ConflictDto[];
  onSelect: (conflict: ConflictDto) => void;
};

type ConflictItemProps = {
  item: ConflictDto;
  onPress: () => void;
};

function ConflictItem({ item, onPress }: ConflictItemProps) {
  const { entityType, status, entityId, clientVersion, serverVersion } = item;
  return (
    <TouchableOpacity style={styles.item} onPress={onPress}>
      <View style={styles.itemHeader}>
        <View style={styles.typeBadge}>
          <Text style={styles.typeBadgeText}>{entityType}</Text>
        </View>
        <View style={styles.statusBadge}>
          <Text style={styles.statusBadgeText}>{status}</Text>
        </View>
      </View>
      <Text style={styles.entityId} numberOfLines={1}>
        {entityId}
      </Text>
      <Text style={styles.versions}>
        Client v{clientVersion} / Server v{serverVersion}
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
      keyExtractor={({ id }) => id}
      renderItem={({ item }) => <ConflictItem item={item} onPress={() => onSelect(item)} />}
      contentContainerStyle={styles.list}
    />
  );
}

const styles = StyleSheet.create({
  list: {
    padding: Spacing.lg,
  },
  item: {
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  itemHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.xs,
  },
  typeBadge: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.sm,
    paddingVertical: 2,
    borderRadius: Radius.md,
  },
  typeBadgeText: {
    color: Colors.white,
    fontSize: 11,
    fontWeight: '600',
    textTransform: 'capitalize',
  },
  statusBadge: {
    backgroundColor: Colors.warning,
    paddingHorizontal: Spacing.sm,
    paddingVertical: 2,
    borderRadius: Radius.md,
  },
  statusBadgeText: {
    color: Colors.white,
    fontSize: 11,
    fontWeight: '600',
    textTransform: 'capitalize',
  },
  entityId: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    fontFamily: FontFamily.mono,
    marginBottom: 2,
  },
  versions: {
    fontSize: FontSize.footnote,
    color: Colors.textSecondary,
  },
  empty: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingTop: 60,
  },
  emptyText: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
});
