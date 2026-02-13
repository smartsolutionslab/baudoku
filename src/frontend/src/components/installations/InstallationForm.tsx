import React, { useState } from "react";
import {
  View,
  Text,
  ScrollView,
  TouchableOpacity,
  StyleSheet,
} from "react-native";
import { FormField } from "../common/FormField";
import { FormPicker } from "../common/FormPicker";
import { GpsButton } from "./GpsButton";
import { useGpsCapture, type GpsPosition } from "../../hooks/useGpsCapture";
import { useInstallationForm } from "../../hooks/useInstallationForm";
import type { InstallationFormData } from "../../validation/schemas";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

const statusOptions = [
  { label: "Geplant", value: "planned" },
  { label: "In Arbeit", value: "in_progress" },
  { label: "Abgeschlossen", value: "completed" },
  { label: "Geprüft", value: "inspected" },
];

const phaseOptions = [
  { label: "L1", value: "L1" },
  { label: "L2", value: "L2" },
  { label: "L3", value: "L3" },
  { label: "N", value: "N" },
  { label: "PE", value: "PE" },
];

type InstallationFormProps = {
  onSubmit: (data: InstallationFormData, gps: GpsPosition | null) => Promise<void>;
  submitting?: boolean;
  initialValues?: Partial<InstallationFormData>;
  initialGps?: GpsPosition | null;
  submitLabel?: string;
};

function CollapsibleSection({
  title,
  defaultOpen,
  children,
}: {
  title: string;
  defaultOpen?: boolean;
  children: React.ReactNode;
}) {
  const [open, setOpen] = useState(defaultOpen ?? false);
  return (
    <View style={sectionStyles.container}>
      <TouchableOpacity
        style={sectionStyles.header}
        onPress={() => setOpen(!open)}
      >
        <Text style={sectionStyles.title}>{title}</Text>
        <Text style={sectionStyles.chevron}>{open ? "\u2228" : "\u203A"}</Text>
      </TouchableOpacity>
      {open && <View style={sectionStyles.body}>{children}</View>}
    </View>
  );
}

export function InstallationForm({
  onSubmit,
  submitting,
  initialValues,
  initialGps,
  submitLabel,
}: InstallationFormProps) {
  const {
    form,
    errors,
    set,
    str,
    handleSubmit,
    hasComponentValues,
    hasCableValues,
    hasElectricalValues,
  } = useInstallationForm({ initialValues, onSubmit });

  const gps = useGpsCapture();

  // Use initialGps if provided and no new capture has been done
  const currentGps = gps.position ?? initialGps ?? null;

  return (
    <ScrollView
      style={styles.scroll}
      contentContainerStyle={styles.content}
      keyboardShouldPersistTaps="handled"
    >
      <Text style={styles.sectionTitle}>Typ & Status</Text>
      <FormField
        label="Typ"
        required
        value={str("type")}
        onChangeText={(v) => set("type", v)}
        error={errors.type}
        placeholder="z.B. Kabelschacht, Steckdose, Muffe"
      />
      <FormPicker
        label="Status"
        required
        options={statusOptions}
        value={str("status") || "in_progress"}
        onValueChange={(v) => set("status", v)}
        error={errors.status}
      />

      <CollapsibleSection title="Komponente" defaultOpen={hasComponentValues}>
        <FormField
          label="Hersteller"
          value={str("manufacturer")}
          onChangeText={(v) => set("manufacturer", v)}
          placeholder="Hersteller"
        />
        <FormField
          label="Modell"
          value={str("model")}
          onChangeText={(v) => set("model", v)}
          placeholder="Modell"
        />
        <FormField
          label="Seriennummer"
          value={str("serialNumber")}
          onChangeText={(v) => set("serialNumber", v)}
          placeholder="Seriennummer"
        />
      </CollapsibleSection>

      <CollapsibleSection title="Kabel" defaultOpen={hasCableValues}>
        <FormField
          label="Kabeltyp"
          value={str("cableType")}
          onChangeText={(v) => set("cableType", v)}
          placeholder="z.B. NYY-J 5x16"
        />
        <FormField
          label="Querschnitt (mm\u00B2)"
          value={str("crossSectionMm2")}
          onChangeText={(v) => set("crossSectionMm2", v)}
          keyboardType="decimal-pad"
          placeholder="16"
        />
        <FormField
          label="L\u00E4nge (m)"
          value={str("lengthM")}
          onChangeText={(v) => set("lengthM", v)}
          keyboardType="decimal-pad"
          placeholder="25"
        />
      </CollapsibleSection>

      <CollapsibleSection title="Elektrik" defaultOpen={hasElectricalValues}>
        <FormField
          label="Stromkreis"
          value={str("circuitId")}
          onChangeText={(v) => set("circuitId", v)}
          placeholder="SK-01"
        />
        <FormField
          label="Sicherungstyp"
          value={str("fuseType")}
          onChangeText={(v) => set("fuseType", v)}
          placeholder="B16"
        />
        <FormField
          label="Nennstrom (A)"
          value={str("fuseRatingA")}
          onChangeText={(v) => set("fuseRatingA", v)}
          keyboardType="decimal-pad"
          placeholder="16"
        />
        <FormField
          label="Spannung (V)"
          value={str("voltageV")}
          onChangeText={(v) => set("voltageV", v)}
          keyboardType="numeric"
          placeholder="230"
        />
        <FormPicker
          label="Phase"
          options={phaseOptions}
          value={(form.phase as string) ?? null}
          onValueChange={(v) => set("phase", v)}
          placeholder="Phase w\u00E4hlen..."
        />
      </CollapsibleSection>

      <Text style={styles.sectionTitle}>Weitere Angaben</Text>
      <FormField
        label="Verlegetiefe (mm)"
        value={str("depthMm")}
        onChangeText={(v) => set("depthMm", v)}
        keyboardType="numeric"
        placeholder="600"
      />
      <FormField
        label="Notizen"
        value={str("notes")}
        onChangeText={(v) => set("notes", v)}
        placeholder="Zus\u00E4tzliche Informationen..."
        multiline
        numberOfLines={3}
        style={{ minHeight: 80, textAlignVertical: "top" }}
      />

      <GpsButton
        position={currentGps}
        capturing={gps.capturing}
        error={gps.error}
        onCapture={gps.capturePosition}
        onClear={gps.clearPosition}
      />

      <TouchableOpacity
        style={[styles.button, submitting && styles.buttonDisabled]}
        onPress={() => void handleSubmit(currentGps)}
        disabled={submitting}
      >
        <Text style={styles.buttonText}>
          {submitting
            ? "Speichert..."
            : submitLabel ?? "Speichern"}
        </Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const sectionStyles = StyleSheet.create({
  container: {
    marginBottom: Spacing.md,
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    backgroundColor: Colors.card,
    borderRadius: 10,
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
    marginBottom: Spacing.sm,
  },
  title: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textPrimary,
  },
  chevron: {
    fontSize: 18,
    color: Colors.textTertiary,
    fontWeight: "600",
  },
  body: {
    paddingLeft: Spacing.sm,
  },
});

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
    marginTop: Spacing.md,
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
