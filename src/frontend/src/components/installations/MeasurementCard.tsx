import React from "react";
import { View, Text, StyleSheet } from "react-native";
import type { Measurement } from "../../db/repositories/types";
import { StatusBadge } from "../common/StatusBadge";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

interface MeasurementCardProps {
  measurement: Measurement;
}

export function MeasurementCard({ measurement }: MeasurementCardProps) {
  const m = measurement;
  const thresholds =
    m.minThreshold != null || m.maxThreshold != null
      ? [
          m.minThreshold != null ? `Min: ${m.minThreshold}` : null,
          m.maxThreshold != null ? `Max: ${m.maxThreshold}` : null,
        ]
          .filter(Boolean)
          .join(" / ")
      : null;

  return (
    <View style={styles.card}>
      <View style={styles.header}>
        <Text style={styles.type}>{m.type}</Text>
        {m.result && <StatusBadge status={m.result} />}
      </View>
      <Text style={styles.value}>
        {m.value} {m.unit}
      </Text>
      {thresholds && <Text style={styles.threshold}>{thresholds}</Text>}
      {m.notes ? <Text style={styles.notes}>{m.notes}</Text> : null}
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    borderRadius: 10,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 2,
  },
  type: {
    fontSize: FontSize.body,
    fontWeight: "600",
    color: Colors.textPrimary,
    flex: 1,
    marginRight: Spacing.sm,
  },
  value: {
    fontSize: FontSize.headline,
    fontWeight: "700",
    color: Colors.textPrimary,
    marginVertical: Spacing.xs,
  },
  threshold: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
  },
  notes: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
});
