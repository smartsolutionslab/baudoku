import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import type { Installation } from "../../db/repositories/types";
import { StatusBadge } from "../common/StatusBadge";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

interface InstallationCardProps {
  installation: Installation;
  onPress: () => void;
}

function formatDate(d: Date | null | undefined): string {
  if (!d) return "";
  return d.toLocaleDateString("de-DE", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

export function InstallationCard({
  installation,
  onPress,
}: InstallationCardProps) {
  const subtitle = [installation.manufacturer, installation.model]
    .filter(Boolean)
    .join(" — ");

  return (
    <TouchableOpacity style={styles.card} onPress={onPress} activeOpacity={0.7}>
      <View style={styles.header}>
        <Text style={styles.type} numberOfLines={1}>
          {installation.type}
        </Text>
        <StatusBadge status={installation.status} />
      </View>
      {subtitle ? (
        <Text style={styles.subtitle} numberOfLines={1}>
          {subtitle}
        </Text>
      ) : null}
      <Text style={styles.date}>{formatDate(installation.createdAt)}</Text>
    </TouchableOpacity>
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
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textPrimary,
    flex: 1,
    marginRight: Spacing.sm,
  },
  subtitle: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    marginBottom: 2,
  },
  date: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
  },
});
