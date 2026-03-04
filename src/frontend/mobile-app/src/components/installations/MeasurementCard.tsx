import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { FontAwesome } from '@expo/vector-icons';
import type { Measurement } from '../../db/repositories/types';
import { StatusBadge } from '../common';
import { Card, Body, Caption } from '../core';
import { Colors, Spacing, FontSize, Radius } from '../../styles/tokens';

type MeasurementCardProps = {
  measurement: Measurement;
  onDelete?: (measurement: Measurement) => void;
};

export function MeasurementCard({ measurement, onDelete }: MeasurementCardProps) {
  const { type, result, value, unit, notes, minThreshold, maxThreshold } = measurement;
  const thresholds =
    minThreshold != null || maxThreshold != null
      ? [
          minThreshold != null ? `Min: ${minThreshold}` : null,
          maxThreshold != null ? `Max: ${maxThreshold}` : null,
        ]
          .filter(Boolean)
          .join(' / ')
      : null;

  return (
    <Card style={styles.card}>
      <View style={styles.header}>
        <Body style={styles.type}>{type}</Body>
        <View style={styles.headerRight}>
          {result && <StatusBadge status={result} />}
          {onDelete && (
            <TouchableOpacity style={styles.deleteButton} onPress={() => onDelete(measurement)}>
              <FontAwesome name="trash-o" size={16} color={Colors.danger} />
            </TouchableOpacity>
          )}
        </View>
      </View>
      <Text style={styles.value}>
        {value} {unit}
      </Text>
      {thresholds && <Caption style={styles.threshold}>{thresholds}</Caption>}
      {notes ? <Caption style={styles.notes}>{notes}</Caption> : null}
    </Card>
  );
}

const styles = StyleSheet.create({
  card: {
    borderRadius: Radius.md,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 2,
  },
  headerRight: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.sm,
  },
  type: {
    fontWeight: '600',
    flex: 1,
    marginRight: Spacing.sm,
  },
  deleteButton: {
    padding: Spacing.xs,
  },
  value: {
    fontSize: FontSize.headline,
    fontWeight: '700',
    color: Colors.textPrimary,
    marginVertical: Spacing.xs,
  },
  threshold: {
    fontSize: FontSize.footnote,
  },
  notes: {
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
});
