import { View, Text, StyleSheet } from "react-native";
import { StatusBadge } from "@/components/common";
import { InstallationDetails } from "./InstallationDetails";
import { Colors, Spacing, FontSize, Radius } from "@/styles/tokens";
import type { Installation } from "@/db/repositories/types";

type InstallationInfoSectionProps = {
  installation: Installation;
};

export function InstallationInfoSection({ installation }: InstallationInfoSectionProps) {
  const subtitle = [installation.manufacturer, installation.model]
    .filter(Boolean)
    .join(" — ");

  return (
    <>
      <View style={styles.card}>
        <View style={styles.headerRow}>
          <Text style={styles.type}>{installation.type}</Text>
          <StatusBadge status={installation.status} />
        </View>
        {subtitle ? (
          <Text style={styles.subtitle}>{subtitle}</Text>
        ) : null}
        {installation.serialNumber ? (
          <Text style={styles.detail}>
            SN: {installation.serialNumber}
          </Text>
        ) : null}
        {installation.notes ? (
          <Text style={styles.notes}>{installation.notes}</Text>
        ) : null}
      </View>

      <InstallationDetails installation={installation} />
    </>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
  },
  headerRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: Spacing.xs,
  },
  type: {
    fontSize: FontSize.title,
    fontWeight: "700",
    color: Colors.textPrimary,
    flex: 1,
    marginRight: Spacing.sm,
  },
  subtitle: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
    marginBottom: Spacing.xs,
  },
  detail: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  notes: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
    marginTop: Spacing.sm,
  },
});
