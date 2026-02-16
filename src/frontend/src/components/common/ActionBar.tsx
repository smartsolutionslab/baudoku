import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

export type ActionBarItem = {
  icon: React.ComponentProps<typeof FontAwesome>["name"];
  label: string;
  onPress: () => void;
  color?: string;
};

type ActionBarProps = {
  actions: ActionBarItem[];
};

export function ActionBar({ actions }: ActionBarProps) {
  return (
    <View style={styles.container}>
      {actions.map((action) => (
        <TouchableOpacity
          key={action.label}
          style={styles.item}
          onPress={action.onPress}
          activeOpacity={0.7}
        >
          <FontAwesome
            name={action.icon}
            size={20}
            color={action.color ?? Colors.primary}
          />
          <Text
            style={[
              styles.label,
              action.color ? { color: action.color } : undefined,
            ]}
          >
            {action.label}
          </Text>
        </TouchableOpacity>
      ))}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.sm,
    borderRadius: Radius.lg,
    paddingVertical: Spacing.md,
    paddingHorizontal: Spacing.sm,
  },
  item: {
    flex: 1,
    alignItems: "center",
    gap: Spacing.xs,
  },
  label: {
    fontSize: FontSize.footnote,
    color: Colors.primary,
    fontWeight: "500",
  },
});
