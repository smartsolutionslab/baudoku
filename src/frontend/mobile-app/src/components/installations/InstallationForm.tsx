import { Text, ScrollView, StyleSheet } from 'react-native';
import { FormField, FormPicker, CollapsibleSection } from '../common';
import { Button } from '../core';
import { GpsButton } from './GpsButton';
import { useGpsCapture, type GpsPosition, useInstallationForm } from '../../hooks';
import { PHASE_OPTIONS } from '@baudoku/documentation';
import type { InstallationFormData } from '../../validation/schemas';
import { Colors, Spacing, FontSize } from '../../styles/tokens';
import { INSTALLATION_STATUS_OPTIONS } from '../../constants';

type InstallationFormProps = {
  onSubmit: (data: InstallationFormData, gps: GpsPosition | null) => Promise<void>;
  submitting?: boolean;
  initialValues?: Partial<InstallationFormData>;
  initialGps?: GpsPosition | null;
  submitLabel?: string;
};

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
        value={str('type')}
        onChangeText={(v) => set('type', v)}
        error={errors.type}
        placeholder="z.B. Kabelschacht, Steckdose, Muffe"
      />
      <FormPicker
        label="Status"
        required
        options={INSTALLATION_STATUS_OPTIONS}
        value={str('status') || 'in_progress'}
        onValueChange={(v) => set('status', v)}
        error={errors.status}
      />

      <CollapsibleSection title="Komponente" defaultOpen={hasComponentValues}>
        <FormField
          label="Hersteller"
          value={str('manufacturer')}
          onChangeText={(v) => set('manufacturer', v)}
          placeholder="Hersteller"
        />
        <FormField
          label="Modell"
          value={str('model')}
          onChangeText={(v) => set('model', v)}
          placeholder="Modell"
        />
        <FormField
          label="Seriennummer"
          value={str('serialNumber')}
          onChangeText={(v) => set('serialNumber', v)}
          placeholder="Seriennummer"
        />
      </CollapsibleSection>

      <CollapsibleSection title="Kabel" defaultOpen={hasCableValues}>
        <FormField
          label="Kabeltyp"
          value={str('cableType')}
          onChangeText={(v) => set('cableType', v)}
          placeholder="z.B. NYY-J 5x16"
        />
        <FormField
          label="Querschnitt"
          value={str('crossSectionMm2')}
          onChangeText={(v) => set('crossSectionMm2', v)}
          keyboardType="decimal-pad"
          placeholder="16"
          suffix="mm²"
        />
        <FormField
          label="Länge"
          value={str('lengthM')}
          onChangeText={(v) => set('lengthM', v)}
          keyboardType="decimal-pad"
          placeholder="25"
          suffix="m"
        />
      </CollapsibleSection>

      <CollapsibleSection title="Elektrik" defaultOpen={hasElectricalValues}>
        <FormField
          label="Stromkreis"
          value={str('circuitId')}
          onChangeText={(v) => set('circuitId', v)}
          placeholder="SK-01"
        />
        <FormField
          label="Sicherungstyp"
          value={str('fuseType')}
          onChangeText={(v) => set('fuseType', v)}
          placeholder="B16"
        />
        <FormField
          label="Nennstrom"
          value={str('fuseRatingA')}
          onChangeText={(v) => set('fuseRatingA', v)}
          keyboardType="decimal-pad"
          placeholder="16"
          suffix="A"
        />
        <FormField
          label="Spannung"
          value={str('voltageV')}
          onChangeText={(v) => set('voltageV', v)}
          keyboardType="numeric"
          placeholder="230"
          suffix="V"
        />
        <FormPicker
          label="Phase"
          options={PHASE_OPTIONS}
          value={(form.phase as string) ?? null}
          onValueChange={(v) => set('phase', v)}
          placeholder="Phase w\u00E4hlen..."
        />
      </CollapsibleSection>

      <Text style={styles.sectionTitle}>Weitere Angaben</Text>
      <FormField
        label="Verlegetiefe"
        value={str('depthMm')}
        onChangeText={(v) => set('depthMm', v)}
        keyboardType="numeric"
        placeholder="600"
        suffix="mm"
      />
      <FormField
        label="Notizen"
        value={str('notes')}
        onChangeText={(v) => set('notes', v)}
        placeholder="Zus\u00E4tzliche Informationen..."
        multiline
        numberOfLines={3}
        style={{ minHeight: 80, textAlignVertical: 'top' }}
      />

      <GpsButton
        position={currentGps}
        capturing={gps.capturing}
        error={gps.error}
        onCapture={gps.capturePosition}
        onClear={gps.clearPosition}
      />

      <Button
        title={submitting ? 'Speichert...' : (submitLabel ?? 'Speichern')}
        onPress={() => void handleSubmit(currentGps)}
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
    paddingBottom: Spacing.xxl,
  },
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: '600',
    color: Colors.textPrimary,
    marginTop: Spacing.lg,
    marginBottom: Spacing.md,
  },
  button: {
    marginTop: Spacing.md,
  },
});
