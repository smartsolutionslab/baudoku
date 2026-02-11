import React, { useState, useCallback } from "react";
import {
  ScrollView,
  Text,
  TouchableOpacity,
  StyleSheet,
  Alert,
} from "react-native";
import { FormField } from "../common/FormField";
import { FormPicker } from "../common/FormPicker";
import { zoneSchema, type ZoneFormData } from "../../validation/schemas";
import { Colors, Spacing, FontSize } from "../../styles/tokens";
import type { Zone } from "../../db/repositories/types";

const typeOptions = [
  { label: "Gebäude", value: "building" },
  { label: "Stockwerk", value: "floor" },
  { label: "Raum", value: "room" },
  { label: "Graben", value: "trench" },
  { label: "Abschnitt", value: "section" },
];

interface ZoneFormProps {
  zones?: Zone[];
  defaultParentZoneId?: string | null;
  initialValues?: Partial<ZoneFormData>;
  submitLabel?: string;
  onSubmit: (data: ZoneFormData) => Promise<void>;
  submitting?: boolean;
}

export function ZoneForm({
  zones,
  defaultParentZoneId,
  initialValues,
  submitLabel,
  onSubmit,
  submitting,
}: ZoneFormProps) {
  const [form, setForm] = useState<Partial<ZoneFormData>>({
    type: "building",
    parentZoneId: defaultParentZoneId ?? null,
    ...initialValues,
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const parentOptions = [
    { label: "— Keine —", value: "__none__" },
    ...(zones?.map((z) => ({ label: z.name, value: z.id })) ?? []),
  ];

  const set = useCallback(
    <K extends keyof ZoneFormData>(key: K, value: ZoneFormData[K]) => {
      setForm((prev) => ({ ...prev, [key]: value }));
      setErrors((prev) => {
        const next = { ...prev };
        delete next[key];
        return next;
      });
    },
    []
  );

  const handleSubmit = useCallback(async () => {
    const result = zoneSchema.safeParse(form);
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
      Alert.alert("Fehler", "Zone konnte nicht gespeichert werden.");
    }
  }, [form, onSubmit]);

  return (
    <ScrollView
      style={styles.scroll}
      contentContainerStyle={styles.content}
      keyboardShouldPersistTaps="handled"
    >
      <FormField
        label="Name"
        required
        value={form.name ?? ""}
        onChangeText={(v) => set("name", v)}
        error={errors.name}
        placeholder="z.B. Hauptgebäude, 1. OG, Raum 101"
      />
      <FormPicker
        label="Typ"
        required
        options={typeOptions}
        value={form.type}
        onValueChange={(v) => set("type", v as ZoneFormData["type"])}
        error={errors.type}
      />
      <FormPicker
        label="Übergeordnete Zone"
        options={parentOptions}
        value={form.parentZoneId ?? "__none__"}
        onValueChange={(v) =>
          set("parentZoneId", v === "__none__" ? null : v)
        }
      />
      <FormField
        label="Sortierung"
        value={form.sortOrder != null ? String(form.sortOrder) : ""}
        onChangeText={(v) =>
          set("sortOrder", v ? parseInt(v, 10) || 0 : undefined)
        }
        error={errors.sortOrder}
        keyboardType="numeric"
        placeholder="0"
      />

      <TouchableOpacity
        style={[styles.button, submitting && styles.buttonDisabled]}
        onPress={() => void handleSubmit()}
        disabled={submitting}
      >
        <Text style={styles.buttonText}>
          {submitting ? "Speichert..." : (submitLabel ?? "Speichern")}
        </Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  scroll: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  content: {
    padding: Spacing.lg,
    paddingBottom: 40,
  },
  button: {
    backgroundColor: Colors.primary,
    paddingVertical: 14,
    borderRadius: 10,
    alignItems: "center",
    marginTop: Spacing.xl,
  },
  buttonDisabled: {
    backgroundColor: Colors.disabled,
  },
  buttonText: {
    color: "#fff",
    fontSize: FontSize.callout,
    fontWeight: "600",
  },
});
