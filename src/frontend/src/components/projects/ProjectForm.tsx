import React from "react";
import {
  Text,
  ScrollView,
  StyleSheet,
} from "react-native";
import { FormField } from "../common/FormField";
import { FormPicker } from "../common/FormPicker";
import { Button } from "../core";
import { useProjectForm } from "../../hooks/useProjectForm";
import type { ProjectFormData } from "../../validation/schemas";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

const statusOptions = [
  { label: "Aktiv", value: "active" },
  { label: "Abgeschlossen", value: "completed" },
  { label: "Archiviert", value: "archived" },
];

type ProjectFormProps = {
  onSubmit: (data: ProjectFormData) => Promise<void>;
  submitting?: boolean;
  initialValues?: Partial<ProjectFormData>;
  submitLabel?: string;
};

export function ProjectForm({
  onSubmit,
  submitting,
  initialValues,
  submitLabel,
}: ProjectFormProps) {
  const { form, errors, set, handleSubmit } = useProjectForm({
    initialValues,
    onSubmit,
  });

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

      <Button
        title={submitLabel ?? "Speichern"}
        onPress={() => void handleSubmit()}
        loading={submitting}
        style={styles.button}
      />
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
    marginTop: Spacing.xl,
  },
});
