import React from "react";
import { View, Text, StyleSheet } from "react-native";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

interface DashboardCardProps {
  title: string;
  value: number;
  subtitle?: string;
  color?: string;
}

export function DashboardCard({
  title,
  value,
  subtitle,
  color = Colors.primary,
}: DashboardCardProps) {
  return (
    <View style={styles.card}>
      <Text style={styles.title}>{title}</Text>
      <Text style={[styles.value, { color }]}>{value}</Text>
      {subtitle ? <Text style={styles.subtitle}>{subtitle}</Text> : null}
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    flex: 1,
  },
  title: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    fontWeight: "500",
    marginBottom: Spacing.xs,
  },
  value: {
    fontSize: 28,
    fontWeight: "700",
  },
  subtitle: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
    marginTop: Spacing.xs,
  },
});
