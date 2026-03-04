import { useState } from 'react';
import { Text, TouchableOpacity, FlatList, StyleSheet } from 'react-native';
import { BottomSheet } from './BottomSheet';
import { FormFieldWrapper } from './FormFieldWrapper';
import { Colors, Spacing, FontSize, Radius } from '../../styles/tokens';

type PickerOption = {
  label: string;
  value: string;
};

type FormPickerProps = {
  label: string;
  options: PickerOption[];
  value: string | null | undefined;
  onValueChange: (value: string) => void;
  error?: string;
  required?: boolean;
  placeholder?: string;
};

export function FormPicker({
  label,
  options,
  value,
  onValueChange,
  error,
  required,
  placeholder = 'Auswählen...',
}: FormPickerProps) {
  const [visible, setVisible] = useState(false);
  const selected = options.find((o) => o.value === value);

  return (
    <FormFieldWrapper label={label} error={error} required={required}>
      <TouchableOpacity
        style={[styles.picker, error && styles.pickerError]}
        onPress={() => setVisible(true)}
      >
        <Text style={selected ? styles.pickerText : styles.placeholder}>
          {selected?.label ?? placeholder}
        </Text>
        <Text style={styles.chevron}>&rsaquo;</Text>
      </TouchableOpacity>

      <BottomSheet visible={visible} onClose={() => setVisible(false)} title={label}>
        <FlatList
          data={options}
          keyExtractor={(item) => item.value}
          renderItem={({ item }) => (
            <TouchableOpacity
              style={[styles.option, item.value === value && styles.optionActive]}
              onPress={() => {
                onValueChange(item.value);
                setVisible(false);
              }}
            >
              <Text style={[styles.optionText, item.value === value && styles.optionTextActive]}>
                {item.label}
              </Text>
            </TouchableOpacity>
          )}
        />
      </BottomSheet>
    </FormFieldWrapper>
  );
}

const styles = StyleSheet.create({
  picker: {
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.md,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: Colors.separator,
  },
  pickerError: {
    borderColor: Colors.danger,
  },
  pickerText: {
    fontSize: FontSize.body,
    color: Colors.textPrimary,
  },
  placeholder: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
  chevron: {
    fontSize: 20,
    color: Colors.textTertiary,
  },
  option: {
    paddingVertical: 14,
    paddingHorizontal: Spacing.xl,
  },
  optionActive: {
    backgroundColor: Colors.optionActiveBg,
  },
  optionText: {
    fontSize: FontSize.callout,
    color: Colors.textPrimary,
  },
  optionTextActive: {
    color: Colors.primary,
    fontWeight: '600',
  },
});
