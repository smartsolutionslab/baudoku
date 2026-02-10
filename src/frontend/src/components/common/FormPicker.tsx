import React, { useState } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  Modal,
  FlatList,
  StyleSheet,
} from "react-native";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

interface PickerOption {
  label: string;
  value: string;
}

interface FormPickerProps {
  label: string;
  options: PickerOption[];
  value: string | null | undefined;
  onValueChange: (value: string) => void;
  error?: string;
  required?: boolean;
  placeholder?: string;
}

export function FormPicker({
  label,
  options,
  value,
  onValueChange,
  error,
  required,
  placeholder = "Auswählen...",
}: FormPickerProps) {
  const [visible, setVisible] = useState(false);
  const selected = options.find((o) => o.value === value);

  return (
    <View style={styles.container}>
      <Text style={styles.label}>
        {label}
        {required && <Text style={styles.required}> *</Text>}
      </Text>
      <TouchableOpacity
        style={[styles.picker, error && styles.pickerError]}
        onPress={() => setVisible(true)}
      >
        <Text style={selected ? styles.pickerText : styles.placeholder}>
          {selected?.label ?? placeholder}
        </Text>
        <Text style={styles.chevron}>›</Text>
      </TouchableOpacity>
      {error ? <Text style={styles.error}>{error}</Text> : null}

      <Modal visible={visible} transparent animationType="slide">
        <TouchableOpacity
          style={styles.overlay}
          activeOpacity={1}
          onPress={() => setVisible(false)}
        >
          <View style={styles.sheet}>
            <Text style={styles.sheetTitle}>{label}</Text>
            <FlatList
              data={options}
              keyExtractor={(item) => item.value}
              renderItem={({ item }) => (
                <TouchableOpacity
                  style={[
                    styles.option,
                    item.value === value && styles.optionActive,
                  ]}
                  onPress={() => {
                    onValueChange(item.value);
                    setVisible(false);
                  }}
                >
                  <Text
                    style={[
                      styles.optionText,
                      item.value === value && styles.optionTextActive,
                    ]}
                  >
                    {item.label}
                  </Text>
                </TouchableOpacity>
              )}
            />
          </View>
        </TouchableOpacity>
      </Modal>
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
  picker: {
    backgroundColor: Colors.card,
    borderRadius: 10,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.md,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
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
  error: {
    fontSize: FontSize.footnote,
    color: Colors.danger,
    marginTop: Spacing.xs,
  },
  overlay: {
    flex: 1,
    justifyContent: "flex-end",
    backgroundColor: "rgba(0,0,0,0.3)",
  },
  sheet: {
    backgroundColor: Colors.card,
    borderTopLeftRadius: 16,
    borderTopRightRadius: 16,
    paddingTop: Spacing.lg,
    paddingBottom: 34,
    maxHeight: "50%",
  },
  sheetTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    textAlign: "center",
    marginBottom: Spacing.lg,
  },
  option: {
    paddingVertical: 14,
    paddingHorizontal: Spacing.xl,
  },
  optionActive: {
    backgroundColor: "#E8F0FE",
  },
  optionText: {
    fontSize: FontSize.callout,
    color: Colors.textPrimary,
  },
  optionTextActive: {
    color: Colors.primary,
    fontWeight: "600",
  },
});
