import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { typedZodResolver } from '@/hooks/useZodForm';
import {
  measurementSchema,
  type MeasurementFormData,
  MEASUREMENT_TYPES,
  MEASUREMENT_TYPE_OPTIONS,
} from '@baudoku/documentation';
import { FormField } from '../common/FormField';
import { FormSelect } from '../common/FormSelect';
import { FormTextarea } from '../common/FormTextarea';
import { Button } from '../common/Button';

type MeasurementFormProps = {
  onSubmit: (data: MeasurementFormData) => void | Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
};

const typeOptions = MEASUREMENT_TYPE_OPTIONS;

export function MeasurementForm({ onSubmit, onCancel, isSubmitting }: MeasurementFormProps) {
  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<MeasurementFormData>({
    resolver: typedZodResolver(measurementSchema),
  });

  // Auto-fill unit and thresholds when type changes
  // eslint-disable-next-line react-hooks/incompatible-library
  const watchType = watch('type');
  useEffect(() => {
    const preset = MEASUREMENT_TYPES.find((m) => m.type === watchType);
    if (preset) {
      setValue('unit', preset.unit);
      if (preset.minThreshold != null) setValue('minThreshold', preset.minThreshold);
      if (preset.maxThreshold != null) setValue('maxThreshold', preset.maxThreshold);
    }
  }, [watchType, setValue]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="rounded-xl border border-gray-200 bg-white p-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <div className="sm:col-span-2">
            <FormSelect
              label="Messtyp *"
              error={errors.type}
              register={register('type')}
              options={typeOptions}
              placeholder="Messtyp auswählen"
            />
          </div>
          <FormField
            label="Messwert *"
            error={errors.value}
            register={register('value')}
            type="number"
            step="any"
          />
          <FormField label="Einheit *" error={errors.unit} register={register('unit')} />
          <FormField
            label="Min. Schwellwert"
            error={errors.minThreshold}
            register={register('minThreshold')}
            type="number"
            step="any"
          />
          <FormField
            label="Max. Schwellwert"
            error={errors.maxThreshold}
            register={register('maxThreshold')}
            type="number"
            step="any"
          />
          <div className="sm:col-span-2">
            <FormField
              label="Prüfer *"
              error={errors.measuredBy}
              register={register('measuredBy')}
              placeholder="Name des Prüfers"
            />
          </div>
          <div className="sm:col-span-2">
            <FormTextarea
              label="Notizen"
              error={errors.notes}
              register={register('notes')}
              rows={2}
            />
          </div>
        </div>
      </div>

      <div className="flex justify-end gap-3">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Abbrechen
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Speichert...' : 'Messung speichern'}
        </Button>
      </div>
    </form>
  );
}
