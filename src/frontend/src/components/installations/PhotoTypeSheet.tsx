import React from "react";
import { Text, TouchableOpacity, StyleSheet } from "react-native";
import { BottomSheet } from "../common/BottomSheet";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

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
    <BottomSheet visible={visible} onClose={onClose} title="Foto-Typ wählen">
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
    </BottomSheet>
  );
}

const styles = StyleSheet.create({
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
    borderRadius: Radius.md,
  },
  cancelText: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textTertiary,
  },
});
