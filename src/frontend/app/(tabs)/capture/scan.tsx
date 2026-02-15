import React, { useState } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
} from "react-native";
import { useRouter } from "expo-router";
import { CameraView, useCameraPermissions } from "expo-camera";
import { FontAwesome } from "@expo/vector-icons";
import { useQrScanner } from "../../../src/hooks/useQrScanner";
import { ScanOverlay } from "../../../src/components/capture/ScanOverlay";
import { Button } from "../../../src/components/core/Button";
import { Colors, Spacing, FontSize, Radius } from "../../../src/styles/tokens";
import * as zoneRepo from "../../../src/db/repositories/zoneRepo";
import * as projectRepo from "../../../src/db/repositories/projectRepo";

export default function ScanScreen() {
  const router = useRouter();
  const [permission, requestPermission] = useCameraPermissions();
  const { scanned, scanResult, error, onBarcodeScanned, resetScanner } =
    useQrScanner();
  const [torch, setTorch] = useState(false);
  const [zoneName, setZoneName] = useState<string | null>(null);
  const [projectName, setProjectName] = useState<string | null>(null);

  React.useEffect(() => {
    if (scanResult) {
      (async () => {
        const zone = await zoneRepo.getById(scanResult.zoneId);
        setZoneName(zone?.name ?? "Unbekannte Zone");
        const project = await projectRepo.getById(scanResult.projectId);
        setProjectName(project?.name ?? "Unbekanntes Projekt");
      })();
    }
  }, [scanResult]);

  if (!permission) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" color={Colors.primary} />
      </View>
    );
  }

  if (!permission.granted) {
    return (
      <View style={styles.centered}>
        <FontAwesome
          name="camera"
          size={48}
          color={Colors.textTertiary}
          style={{ marginBottom: Spacing.lg }}
        />
        <Text style={styles.permissionText}>
          Kamerazugriff wird für den QR-Scanner benötigt.
        </Text>
        <Button
          title="Berechtigung erteilen"
          onPress={requestPermission}
          style={{ marginBottom: Spacing.md, paddingHorizontal: Spacing.xl }}
        />
        <TouchableOpacity
          style={styles.backBtn}
          onPress={() => router.back()}
        >
          <Text style={styles.backBtnText}>Zurück</Text>
        </TouchableOpacity>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <CameraView
        style={StyleSheet.absoluteFill}
        facing="back"
        enableTorch={torch}
        barcodeScannerSettings={{ barcodeTypes: ["qr"] }}
        onBarcodeScanned={scanned ? undefined : onBarcodeScanned}
      />
      <ScanOverlay />

      {/* Top bar */}
      <View style={styles.topBar}>
        <TouchableOpacity onPress={() => router.back()} style={styles.iconBtn}>
          <FontAwesome name="arrow-left" size={20} color={Colors.white} />
        </TouchableOpacity>
        <Text style={styles.topTitle}>QR-Scanner</Text>
        <TouchableOpacity
          onPress={() => setTorch((t) => !t)}
          style={styles.iconBtn}
        >
          <FontAwesome
            name={torch ? "flash" : "flash"}
            size={20}
            color={torch ? Colors.warning : Colors.white}
          />
        </TouchableOpacity>
      </View>

      {/* Result card */}
      {scanned && (
        <View style={styles.resultContainer}>
          {error ? (
            <View style={styles.errorCard}>
              <FontAwesome name="exclamation-circle" size={24} color={Colors.danger} />
              <Text style={styles.errorText}>{error}</Text>
              <TouchableOpacity style={styles.retryBtn} onPress={resetScanner}>
                <Text style={styles.retryBtnText}>Erneut scannen</Text>
              </TouchableOpacity>
            </View>
          ) : scanResult && zoneName ? (
            <View style={styles.successCard}>
              <FontAwesome name="check-circle" size={24} color={Colors.success} />
              <Text style={styles.successTitle}>Zone gefunden</Text>
              <Text style={styles.successZone}>{zoneName}</Text>
              <Text style={styles.successProject}>{projectName}</Text>
              <View style={styles.actionRow}>
                <TouchableOpacity
                  style={[styles.actionBtn, styles.actionBtnSecondary]}
                  onPress={() => {
                    router.replace(
                      `/(tabs)/projects/zone/${scanResult.zoneId}?projectId=${scanResult.projectId}`
                    );
                  }}
                >
                  <FontAwesome name="folder-open" size={16} color={Colors.primary} />
                  <Text style={styles.actionBtnSecondaryText}>Zur Zone</Text>
                </TouchableOpacity>
                <TouchableOpacity
                  style={[styles.actionBtn, styles.actionBtnPrimary]}
                  onPress={() => {
                    router.replace(
                      `/(tabs)/capture/new?projectId=${scanResult.projectId}&zoneId=${scanResult.zoneId}`
                    );
                  }}
                >
                  <FontAwesome name="plus" size={16} color={Colors.white} />
                  <Text style={styles.actionBtnPrimaryText}>
                    Neue Installation
                  </Text>
                </TouchableOpacity>
              </View>
              <TouchableOpacity
                style={styles.rescanLink}
                onPress={resetScanner}
              >
                <Text style={styles.rescanText}>Erneut scannen</Text>
              </TouchableOpacity>
            </View>
          ) : (
            <View style={styles.loadingCard}>
              <ActivityIndicator color={Colors.primary} />
              <Text style={styles.loadingText}>Zone wird geladen...</Text>
            </View>
          )}
        </View>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.black,
  },
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: Colors.background,
    padding: Spacing.xl,
  },
  permissionText: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
    textAlign: "center",
    marginBottom: Spacing.lg,
  },
  backBtn: {
    paddingVertical: Spacing.md,
  },
  backBtnText: {
    color: Colors.primary,
    fontSize: FontSize.body,
  },
  topBar: {
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    paddingTop: 56,
    paddingHorizontal: Spacing.lg,
    paddingBottom: Spacing.md,
  },
  topTitle: {
    color: Colors.white,
    fontSize: FontSize.headline,
    fontWeight: "600",
  },
  iconBtn: {
    width: 40,
    height: 40,
    justifyContent: "center",
    alignItems: "center",
  },
  resultContainer: {
    position: "absolute",
    bottom: 0,
    left: 0,
    right: 0,
    padding: Spacing.lg,
    paddingBottom: 40,
  },
  errorCard: {
    backgroundColor: Colors.card,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    alignItems: "center",
    gap: Spacing.sm,
  },
  errorText: {
    fontSize: FontSize.body,
    color: Colors.danger,
    textAlign: "center",
  },
  retryBtn: {
    backgroundColor: Colors.primary,
    paddingVertical: Spacing.sm,
    paddingHorizontal: Spacing.lg,
    borderRadius: Radius.md,
    marginTop: Spacing.sm,
  },
  retryBtnText: {
    color: Colors.white,
    fontWeight: "600",
    fontSize: FontSize.body,
  },
  successCard: {
    backgroundColor: Colors.card,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    alignItems: "center",
    gap: Spacing.xs,
  },
  successTitle: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.success,
  },
  successZone: {
    fontSize: FontSize.headline,
    fontWeight: "700",
    color: Colors.textPrimary,
  },
  successProject: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    marginBottom: Spacing.sm,
  },
  actionRow: {
    flexDirection: "row",
    gap: Spacing.sm,
  },
  actionBtn: {
    flexDirection: "row",
    alignItems: "center",
    gap: Spacing.sm,
    paddingVertical: Spacing.md,
    paddingHorizontal: Spacing.lg,
    borderRadius: Radius.md,
  },
  actionBtnPrimary: {
    backgroundColor: Colors.primary,
  },
  actionBtnPrimaryText: {
    color: Colors.white,
    fontWeight: "600",
    fontSize: FontSize.body,
  },
  actionBtnSecondary: {
    backgroundColor: Colors.background,
    borderWidth: 1,
    borderColor: Colors.primary,
  },
  actionBtnSecondaryText: {
    color: Colors.primary,
    fontWeight: "600",
    fontSize: FontSize.body,
  },
  rescanLink: {
    marginTop: Spacing.sm,
  },
  rescanText: {
    color: Colors.primary,
    fontSize: FontSize.caption,
  },
  loadingCard: {
    backgroundColor: Colors.card,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    alignItems: "center",
    gap: Spacing.sm,
  },
  loadingText: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
  },
});
