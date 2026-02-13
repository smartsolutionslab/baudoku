import React from "react";
import {
  View,
  Text,
  TouchableOpacity,
  Modal,
  StyleSheet,
  Pressable,
} from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

type PhotoSourceSheetProps = {
  visible: boolean;
  onCamera: () => void;
  onGallery: () => void;
  onClose: () => void;
};

export function PhotoSourceSheet({
  visible,
  onCamera,
  onGallery,
  onClose,
}: PhotoSourceSheetProps) {
  return (
    <Modal visible={visible} transparent animationType="slide">
      <Pressable style={styles.overlay} onPress={onClose}>
        <Pressable style={styles.sheet} onPress={(e) => e.stopPropagation()}>
          <View style={styles.handle} />
          <Text style={styles.title}>Foto hinzufügen</Text>

          <TouchableOpacity style={styles.option} onPress={onCamera}>
            <FontAwesome
              name="camera"
              size={20}
              color={Colors.primary}
              style={styles.icon}
            />
            <Text style={styles.optionText}>Foto aufnehmen</Text>
          </TouchableOpacity>

          <TouchableOpacity style={styles.option} onPress={onGallery}>
            <FontAwesome
              name="image"
              size={20}
              color={Colors.primary}
              style={styles.icon}
            />
            <Text style={styles.optionText}>Aus Galerie wählen</Text>
          </TouchableOpacity>

          <TouchableOpacity style={styles.cancelButton} onPress={onClose}>
            <Text style={styles.cancelText}>Abbrechen</Text>
          </TouchableOpacity>
        </Pressable>
      </Pressable>
    </Modal>
  );
}

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: "rgba(0,0,0,0.4)",
    justifyContent: "flex-end",
  },
  sheet: {
    backgroundColor: Colors.card,
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    paddingHorizontal: Spacing.lg,
    paddingBottom: 40,
  },
  handle: {
    width: 36,
    height: 4,
    borderRadius: 2,
    backgroundColor: Colors.separator,
    alignSelf: "center",
    marginTop: Spacing.sm,
    marginBottom: Spacing.lg,
  },
  title: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    color: Colors.textPrimary,
    marginBottom: Spacing.lg,
  },
  option: {
    flexDirection: "row",
    alignItems: "center",
    paddingVertical: Spacing.md,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: Colors.separator,
  },
  icon: {
    width: 28,
  },
  optionText: {
    fontSize: FontSize.callout,
    color: Colors.textPrimary,
    marginLeft: Spacing.sm,
  },
  cancelButton: {
    alignItems: "center",
    paddingVertical: 14,
    marginTop: Spacing.lg,
    backgroundColor: Colors.background,
    borderRadius: 10,
  },
  cancelText: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textTertiary,
  },
});
