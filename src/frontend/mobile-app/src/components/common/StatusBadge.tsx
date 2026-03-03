import { View, Text, StyleSheet } from 'react-native';
import { PROJECT_STATUS_LABELS, ZONE_TYPE_LABELS } from '@baudoku/projects';
import { INSTALLATION_STATUS_LABELS, MEASUREMENT_RESULT_LABELS } from '@baudoku/documentation';
import { SYNC_STATUS_LABELS } from '@baudoku/sync';
import { Colors, Spacing, Radius } from '../../styles/tokens';

const statusColors: Record<string, string> = {
  // Project
  active: Colors.success,
  completed: Colors.primary,
  archived: Colors.textTertiary,
  // Installation
  planned: Colors.textTertiary,
  in_progress: Colors.warning,
  inspected: Colors.primary,
  // Measurement result
  passed: Colors.success,
  failed: Colors.danger,
  warning: Colors.warning,
  // Sync outbox
  pending: Colors.warning,
  syncing: Colors.primary,
  // Zone type
  building: Colors.zoneBuilding,
  floor: Colors.zoneFloor,
  room: Colors.primary,
  trench: Colors.warning,
  section: Colors.textTertiary,
};

const statusLabels: Record<string, string> = {
  ...PROJECT_STATUS_LABELS,
  ...INSTALLATION_STATUS_LABELS,
  ...ZONE_TYPE_LABELS,
  ...MEASUREMENT_RESULT_LABELS,
  ...SYNC_STATUS_LABELS,
  section: 'Abschnitt',
};

type StatusBadgeProps = {
  status: string;
  label?: string;
};

export function StatusBadge({ status, label }: StatusBadgeProps) {
  const bg = statusColors[status] ?? Colors.textTertiary;
  const text = label ?? statusLabels[status] ?? status;

  return (
    <View style={[styles.badge, { backgroundColor: bg }]}>
      <Text style={styles.text}>{text}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  badge: {
    paddingHorizontal: Spacing.sm,
    paddingVertical: 2,
    borderRadius: Radius.md,
    alignSelf: 'flex-start',
  },
  text: {
    color: Colors.white,
    fontSize: 11,
    fontWeight: '600',
    textTransform: 'uppercase',
  },
});
