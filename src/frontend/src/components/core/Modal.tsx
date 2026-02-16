import React from "react";
import {
  Modal as RNModal,
  View,
  Text,
  TouchableOpacity,
  Pressable,
  StyleSheet,
} from "react-native";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type ModalProps = {
  visible: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
};

export function Modal({ visible, onClose, title, children, footer }: ModalProps) {
  return (
    <RNModal visible={visible} transparent animationType="slide">
      <Pressable style={styles.overlay} onPress={onClose}>
        <Pressable style={styles.sheet} onPress={(e) => e.stopPropagation()}>
          <View style={styles.handle} />

          {title ? <Text style={styles.title}>{title}</Text> : null}

          <View style={styles.body}>{children}</View>

          {footer ? (
            <View style={styles.footer}>{footer}</View>
          ) : (
            <TouchableOpacity style={styles.closeButton} onPress={onClose}>
              <Text style={styles.closeText}>Schlie\u00dfen</Text>
            </TouchableOpacity>
          )}
        </Pressable>
      </Pressable>
    </RNModal>
  );
}

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: Colors.overlay,
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
  body: {
    marginBottom: Spacing.md,
  },
  footer: {
    marginTop: Spacing.sm,
  },
  closeButton: {
    alignItems: "center",
    paddingVertical: 14,
    marginTop: Spacing.lg,
    backgroundColor: Colors.background,
    borderRadius: Radius.md,
  },
  closeText: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textTertiary,
  },
});
