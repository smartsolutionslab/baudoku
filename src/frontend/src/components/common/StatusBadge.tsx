import React from "react";
import { View, Text, StyleSheet } from "react-native";
import { Colors, Spacing, Radius } from "../../styles/tokens";

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
  building: "#5856D6",
  floor: "#AF52DE",
  room: Colors.primary,
  trench: Colors.warning,
  section: Colors.textTertiary,
};

export const statusLabels: Record<string, string> = {
  active: "Aktiv",
  completed: "Abgeschlossen",
  archived: "Archiviert",
  planned: "Geplant",
  in_progress: "In Arbeit",
  inspected: "Geprüft",
  passed: "Bestanden",
  failed: "Fehlgeschlagen",
  warning: "Warnung",
  pending: "Ausstehend",
  syncing: "Wird synchronisiert",
  building: "Gebäude",
  floor: "Stockwerk",
  room: "Raum",
  trench: "Graben",
  section: "Abschnitt",
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
    alignSelf: "flex-start",
  },
  text: {
    color: Colors.white,
    fontSize: 11,
    fontWeight: "600",
    textTransform: "uppercase",
  },
});
