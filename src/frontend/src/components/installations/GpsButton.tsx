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
    return (
      <View style={styles.successCard}>
        <View style={styles.successHeader}>
          <FontAwesome
            name="map-marker"
            size={16}
            color={Colors.success}
            style={styles.successIcon}
          />
          <Text style={styles.successTitle}>GPS-Position erfasst</Text>
          {onClear && (
            <TouchableOpacity onPress={onClear}>
              <FontAwesome name="close" size={16} color={Colors.textTertiary} />
            </TouchableOpacity>
          )}
        </View>
        <Text style={styles.coordText}>
          {position.gpsLat.toFixed(6)}, {position.gpsLng.toFixed(6)}
        </Text>
        <Text style={styles.accuracyText}>
          Genauigkeit: {position.gpsAccuracy.toFixed(1)} m
        </Text>
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
    backgroundColor: "#E8F5E9",
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
    color: Colors.success,
    flex: 1,
  },
  coordText: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    fontFamily: "SpaceMono",
  },
  accuracyText: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
    marginTop: 2,
  },
  errorText: {
    fontSize: FontSize.caption,
    color: Colors.danger,
    marginTop: Spacing.xs,
  },
});
