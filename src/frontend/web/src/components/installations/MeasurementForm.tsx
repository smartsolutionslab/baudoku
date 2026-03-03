import { useForm } from 'react-hook-form';
import { typedZodResolver } from '@/hooks/useZodForm';
import {
  measurementSchema,
  type MeasurementFormData,
  MEASUREMENT_TYPES,
} from '@baudoku/documentation';
import { FormField } from '../common/FormField';
import { FormSelect } from '../common/FormSelect';
import { Button } from '../common/Button';
import { inputClassName } from '../common/formStyles';

type MeasurementFormProps = {
  onSubmit: (data: MeasurementFormData) => void | Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
};

const typeOptions = MEASUREMENT_TYPES.map((m) => ({
  value: m.type,
  label: `${m.type} (${m.unit})`,
}));

export function MeasurementForm({
  onSubmit,
  onCancel,
  isSubmitting,
}: MeasurementFormProps) {
  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<MeasurementFormData>({
    resolver: typedZodResolver(measurementSchema),
  });

  const selectedType = watch('type');

  // Auto-fill unit and thresholds when type changes
  const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const preset = MEASUREMENT_TYPES.find((m) => m.type === e.target.value);
    if (preset) {
      setValue('unit', preset.unit);
      if (preset.minThreshold != null) setValue('minThreshold', preset.minThreshold);
      if (preset.maxThreshold != null) setValue('maxThreshold', preset.maxThreshold);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className='space-y-4'>
      <div className='rounded-xl border border-gray-200 bg-white p-4'>
        <div className='grid gap-4 sm:grid-cols-2'>
          <div className='sm:col-span-2'>
            <label className='block text-sm font-medium text-gray-700'>
              Messtyp *
            </label>
            <select
              {...register('type')}
              onChange={(e) => {
                register('type').onChange(e);
                handleTypeChange(e);
              }}
              className={inputClassName(!!errors.type)}
            >
              <option value=''>Messtyp auswählen</option>
              {typeOptions.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
            {errors.type && (
              <p className='mt-1 text-sm text-red-600'>{errors.type.message}</p>
            )}
          </div>
          <FormField
            label='Messwert *'
            error={errors.value}
            register={register('value')}
            type='number'
            step='any'
          />
          <FormField
            label='Einheit *'
            error={errors.unit}
            register={register('unit')}
          />
          <FormField
            label='Min. Schwellwert'
            error={errors.minThreshold}
            register={register('minThreshold')}
            type='number'
            step='any'
          />
          <FormField
            label='Max. Schwellwert'
            error={errors.maxThreshold}
            register={register('maxThreshold')}
            type='number'
            step='any'
          />
          <div className='sm:col-span-2'>
            <FormField
              label='Prüfer *'
              error={errors.measuredBy}
              register={register('measuredBy')}
              placeholder='Name des Prüfers'
            />
          </div>
          <div className='sm:col-span-2'>
            <label className='block text-sm font-medium text-gray-700'>
              Notizen
            </label>
            <textarea
              {...register('notes')}
              rows={2}
              className='mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500'
            />
          </div>
        </div>
      </div>

      <div className='flex justify-end gap-3'>
        <Button type='button' variant='secondary' onClick={onCancel}>
          Abbrechen
        </Button>
        <Button type='submit' disabled={isSubmitting}>
          {isSubmitting ? 'Speichert...' : 'Messung speichern'}
        </Button>
      </div>
    </form>
  );
}
