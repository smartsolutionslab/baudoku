import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import type { Measurement } from "../../db/repositories/types";
import { StatusBadge } from "../common/StatusBadge";
import { Card, Body, Caption } from "../core";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

type MeasurementCardProps = {
  measurement: Measurement;
  onDelete?: (measurement: Measurement) => void;
};

export function MeasurementCard({ measurement, onDelete }: MeasurementCardProps) {
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
    <Card style={styles.card}>
      <View style={styles.header}>
        <Body style={styles.type}>{m.type}</Body>
        <View style={styles.headerRight}>
          {m.result && <StatusBadge status={m.result} />}
          {onDelete && (
            <TouchableOpacity
              style={styles.deleteButton}
              onPress={() => onDelete(m)}
            >
              <FontAwesome name="trash-o" size={16} color={Colors.danger} />
            </TouchableOpacity>
          )}
        </View>
      </View>
      <Text style={styles.value}>
        {m.value} {m.unit}
      </Text>
      {thresholds && <Caption style={styles.threshold}>{thresholds}</Caption>}
      {m.notes ? <Caption style={styles.notes}>{m.notes}</Caption> : null}
    </Card>
  );
}

const styles = StyleSheet.create({
  card: {
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
  headerRight: {
    flexDirection: "row",
    alignItems: "center",
    gap: Spacing.sm,
  },
  type: {
    fontWeight: "600",
    flex: 1,
    marginRight: Spacing.sm,
  },
  deleteButton: {
    padding: Spacing.xs,
  },
  value: {
    fontSize: FontSize.headline,
    fontWeight: "700",
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
