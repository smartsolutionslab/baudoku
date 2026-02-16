import { View, Text, TouchableOpacity, Share, StyleSheet } from "react-native";
import QRCode from "react-native-qrcode-svg";
import { BottomSheet, statusLabels } from "../common";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type ZoneQrSheetProps = {
  visible: boolean;
  onClose: () => void;
  qrValue: string;
  zoneName: string;
  zoneType: string;
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
    <BottomSheet visible={visible} onClose={onClose}>
      <View style={styles.centered}>
        <Text style={styles.title}>{zoneName}</Text>
        <Text style={styles.subtitle}>
          {statusLabels[zoneType] ?? zoneType}
        </Text>

        <View style={styles.qrContainer}>
          <QRCode
            value={qrValue}
            size={200}
            backgroundColor={Colors.white}
            color={Colors.black}
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
      </View>
    </BottomSheet>
  );
}

const styles = StyleSheet.create({
  centered: {
    alignItems: "center",
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
    backgroundColor: Colors.white,
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
    color: Colors.white,
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
