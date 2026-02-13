import React from "react";
import {
  TouchableOpacity,
  Text,
  ActivityIndicator,
  StyleSheet,
  type ViewStyle,
  type TextStyle,
} from "react-native";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

type ButtonVariant = "primary" | "secondary" | "danger";

type ButtonProps = {
  title: string;
  onPress: () => void;
  variant?: ButtonVariant;
  disabled?: boolean;
  loading?: boolean;
  style?: ViewStyle;
};

const variantStyles: Record<ButtonVariant, { container: ViewStyle; text: TextStyle }> = {
  primary: {
    container: { backgroundColor: Colors.primary },
    text: { color: "#fff" },
  },
  secondary: {
    container: { backgroundColor: Colors.background },
    text: { color: Colors.textTertiary },
  },
  danger: {
    container: { backgroundColor: Colors.danger },
    text: { color: "#fff" },
  },
};

export function Button({
  title,
  onPress,
  variant = "primary",
  disabled = false,
  loading = false,
  style,
}: ButtonProps) {
  const v = variantStyles[variant];
  const isDisabled = disabled || loading;

  return (
    <TouchableOpacity
      style={[
        styles.container,
        v.container,
        isDisabled && styles.disabled,
        style,
      ]}
      onPress={onPress}
      disabled={isDisabled}
      activeOpacity={0.7}
    >
      {loading ? (
        <ActivityIndicator
          size="small"
          color={variant === "secondary" ? Colors.textTertiary : "#fff"}
        />
      ) : (
        <Text style={[styles.text, v.text]}>{title}</Text>
      )}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingVertical: 14,
    borderRadius: 10,
    alignItems: "center",
    justifyContent: "center",
    minHeight: 48,
  },
  disabled: {
    backgroundColor: Colors.disabled,
  },
  text: {
    fontSize: FontSize.callout,
    fontWeight: "600",
  },
});
