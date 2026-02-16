import React from "react";
import { View, Text, StyleSheet } from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import {
  calculateGpsQuality,
  gpsSourceLabels,
  type GpsQualityResult,
} from "../../utils/gpsQuality";
import { Colors, Spacing, FontSize, FontFamily } from "../../styles/tokens";

type QualityIndicatorProps = {
  gpsAccuracy: number;
  gpsHdop?: number | null;
  gpsSatCount?: number | null;
  gpsCorrService?: string | null;
  gpsSource?: string | null;
  compact?: boolean;
};

export function QualityIndicator({
  gpsAccuracy,
  gpsHdop,
  gpsSatCount,
  gpsCorrService,
  gpsSource,
  compact = false,
}: QualityIndicatorProps) {
  const quality = calculateGpsQuality({
    gpsAccuracy,
    gpsHdop,
    gpsSatCount,
    gpsCorrService,
  });

  if (compact) {
    return <GradeBadge quality={quality} />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.topRow}>
        <GradeBadge quality={quality} />
        <Text style={styles.qualityLabel}>{quality.label}</Text>
        <Text style={styles.accuracyText}>
          {gpsAccuracy.toFixed(1)} m
        </Text>
      </View>

      <View style={styles.detailRow}>
        {gpsSatCount != null && (
          <View style={styles.detailItem}>
            <FontAwesome
              name="signal"
              size={11}
              color={Colors.textTertiary}
              style={styles.detailIcon}
            />
            <Text style={styles.detailText}>{gpsSatCount} Sat.</Text>
          </View>
        )}
        {gpsSource && (
          <View style={styles.detailItem}>
            <FontAwesome
              name="location-arrow"
              size={11}
              color={Colors.textTertiary}
              style={styles.detailIcon}
            />
            <Text style={styles.detailText}>
              {gpsSourceLabels[gpsSource] ?? gpsSource}
            </Text>
          </View>
        )}
        {gpsHdop != null && (
          <View style={styles.detailItem}>
            <Text style={styles.detailText}>HDOP {gpsHdop.toFixed(1)}</Text>
          </View>
        )}
      </View>

      {quality.grade === "D" && (
        <View style={styles.warningBanner}>
          <FontAwesome
            name="exclamation-triangle"
            size={12}
            color={Colors.danger}
            style={styles.warningIcon}
          />
          <Text style={styles.warningText}>
            Geringe GPS-Qualität — Position ggf. ungenau
          </Text>
        </View>
      )}
    </View>
  );
}

function GradeBadge({ quality }: { quality: GpsQualityResult }) {
  return (
    <View style={[styles.gradeBadge, { backgroundColor: quality.color }]}>
      <Text style={styles.gradeText}>{quality.grade}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: Spacing.xs,
  },
  topRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: Spacing.sm,
  },
  gradeBadge: {
    width: 24,
    height: 24,
    borderRadius: 6,
    alignItems: "center",
    justifyContent: "center",
  },
  gradeText: {
    color: Colors.white,
    fontSize: FontSize.footnote,
    fontWeight: "700",
  },
  qualityLabel: {
    fontSize: FontSize.body,
    fontWeight: "600",
    flex: 1,
  },
  accuracyText: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    fontFamily: FontFamily.mono,
  },
  detailRow: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: Spacing.md,
  },
  detailItem: {
    flexDirection: "row",
    alignItems: "center",
  },
  detailIcon: {
    marginRight: 4,
  },
  detailText: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
  },
  warningBanner: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: Colors.gpsBgD,
    paddingHorizontal: Spacing.sm,
    paddingVertical: Spacing.xs,
    borderRadius: 6,
  },
  warningIcon: {
    marginRight: Spacing.xs,
  },
  warningText: {
    fontSize: FontSize.footnote,
    color: Colors.danger,
    flex: 1,
  },
});
