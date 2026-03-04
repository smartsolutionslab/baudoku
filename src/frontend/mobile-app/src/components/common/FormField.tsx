import { View, Text, TextInput, StyleSheet, type TextInputProps } from 'react-native';
import { Colors, Spacing, FontSize, Radius } from '../../styles/tokens';

type FormFieldProps = TextInputProps & {
  label: string;
  error?: string;
  required?: boolean;
  suffix?: string;
};

export function FormField({ label, error, required, suffix, style, ...props }: FormFieldProps) {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>
        {label}
        {required && <Text style={styles.required}> *</Text>}
      </Text>
      <View style={styles.inputWrapper}>
        <TextInput
          style={[
            styles.input,
            suffix && styles.inputWithSuffix,
            error && styles.inputError,
            style,
          ]}
          placeholderTextColor={Colors.textTertiary}
          {...props}
        />
        {suffix && <Text style={styles.suffix}>{suffix}</Text>}
      </View>
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
    fontWeight: '600',
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  required: {
    color: Colors.danger,
  },
  inputWrapper: {
    position: 'relative' as const,
    justifyContent: 'center' as const,
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
  inputWithSuffix: {
    paddingRight: Spacing.md + 40,
  },
  inputError: {
    borderColor: Colors.danger,
  },
  suffix: {
    position: 'absolute' as const,
    right: Spacing.md,
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
  error: {
    fontSize: FontSize.footnote,
    color: Colors.danger,
    marginTop: Spacing.xs,
  },
});
