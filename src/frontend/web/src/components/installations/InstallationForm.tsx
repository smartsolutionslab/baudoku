import { useState } from 'react';
import { useForm, type FieldError } from 'react-hook-form';
import { z } from 'zod';
import { typedZodResolver } from '@/hooks/useZodForm';
import {
  installationSchema,
  type InstallationFormData,
  INSTALLATION_TYPE_OPTIONS,
  INSTALLATION_STATUS_OPTIONS,
  PHASE_OPTIONS,
} from '@baudoku/documentation';
import { formatZoneLabel, type Zone } from '@baudoku/projects';
import { Button } from '../common/Button';
import { formActionsClassName } from '../common/formStyles';
import { GpsPositionSelector } from './GpsPositionSelector';
import type { GpsFormData } from './GpsPositionSelector';
import { InstallationBasicsSection } from './InstallationBasicsSection';
import { InstallationComponentSection } from './InstallationComponentSection';
import { InstallationCableSection } from './InstallationCableSection';
import { InstallationNotesSection } from './InstallationNotesSection';
export type { GpsFormData } from './GpsPositionSelector';

type InstallationFormProps = {
  zones: Zone[];
  defaultValues?: Partial<InstallationFormData & { zoneId: string }>;
  onSubmit: (
    data: InstallationFormData & { zoneId: string },
    gps: GpsFormData | null,
  ) => void | Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
};

const statusOptions = INSTALLATION_STATUS_OPTIONS;
const typeOptions = INSTALLATION_TYPE_OPTIONS;
const phaseOptions = PHASE_OPTIONS;

const installationWithZoneSchema = installationSchema.extend({
  zoneId: z.string().min(1, 'Zone erforderlich'),
});

type InstallationWithZoneFormData = z.infer<typeof installationWithZoneSchema>;

export function InstallationForm({
  zones,
  defaultValues,
  onSubmit,
  onCancel,
  isSubmitting,
}: InstallationFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<InstallationWithZoneFormData>({
    resolver: typedZodResolver(installationWithZoneSchema),
    defaultValues: {
      status: 'in_progress',
      zoneId: '',
      ...defaultValues,
    },
  });

  const [gps, setGps] = useState<GpsFormData | null>(null);
  const fieldErrors = errors as Record<string, FieldError | undefined>;

  const submitWithGps = (data: InstallationWithZoneFormData) => {
    onSubmit(data, gps);
  };

  const zoneOptions = zones.map(({ id, name, type }) => ({
    value: id,
    label: formatZoneLabel(name, type),
  }));

  return (
    <form onSubmit={handleSubmit(submitWithGps)} className="space-y-6">
      <InstallationBasicsSection
        register={register}
        errors={fieldErrors}
        typeOptions={typeOptions}
        statusOptions={statusOptions}
        zoneOptions={zoneOptions}
      />

      <GpsPositionSelector gps={gps} onGpsChange={setGps} />

      <InstallationComponentSection register={register} errors={fieldErrors} />

      <InstallationCableSection
        register={register}
        errors={fieldErrors}
        phaseOptions={phaseOptions}
      />

      <InstallationNotesSection register={register} />

      <div className={formActionsClassName}>
        <Button type="button" variant="secondary" onClick={onCancel}>
          Abbrechen
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Speichert...' : 'Installation erstellen'}
        </Button>
      </div>
    </form>
  );
}
