import React, { useState } from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type CollapsibleSectionProps = {
  title: string;
  defaultOpen?: boolean;
  children: React.ReactNode;
};

export function CollapsibleSection({
  title,
  defaultOpen,
  children,
}: CollapsibleSectionProps) {
  const [open, setOpen] = useState(defaultOpen ?? false);
  return (
    <View style={styles.container}>
      <TouchableOpacity style={styles.header} onPress={() => setOpen(!open)}>
        <Text style={styles.title}>{title}</Text>
        <Text style={styles.chevron}>{open ? "\u2228" : "\u203A"}</Text>
      </TouchableOpacity>
      {open && <View style={styles.body}>{children}</View>}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    marginBottom: Spacing.md,
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
    marginBottom: Spacing.sm,
  },
  title: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textPrimary,
  },
  chevron: {
    fontSize: 18,
    color: Colors.textTertiary,
    fontWeight: "600",
  },
  body: {
    paddingLeft: Spacing.sm,
  },
});
