import React from "react";
import {
  View,
  Text,
  Modal,
  Pressable,
  TouchableOpacity,
  Share,
  StyleSheet,
} from "react-native";
import QRCode from "react-native-qrcode-svg";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

interface ZoneQrSheetProps {
  visible: boolean;
  onClose: () => void;
  qrValue: string;
  zoneName: string;
  zoneType: string;
}

const TYPE_LABELS: Record<string, string> = {
  building: "Gebäude",
  floor: "Stockwerk",
  room: "Raum",
  section: "Abschnitt",
  trench: "Graben",
};

export function ZoneQrSheet({
  visible,
  onClose,
  qrValue,
  zoneName,
  zoneType,
}: ZoneQrSheetProps) {
  const handleShare = async () => {
    await Share.share({
      message: `BauDoku Zone: ${zoneName}\n${qrValue}`,
    });
  };

  return (
    <Modal
      visible={visible}
      transparent
      animationType="slide"
      onRequestClose={onClose}
    >
      <Pressable style={styles.backdrop} onPress={onClose}>
        <Pressable style={styles.sheet} onPress={(e) => e.stopPropagation()}>
          <View style={styles.handle} />
          <Text style={styles.title}>{zoneName}</Text>
          <Text style={styles.subtitle}>
            {TYPE_LABELS[zoneType] ?? zoneType}
          </Text>

          <View style={styles.qrContainer}>
            <QRCode
              value={qrValue}
              size={200}
              backgroundColor="#fff"
              color="#000"
              ecl="M"
            />
          </View>

          <Text style={styles.hint}>
            QR-Code ausdrucken und an der Zone anbringen.
          </Text>

          <TouchableOpacity style={styles.shareBtn} onPress={handleShare}>
            <Text style={styles.shareBtnText}>QR-Code teilen</Text>
          </TouchableOpacity>

          <TouchableOpacity style={styles.closeBtn} onPress={onClose}>
            <Text style={styles.closeBtnText}>Schließen</Text>
          </TouchableOpacity>
        </Pressable>
      </Pressable>
    </Modal>
  );
}

const styles = StyleSheet.create({
  backdrop: {
    flex: 1,
    backgroundColor: "rgba(0,0,0,0.4)",
    justifyContent: "flex-end",
  },
  sheet: {
    backgroundColor: Colors.card,
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    padding: Spacing.xl,
    paddingBottom: 40,
    alignItems: "center",
  },
  handle: {
    width: 36,
    height: 5,
    borderRadius: 3,
    backgroundColor: Colors.separator,
    marginBottom: Spacing.lg,
  },
  title: {
    fontSize: FontSize.headline,
    fontWeight: "700",
    color: Colors.textPrimary,
  },
  subtitle: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    marginTop: Spacing.xs,
    marginBottom: Spacing.lg,
  },
  qrContainer: {
    padding: Spacing.lg,
    backgroundColor: "#fff",
    borderRadius: Radius.lg,
    marginBottom: Spacing.lg,
  },
  hint: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    textAlign: "center",
    marginBottom: Spacing.lg,
  },
  shareBtn: {
    backgroundColor: Colors.primary,
    paddingVertical: Spacing.md,
    paddingHorizontal: Spacing.xl,
    borderRadius: Radius.md,
    marginBottom: Spacing.md,
    width: "100%",
    alignItems: "center",
  },
  shareBtnText: {
    color: "#fff",
    fontWeight: "600",
    fontSize: FontSize.body,
  },
  closeBtn: {
    paddingVertical: Spacing.md,
  },
  closeBtnText: {
    color: Colors.primary,
    fontSize: FontSize.body,
  },
});
