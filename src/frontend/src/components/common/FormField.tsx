import React from "react";
import { View, Text, TextInput, StyleSheet, type TextInputProps } from "react-native";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type FormFieldProps = TextInputProps & {
  label: string;
  error?: string;
  required?: boolean;
};

export function FormField({ label, error, required, style, ...props }: FormFieldProps) {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>
        {label}
        {required && <Text style={styles.required}> *</Text>}
      </Text>
      <TextInput
        style={[styles.input, error && styles.inputError, style]}
        placeholderTextColor={Colors.textTertiary}
        {...props}
      />
      {error ? <Text style={styles.error}>{error}</Text> : null}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    marginBottom: Spacing.md,
  },
  label: {
    fontSize: FontSize.caption,
    fontWeight: "600",
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  required: {
    color: Colors.danger,
  },
  input: {
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.md,
    fontSize: FontSize.body,
    color: Colors.textPrimary,
    borderWidth: 1,
    borderColor: Colors.separator,
  },
  inputError: {
    borderColor: Colors.danger,
  },
  error: {
    fontSize: FontSize.footnote,
    color: Colors.danger,
    marginTop: Spacing.xs,
  },
});
