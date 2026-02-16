import React, { useState, useCallback } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  Alert,
  ScrollView,
} from "react-native";
import { FormField } from "../common/FormField";
import { Button, Headline } from "../core";
import {
  MEASUREMENT_TYPES,
  type MeasurementTypePreset,
} from "../../constants/measurementTypes";
import {
  measurementSchema,
  type MeasurementFormData,
} from "../../validation/schemas";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type MeasurementFormProps = {
  onSubmit: (data: MeasurementFormData) => Promise<void>;
  onCancel: () => void;
  submitting?: boolean;
};

export function MeasurementForm({
  onSubmit,
  onCancel,
  submitting,
}: MeasurementFormProps) {
  const [form, setForm] = useState<Record<string, string>>({
    measuredBy: "local-user",
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [showSuggestions, setShowSuggestions] = useState(false);

  const set = useCallback((key: string, value: string) => {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => {
      const next = { ...prev };
      delete next[key];
      return next;
    });
  }, []);

  const applyPreset = useCallback((preset: MeasurementTypePreset) => {
    setForm((prev) => ({
      ...prev,
      type: preset.type,
      unit: preset.unit,
      minThreshold:
        preset.minThreshold != null ? String(preset.minThreshold) : "",
      maxThreshold:
        preset.maxThreshold != null ? String(preset.maxThreshold) : "",
    }));
    setShowSuggestions(false);
    setErrors({});
  }, []);

  const filteredSuggestions = form.type
    ? MEASUREMENT_TYPES.filter((mt) =>
        mt.type.toLowerCase().includes((form.type ?? "").toLowerCase())
      )
    : MEASUREMENT_TYPES;

  const handleSubmit = useCallback(async () => {
    const cleaned: Record<string, unknown> = {};
    for (const [k, v] of Object.entries(form)) {
      if (v.trim() === "") continue;
      cleaned[k] = v;
    }

    const result = measurementSchema.safeParse(cleaned);
    if (!result.success) {
      const fieldErrors: Record<string, string> = {};
      for (const issue of result.error.issues) {
        const key = issue.path[0]?.toString();
        if (key && !fieldErrors[key]) {
          fieldErrors[key] = issue.message;
        }
      }
      setErrors(fieldErrors);
      return;
    }
    try {
      await onSubmit(result.data);
    } catch {
      Alert.alert("Fehler", "Messung konnte nicht gespeichert werden.");
    }
  }, [form, onSubmit]);

  return (
    <View style={styles.container}>
      <Headline style={styles.title}>Neue Messung</Headline>

      <View>
        <FormField
          label="Messtyp"
          required
          value={form.type ?? ""}
          onChangeText={(v) => {
            set("type", v);
            setShowSuggestions(true);
          }}
          onFocus={() => setShowSuggestions(true)}
          error={errors.type}
          placeholder="z.B. Isolationswiderstand"
        />
        {showSuggestions && filteredSuggestions.length > 0 && (
          <ScrollView
            horizontal
            style={styles.suggestions}
            showsHorizontalScrollIndicator={false}
          >
            {filteredSuggestions.map((mt) => (
              <TouchableOpacity
                key={mt.type}
                style={styles.chip}
                onPress={() => applyPreset(mt)}
              >
                <Text style={styles.chipText}>{mt.type}</Text>
              </TouchableOpacity>
            ))}
          </ScrollView>
        )}
      </View>

      <View style={styles.row}>
        <View style={styles.flex2}>
          <FormField
            label="Messwert"
            required
            value={form.value ?? ""}
            onChangeText={(v) => set("value", v)}
            error={errors.value}
            keyboardType="decimal-pad"
            placeholder="0.00"
          />
        </View>
        <View style={styles.flex1}>
          <FormField
            label="Einheit"
            required
            value={form.unit ?? ""}
            onChangeText={(v) => set("unit", v)}
            error={errors.unit}
            placeholder="Ω"
          />
        </View>
      </View>

      <View style={styles.row}>
        <View style={styles.flex1}>
          <FormField
            label="Min-Schwelle"
            value={form.minThreshold ?? ""}
            onChangeText={(v) => set("minThreshold", v)}
            keyboardType="decimal-pad"
            placeholder="—"
          />
        </View>
        <View style={styles.flex1}>
          <FormField
            label="Max-Schwelle"
            value={form.maxThreshold ?? ""}
            onChangeText={(v) => set("maxThreshold", v)}
            keyboardType="decimal-pad"
            placeholder="—"
          />
        </View>
      </View>

      <FormField
        label="Notizen"
        value={form.notes ?? ""}
        onChangeText={(v) => set("notes", v)}
        placeholder="Optionale Anmerkungen"
      />
      <FormField
        label="Prüfer"
        required
        value={form.measuredBy ?? ""}
        onChangeText={(v) => set("measuredBy", v)}
        error={errors.measuredBy}
        placeholder="Name"
      />

      <View style={styles.actions}>
        <Button
          title="Abbrechen"
          variant="secondary"
          onPress={onCancel}
          style={styles.actionButton}
        />
        <Button
          title="Speichern"
          onPress={() => void handleSubmit()}
          loading={submitting}
          style={styles.actionButton}
        />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    backgroundColor: Colors.card,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
  },
  title: {
    marginBottom: Spacing.md,
  },
  suggestions: {
    marginBottom: Spacing.sm,
    maxHeight: 36,
  },
  chip: {
    backgroundColor: Colors.background,
    borderRadius: Radius.xl,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    marginRight: Spacing.xs,
    borderWidth: 1,
    borderColor: Colors.separator,
  },
  chipText: {
    fontSize: FontSize.caption,
    color: Colors.primary,
  },
  row: {
    flexDirection: "row",
    gap: Spacing.sm,
  },
  flex1: {
    flex: 1,
  },
  flex2: {
    flex: 2,
  },
  actions: {
    flexDirection: "row",
    gap: Spacing.sm,
    marginTop: Spacing.md,
  },
  actionButton: {
    flex: 1,
  },
});
