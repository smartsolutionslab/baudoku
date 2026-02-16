import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import type { Installation } from "../../db/repositories/types";
import { StatusBadge } from "../common";
import { calculateGpsQuality, formatDate } from "../../utils";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type InstallationCardProps = {
  installation: Installation;
  onPress: () => void;
};

export function InstallationCard({ installation, onPress }: InstallationCardProps) {
  const subtitle = [installation.manufacturer, installation.model]
    .filter(Boolean)
    .join(" — ");

  const gpsQuality = installation.gpsAccuracy != null
      ? calculateGpsQuality({
          gpsAccuracy: installation.gpsAccuracy,
          gpsHdop: installation.gpsHdop,
          gpsSatCount: installation.gpsSatCount,
          gpsCorrService: installation.gpsCorrService,
        })
      : null;

  return (
    <TouchableOpacity style={styles.card} onPress={onPress} activeOpacity={0.7}>
      <View style={styles.header}>
        <Text style={styles.type} numberOfLines={1}>
          {installation.type}
        </Text>
        {gpsQuality && (
          <View style={[styles.gpsBadge, { backgroundColor: gpsQuality.color }]}>
            <Text style={styles.gpsBadgeText}>{gpsQuality.grade}</Text>
          </View>
        )}
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
    borderRadius: Radius.md,
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
  gpsBadge: {
    width: 20,
    height: 20,
    borderRadius: 5,
    alignItems: "center",
    justifyContent: "center",
    marginRight: Spacing.xs,
  },
  gpsBadgeText: {
    color: Colors.white,
    fontSize: 10,
    fontWeight: "700",
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
