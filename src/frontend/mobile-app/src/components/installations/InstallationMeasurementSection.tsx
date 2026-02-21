import { View, Text, StyleSheet } from "react-native";
import { EmptyState } from "@/components/common";
import { MeasurementCard, MeasurementForm } from "@/components/installations";
import { Colors, Spacing, FontSize } from "@/styles/tokens";
import type { Measurement } from "@/db/repositories/types";
import type { MeasurementFormData } from "@/validation/schemas";

type InstallationMeasurementSectionProps = {
  measurements: Measurement[];
  showForm: boolean;
  submitting: boolean;
  onSubmit: (data: MeasurementFormData) => Promise<void>;
  onCancel: () => void;
  onDelete: (m: Measurement) => void;
};

export function InstallationMeasurementSection({
  measurements,
  showForm,
  submitting,
  onSubmit,
  onCancel,
  onDelete,
}: InstallationMeasurementSectionProps) {
  return (
    <View style={styles.measurementsSection}>
      <Text style={styles.cardTitle}>Messungen</Text>
      {showForm && (
        <View style={styles.formContainer}>
          <MeasurementForm
            onSubmit={onSubmit}
            onCancel={onCancel}
            submitting={submitting}
          />
        </View>
      )}
      {measurements.length === 0 ? (
        <EmptyState icon="bar-chart" title="Noch keine Messungen" />
      ) : (
        measurements.map((m) => (
          <MeasurementCard
            key={m.id}
            measurement={m}
            onDelete={onDelete}
          />
        ))
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginBottom: Spacing.md,
  },
  measurementsSection: {
    paddingHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
  },
  formContainer: {
    marginBottom: Spacing.md,
  },
});
