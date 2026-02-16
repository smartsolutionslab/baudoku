import React from "react";
import { TouchableOpacity, Text, StyleSheet } from "react-native";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import { Colors, Spacing, FontSize, Shadows } from "../../styles/tokens";

type FloatingActionButtonProps = {
  icon?: React.ComponentProps<typeof FontAwesome>["name"];
  label?: string;
  onPress: () => void;
  testID?: string;
};

export function FloatingActionButton({
  icon = "plus",
  label,
  onPress,
  testID,
}: FloatingActionButtonProps) {
  return (
    <TouchableOpacity style={styles.fab} onPress={onPress} activeOpacity={0.8} testID={testID}>
      <FontAwesome name={icon} size={20} color={Colors.white} />
      {label ? <Text style={styles.label}>{label}</Text> : null}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  fab: {
    position: "absolute",
    right: 20,
    bottom: Spacing.xl,
    backgroundColor: Colors.primary,
    borderRadius: 28,
    height: 56,
    minWidth: 56,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    paddingHorizontal: Spacing.lg,
    gap: Spacing.sm,
    ...Shadows.elevated,
  },
  label: {
    color: Colors.white,
    fontSize: FontSize.body,
    fontWeight: "600",
  },
});
