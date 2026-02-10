import React, { useState, useCallback } from "react";
import {
  View,
  Text,
  ScrollView,
  TouchableOpacity,
  StyleSheet,
  Alert,
} from "react-native";
import { FormField } from "../common/FormField";
import { FormPicker } from "../common/FormPicker";
import { projectSchema, type ProjectFormData } from "../../validation/schemas";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

const statusOptions = [
  { label: "Aktiv", value: "active" },
  { label: "Abgeschlossen", value: "completed" },
  { label: "Archiviert", value: "archived" },
];

interface ProjectFormProps {
  onSubmit: (data: ProjectFormData) => Promise<void>;
  submitting?: boolean;
}

export function ProjectForm({ onSubmit, submitting }: ProjectFormProps) {
  const [form, setForm] = useState<Partial<ProjectFormData>>({
    status: "active",
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const set = useCallback(
    <K extends keyof ProjectFormData>(key: K, value: ProjectFormData[K]) => {
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
    const result = projectSchema.safeParse(form);
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
      Alert.alert("Fehler", "Projekt konnte nicht gespeichert werden.");
    }
  }, [form, onSubmit]);

  return (
    <ScrollView
      style={styles.scroll}
      contentContainerStyle={styles.content}
      keyboardShouldPersistTaps="handled"
    >
      <Text style={styles.sectionTitle}>Projekt</Text>
      <FormField
        label="Name"
        required
        value={form.name ?? ""}
        onChangeText={(v) => set("name", v)}
        error={errors.name}
        placeholder="Projektname"
      />
      <FormPicker
        label="Status"
        required
        options={statusOptions}
        value={form.status}
        onValueChange={(v) => set("status", v as ProjectFormData["status"])}
        error={errors.status}
      />

      <Text style={styles.sectionTitle}>Adresse</Text>
      <FormField
        label="Straße"
        value={form.street ?? ""}
        onChangeText={(v) => set("street", v)}
        error={errors.street}
        placeholder="Musterstraße 1"
      />
      <FormField
        label="PLZ"
        value={form.zipCode ?? ""}
        onChangeText={(v) => set("zipCode", v)}
        error={errors.zipCode}
        placeholder="12345"
        keyboardType="numeric"
      />
      <FormField
        label="Stadt"
        value={form.city ?? ""}
        onChangeText={(v) => set("city", v)}
        error={errors.city}
        placeholder="Berlin"
      />

      <Text style={styles.sectionTitle}>Auftraggeber</Text>
      <FormField
        label="Name"
        value={form.clientName ?? ""}
        onChangeText={(v) => set("clientName", v)}
        error={errors.clientName}
        placeholder="Firma / Person"
      />
      <FormField
        label="Kontakt"
        value={form.clientContact ?? ""}
        onChangeText={(v) => set("clientContact", v)}
        error={errors.clientContact}
        placeholder="Tel. oder E-Mail"
      />

      <TouchableOpacity
        style={[styles.button, submitting && styles.buttonDisabled]}
        onPress={() => void handleSubmit()}
        disabled={submitting}
      >
        <Text style={styles.buttonText}>
          {submitting ? "Speichert..." : "Speichern"}
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
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    color: Colors.textPrimary,
    marginTop: Spacing.lg,
    marginBottom: Spacing.md,
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
