import { useState } from 'react';
import { useForm, type FieldError } from 'react-hook-form';
import { z } from 'zod';
import { typedZodResolver } from '@/hooks/useZodForm';
import {
  installationSchema,
  type InstallationFormData,
  INSTALLATION_STATUS_LABELS,
  INSTALLATION_TYPE_OPTIONS,
  PHASE_OPTIONS,
} from '@baudoku/documentation';
import { formatZoneLabel, type Zone } from '@baudoku/projects';
import { optionsFromLabels } from '@baudoku/core';
import { Button } from '../common/Button';
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

const statusOptions = optionsFromLabels(INSTALLATION_STATUS_LABELS);
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
      status: 'planned',
      zoneId: '',
      ...defaultValues,
    },
  });

  const [gps, setGps] = useState<GpsFormData | null>(null);
  const fieldErrors = errors as Record<string, FieldError | undefined>;
  const reg = register;

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
        register={reg}
        errors={fieldErrors}
        typeOptions={typeOptions}
        statusOptions={statusOptions}
        zoneOptions={zoneOptions}
      />

      <GpsPositionSelector gps={gps} onGpsChange={setGps} />

      <InstallationComponentSection register={reg} errors={fieldErrors} />

      <InstallationCableSection register={reg} errors={fieldErrors} phaseOptions={phaseOptions} />

      <InstallationNotesSection register={reg} />

      <div className="flex justify-end gap-3">
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
