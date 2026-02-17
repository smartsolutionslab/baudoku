import React from "react";
import { View, StyleSheet, type ViewStyle } from "react-native";
import { Colors, Spacing, Radius, Shadows } from "../../styles/tokens";

type CardProps = {
  children: React.ReactNode;
  style?: ViewStyle;
};

export function Card({ children, style }: CardProps) {
  return <View style={[styles.card, style]}>{children}</View>;
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    ...Shadows.card,
  },
});
