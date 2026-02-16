import { ScrollView, StyleSheet } from "react-native";
import { FormField, FormPicker } from "../common";
import { Button } from "../core";
import { useZoneForm } from "../../hooks";
import type { ZoneFormData } from "../../validation/schemas";
import { Colors, Spacing } from "../../styles/tokens";
import type { Zone } from "../../db/repositories/types";

const typeOptions = [
  { label: "Gebäude", value: "building" },
  { label: "Stockwerk", value: "floor" },
  { label: "Raum", value: "room" },
  { label: "Graben", value: "trench" },
  { label: "Abschnitt", value: "section" },
];

type ZoneFormProps = {
  zones?: Zone[];
  defaultParentZoneId?: string | null;
  initialValues?: Partial<ZoneFormData>;
  submitLabel?: string;
  onSubmit: (data: ZoneFormData) => Promise<void>;
  submitting?: boolean;
};

export function ZoneForm({
  zones,
  defaultParentZoneId,
  initialValues,
  submitLabel,
  onSubmit,
  submitting,
}: ZoneFormProps) {
  const { form, errors, set, handleSubmit } = useZoneForm({
    initialValues,
    defaultParentZoneId,
    onSubmit,
  });

  const parentOptions = [
    { label: "\u2014 Keine \u2014", value: "__none__" },
    ...(zones?.map((z) => ({ label: z.name, value: z.id })) ?? []),
  ];

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

      <Button
        title={submitting ? "Speichert..." : (submitLabel ?? "Speichern")}
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
  button: {
    marginTop: Spacing.xl,
  },
});
