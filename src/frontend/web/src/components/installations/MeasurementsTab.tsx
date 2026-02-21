import { useState } from "react";
import { MeasurementCard } from "./MeasurementCard";
import { MeasurementForm } from "./MeasurementForm";
import { PlusIcon } from "@/components/icons";
import type { Measurement, MeasurementFormData } from "@baudoku/documentation";

type MeasurementsTabProps = {
  measurements: Measurement[];
  onCreateMeasurement: (data: MeasurementFormData) => Promise<void>;
  onDeleteMeasurement: (id: string) => void;
  isCreating: boolean;
};

export function MeasurementsTab({
  measurements,
  onCreateMeasurement,
  onDeleteMeasurement,
  isCreating,
}: MeasurementsTabProps) {
  const [showForm, setShowForm] = useState(false);

  return (
    <div className="space-y-4">
      {!showForm ? (
        <button
          onClick={() => setShowForm(true)}
          className="inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
        >
          <PlusIcon />
          Messung hinzufügen
        </button>
      ) : (
        <MeasurementForm
          onSubmit={async (data: MeasurementFormData) => {
            await onCreateMeasurement(data);
            setShowForm(false);
          }}
          onCancel={() => setShowForm(false)}
          isSubmitting={isCreating}
        />
      )}

      {measurements.length > 0 ? (
        <div className="space-y-3">
          {measurements.map((m) => (
            <MeasurementCard
              key={m.id}
              measurement={m}
              onDelete={onDeleteMeasurement}
            />
          ))}
        </div>
      ) : (
        !showForm && (
          <p className="py-8 text-center text-sm text-gray-500">
            Noch keine Messungen vorhanden.
          </p>
        )
      )}
    </div>
  );
}
