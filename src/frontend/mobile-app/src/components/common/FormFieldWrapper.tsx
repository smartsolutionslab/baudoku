import type { ReactNode } from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { Colors, Spacing, FontSize } from '../../styles/tokens';

type FormFieldWrapperProps = {
  label: string;
  error?: string;
  required?: boolean;
  children: ReactNode;
};

export function FormFieldWrapper({ label, error, required, children }: FormFieldWrapperProps) {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>
        {label}
        {required && <Text style={styles.required}> *</Text>}
      </Text>
      {children}
      {error ? <Text style={styles.error}>{error}</Text> : null}
    </View>
  );
}

export const formFieldWrapperStyles = StyleSheet.create({
  container: {
    marginBottom: Spacing.md,
  },
  label: {
    fontSize: FontSize.caption,
    fontWeight: '600',
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  required: {
    color: Colors.danger,
  },
  error: {
    fontSize: FontSize.footnote,
    color: Colors.danger,
    marginTop: Spacing.xs,
  },
});

const styles = formFieldWrapperStyles;
