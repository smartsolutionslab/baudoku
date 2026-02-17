import { Text, StyleSheet } from "react-native";
import { Card, Caption } from "../core";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

type DashboardCardProps = {
  title: string;
  value: number;
  subtitle?: string;
  color?: string;
};

export function DashboardCard({ title, value, subtitle, color = Colors.primary}: DashboardCardProps) {
  return (
    <Card style={styles.card}>
      <Caption style={styles.title}>{title}</Caption>
      <Text style={[styles.value, { color }]}>{value}</Text>
      {subtitle ? <Caption style={styles.subtitle}>{subtitle}</Caption> : null}
    </Card>
  );
}

const styles = StyleSheet.create({
  card: {
    flex: 1,
  },
  title: {
    fontWeight: "500",
    marginBottom: Spacing.xs,
  },
  value: {
    fontSize: 28,
    fontWeight: "700",
  },
  subtitle: {
    fontSize: FontSize.footnote,
    marginTop: Spacing.xs,
  },
});
