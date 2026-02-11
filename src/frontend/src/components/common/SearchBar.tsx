import React from "react";
import { View, TextInput, TouchableOpacity, StyleSheet } from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

interface SearchBarProps {
  value: string;
  onChangeText: (text: string) => void;
  placeholder?: string;
  autoFocus?: boolean;
}

export function SearchBar({
  value,
  onChangeText,
  placeholder = "Suchen...",
  autoFocus = true,
}: SearchBarProps) {
  return (
    <View style={styles.container}>
      <FontAwesome
        name="search"
        size={16}
        color={Colors.textTertiary}
        style={styles.icon}
      />
      <TextInput
        style={styles.input}
        value={value}
        onChangeText={onChangeText}
        placeholder={placeholder}
        placeholderTextColor={Colors.textTertiary}
        autoFocus={autoFocus}
        autoCorrect={false}
        clearButtonMode="while-editing"
      />
      {value.length > 0 && (
        <TouchableOpacity onPress={() => onChangeText("")} style={styles.clear}>
          <FontAwesome name="times-circle" size={18} color={Colors.textTertiary} />
        </TouchableOpacity>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    paddingHorizontal: Spacing.md,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  icon: {
    marginRight: Spacing.sm,
  },
  input: {
    flex: 1,
    paddingVertical: 10,
    fontSize: FontSize.body,
    color: Colors.textPrimary,
  },
  clear: {
    padding: Spacing.xs,
  },
});
