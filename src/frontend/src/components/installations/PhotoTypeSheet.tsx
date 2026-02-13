import React from "react";
import {
  View,
  Text,
  TouchableOpacity,
  Modal,
  StyleSheet,
  Pressable,
} from "react-native";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

const PHOTO_TYPES = [
  { value: "before", label: "Vorher" },
  { value: "after", label: "Nachher" },
  { value: "detail", label: "Detail" },
  { value: "overview", label: "Übersicht" },
] as const;

export type PhotoType = (typeof PHOTO_TYPES)[number]["value"];

type PhotoTypeSheetProps = {
  visible: boolean;
  onSelect: (type: PhotoType) => void;
  onClose: () => void;
};

export function PhotoTypeSheet({
  visible,
  onSelect,
  onClose,
}: PhotoTypeSheetProps) {
  return (
    <Modal visible={visible} transparent animationType="slide">
      <Pressable style={styles.overlay} onPress={onClose}>
        <Pressable style={styles.sheet} onPress={(e) => e.stopPropagation()}>
          <View style={styles.handle} />
          <Text style={styles.title}>Foto-Typ wählen</Text>

          {PHOTO_TYPES.map((pt) => (
            <TouchableOpacity
              key={pt.value}
              style={styles.option}
              onPress={() => onSelect(pt.value)}
            >
              <Text style={styles.optionText}>{pt.label}</Text>
            </TouchableOpacity>
          ))}

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
    paddingVertical: Spacing.md,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: Colors.separator,
  },
  optionText: {
    fontSize: FontSize.callout,
    color: Colors.textPrimary,
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
