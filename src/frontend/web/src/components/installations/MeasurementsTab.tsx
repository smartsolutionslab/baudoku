import { useState } from 'react';
import { MeasurementCard } from './MeasurementCard';
import { MeasurementForm } from './MeasurementForm';
import { PlusIcon } from '@/components/icons';
import { Button } from '@/components/common/Button';
import type { MeasurementId } from '@baudoku/core';
import type { Measurement, MeasurementFormData } from '@baudoku/documentation';

type MeasurementsTabProps = {
  measurements: Measurement[];
  onCreateMeasurement: (data: MeasurementFormData) => Promise<void>;
  onDeleteMeasurement: (id: MeasurementId) => void;
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
        <Button
          variant="secondary"
          onClick={() => setShowForm(true)}
          className="inline-flex items-center gap-2"
        >
          <PlusIcon />
          Messung hinzufügen
        </Button>
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
            <MeasurementCard key={m.id} measurement={m} onDelete={onDeleteMeasurement} />
          ))}
        </div>
      ) : (
        !showForm && (
          <p className="py-8 text-center text-sm text-gray-500">Noch keine Messungen vorhanden.</p>
        )
      )}
    </div>
  );
}
