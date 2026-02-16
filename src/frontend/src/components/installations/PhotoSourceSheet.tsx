import { Text, TouchableOpacity, StyleSheet } from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import { BottomSheet } from "../common/BottomSheet";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

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
    <BottomSheet visible={visible} onClose={onClose} title="Foto hinzufügen">
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
    </BottomSheet>
  );
}

const styles = StyleSheet.create({
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
    borderRadius: Radius.md,
  },
  cancelText: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textTertiary,
  },
});
