import React from "react";
import { ScrollView, TouchableOpacity, Text, StyleSheet } from "react-native";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

interface FilterOption {
  label: string;
  value: string;
}

interface FilterChipsProps {
  options: FilterOption[];
  selected: string[];
  onToggle: (value: string) => void;
}

export function FilterChips({ options, selected, onToggle }: FilterChipsProps) {
  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      contentContainerStyle={styles.container}
    >
      {options.map((opt) => {
        const active = selected.includes(opt.value);
        return (
          <TouchableOpacity
            key={opt.value}
            style={[styles.chip, active && styles.chipActive]}
            onPress={() => onToggle(opt.value)}
            activeOpacity={0.7}
          >
            <Text style={[styles.chipText, active && styles.chipTextActive]}>
              {opt.label}
            </Text>
          </TouchableOpacity>
        );
      })}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: Spacing.sm,
    gap: Spacing.sm,
  },
  chip: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    borderRadius: 20,
    backgroundColor: Colors.card,
    borderWidth: 1,
    borderColor: Colors.separator,
  },
  chipActive: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  chipText: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    fontWeight: "500",
  },
  chipTextActive: {
    color: "#fff",
  },
});
