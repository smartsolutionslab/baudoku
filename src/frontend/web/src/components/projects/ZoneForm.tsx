import { useForm } from 'react-hook-form';
import { zoneSchema, type ZoneFormData, ZONE_TYPE_LABELS, formatZoneLabel, type Zone } from '@baudoku/projects';
import { typedZodResolver } from '@/hooks/useZodForm';
import { optionsFromLabels } from '@baudoku/core';
import { FormField } from '../common/FormField';
import { FormSelect } from '../common/FormSelect';
import { Button } from '../common/Button';

type ZoneFormProps = {
  zones: Zone[];
  defaultValues?: Partial<ZoneFormData>;
  onSubmit: (data: ZoneFormData) => void | Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
};

const typeOptions = optionsFromLabels(ZONE_TYPE_LABELS);
const NO_PARENT_OPTION = { value: '', label: 'Keine (Oberste Ebene)' };

export function ZoneForm({ zones, defaultValues, onSubmit, onCancel, isSubmitting}: ZoneFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ZoneFormData>({
    resolver: typedZodResolver(zoneSchema),
    defaultValues: {
      type: 'room',
      sortOrder: 0,
      ...defaultValues,
    },
  });

  const parentOptions = [
    NO_PARENT_OPTION,
    ...zones.map(({ id, name, type }) => ({
      value: id,
      label: formatZoneLabel(name, type),
    })),
  ];

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <div className="grid gap-4 sm:grid-cols-2">
          <div className="sm:col-span-2">
            <FormField
              label="Zonenname *"
              error={errors.name}
              register={register('name')}
              placeholder="z.B. Erdgeschoss"
            />
          </div>
          <FormSelect
            label="Typ *"
            error={errors.type}
            register={register('type')}
            options={typeOptions}
          />
          <FormField
            label="Sortierung"
            error={errors.sortOrder}
            register={register('sortOrder')}
            type="number"
            placeholder="0"
          />
          <div className="sm:col-span-2">
            <FormSelect
              label="Übergeordnete Zone"
              error={errors.parentZoneId}
              register={register('parentZoneId')}
              options={parentOptions}
            />
          </div>
        </div>
      </div>

      <div className="flex justify-end gap-3">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Abbrechen
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Speichert...' : 'Zone erstellen'}
        </Button>
      </div>
    </form>
  );
}
