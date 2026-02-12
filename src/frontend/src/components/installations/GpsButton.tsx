import React from "react";
import {
  View,
  Text,
  TouchableOpacity,
  ActivityIndicator,
  StyleSheet,
} from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import type { GpsPosition } from "../../hooks/useGpsCapture";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

interface GpsButtonProps {
  position: GpsPosition | null;
  capturing: boolean;
  error: string | null;
  onCapture: () => void;
  onClear?: () => void;
}

function getAccuracyColor(accuracy: number): string {
  if (accuracy < 5) return Colors.success;
  if (accuracy < 15) return Colors.warning;
  if (accuracy < 30) return "#FF6B00";
  return Colors.danger;
}

function getAccuracyLabel(accuracy: number): string {
  if (accuracy < 5) return "Ausgezeichnet";
  if (accuracy < 15) return "Akzeptabel";
  if (accuracy < 30) return "Ungenau";
  return "Sehr ungenau";
}

function getAccuracyBackground(accuracy: number): string {
  if (accuracy < 5) return "#E8F5E9";
  if (accuracy < 15) return "#FFF8E1";
  if (accuracy < 30) return "#FFF3E0";
  return "#FFEBEE";
}

export function GpsButton({
  position,
  capturing,
  error,
  onCapture,
  onClear,
}: GpsButtonProps) {
  if (capturing) {
    return (
      <View style={styles.captureCard}>
        <ActivityIndicator size="small" color={Colors.primary} />
        <Text style={styles.captureText}>GPS-Position wird erfasst...</Text>
      </View>
    );
  }

  if (position) {
    const color = getAccuracyColor(position.gpsAccuracy);
    const label = getAccuracyLabel(position.gpsAccuracy);
    const bgColor = getAccuracyBackground(position.gpsAccuracy);

    return (
      <View style={[styles.successCard, { backgroundColor: bgColor }]}>
        <View style={styles.successHeader}>
          <FontAwesome
            name="map-marker"
            size={16}
            color={color}
            style={styles.successIcon}
          />
          <Text style={[styles.successTitle, { color }]}>
            GPS-Position erfasst
          </Text>
          <View style={styles.headerActions}>
            <TouchableOpacity
              style={styles.recaptureButton}
              onPress={onCapture}
            >
              <FontAwesome name="refresh" size={14} color={Colors.primary} />
              <Text style={styles.recaptureText}>Erneut</Text>
            </TouchableOpacity>
            {onClear && (
              <TouchableOpacity onPress={onClear} style={styles.clearButton}>
                <FontAwesome
                  name="close"
                  size={16}
                  color={Colors.textTertiary}
                />
              </TouchableOpacity>
            )}
          </View>
        </View>
        <Text style={styles.coordText}>
          {position.gpsLat.toFixed(6)}, {position.gpsLng.toFixed(6)}
        </Text>
        <View style={styles.accuracyRow}>
          <View style={[styles.accuracyBadge, { backgroundColor: color }]}>
            <Text style={styles.accuracyBadgeText}>
              {position.gpsAccuracy.toFixed(1)} m
            </Text>
          </View>
          <Text style={styles.accuracyLabel}>{label}</Text>
        </View>
      </View>
    );
  }

  return (
    <View>
      <TouchableOpacity style={styles.button} onPress={onCapture}>
        <FontAwesome
          name="crosshairs"
          size={16}
          color="#fff"
          style={styles.buttonIcon}
        />
        <Text style={styles.buttonText}>GPS-Position erfassen</Text>
      </TouchableOpacity>
      {error && <Text style={styles.errorText}>{error}</Text>}
    </View>
  );
}

const styles = StyleSheet.create({
  button: {
    backgroundColor: Colors.primary,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    paddingVertical: 12,
    borderRadius: 10,
    marginTop: Spacing.lg,
  },
  buttonIcon: {
    marginRight: Spacing.sm,
  },
  buttonText: {
    color: "#fff",
    fontSize: FontSize.body,
    fontWeight: "600",
  },
  captureCard: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: Colors.background,
    borderRadius: 10,
    paddingVertical: 14,
    marginTop: Spacing.lg,
  },
  captureText: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
    marginLeft: Spacing.sm,
  },
  successCard: {
    borderRadius: 10,
    padding: Spacing.md,
    marginTop: Spacing.lg,
  },
  successHeader: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: Spacing.xs,
  },
  successIcon: {
    marginRight: Spacing.xs,
  },
  successTitle: {
    fontSize: FontSize.body,
    fontWeight: "600",
    flex: 1,
  },
  headerActions: {
    flexDirection: "row",
    alignItems: "center",
    gap: Spacing.sm,
  },
  recaptureButton: {
    flexDirection: "row",
    alignItems: "center",
    paddingHorizontal: Spacing.sm,
    paddingVertical: Spacing.xs,
    borderRadius: 6,
    backgroundColor: "rgba(0, 122, 255, 0.1)",
  },
  recaptureText: {
    fontSize: FontSize.footnote,
    color: Colors.primary,
    fontWeight: "600",
    marginLeft: 4,
  },
  clearButton: {
    padding: Spacing.xs,
  },
  coordText: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    fontFamily: "SpaceMono",
  },
  accuracyRow: {
    flexDirection: "row",
    alignItems: "center",
    marginTop: Spacing.xs,
  },
  accuracyBadge: {
    paddingHorizontal: Spacing.sm,
    paddingVertical: 2,
    borderRadius: 10,
    marginRight: Spacing.sm,
  },
  accuracyBadgeText: {
    color: "#fff",
    fontSize: FontSize.footnote,
    fontWeight: "700",
  },
  accuracyLabel: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
  },
  errorText: {
    fontSize: FontSize.caption,
    color: Colors.danger,
    marginTop: Spacing.xs,
  },
});
