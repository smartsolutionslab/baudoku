import React from "react";
import { TouchableOpacity, Text, StyleSheet } from "react-native";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import { Colors } from "../../styles/tokens";

interface FloatingActionButtonProps {
  icon?: React.ComponentProps<typeof FontAwesome>["name"];
  label?: string;
  onPress: () => void;
}

export function FloatingActionButton({
  icon = "plus",
  label,
  onPress,
}: FloatingActionButtonProps) {
  return (
    <TouchableOpacity style={styles.fab} onPress={onPress} activeOpacity={0.8}>
      <FontAwesome name={icon} size={20} color="#fff" />
      {label ? <Text style={styles.label}>{label}</Text> : null}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  fab: {
    position: "absolute",
    right: 20,
    bottom: 24,
    backgroundColor: Colors.primary,
    borderRadius: 28,
    height: 56,
    minWidth: 56,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    paddingHorizontal: 16,
    gap: 8,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    elevation: 5,
  },
  label: {
    color: "#fff",
    fontSize: 15,
    fontWeight: "600",
  },
});
