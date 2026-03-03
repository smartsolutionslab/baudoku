import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { typedZodResolver } from '@/hooks/useZodForm';
import { useGpsCapture } from '@/hooks/useGpsCapture';
import {
  installationSchema,
  type InstallationFormData,
  INSTALLATION_STATUS_LABELS,
  INSTALLATION_TYPES,
  PHASES,
} from '@baudoku/documentation';
import { ZONE_TYPE_LABELS, type Zone } from '@baudoku/projects';
import { optionsFromLabels } from '@baudoku/core';
import { FormField } from '../common/FormField';
import { FormSelect } from '../common/FormSelect';
import { Button } from '../common/Button';

export type GpsFormData = {
  latitude: number;
  longitude: number;
  altitude: number | null;
  accuracy: number;
  source: 'browser' | 'manual';
};

type InstallationFormProps = {
  zones: Zone[];
  defaultValues?: Partial<InstallationFormData & { zoneId: string }>;
  onSubmit: (data: InstallationFormData & { zoneId: string }, gps: GpsFormData | null) => void | Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
};

const statusOptions = optionsFromLabels(INSTALLATION_STATUS_LABELS);

const typeOptions = INSTALLATION_TYPES.map((t) => ({ value: t, label: t }));

const phaseOptions = PHASES.map((p) => ({ value: p, label: p }));

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
    setValue,
    watch,
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
  const [manualGps, setManualGps] = useState(false);
  const [manualLat, setManualLat] = useState('');
  const [manualLng, setManualLng] = useState('');
  const { position: browserPosition, capturing, error: gpsError, capturePosition } = useGpsCapture();

  useEffect(() => {
    if (browserPosition) {
      setGps({
        latitude: browserPosition.latitude,
        longitude: browserPosition.longitude,
        altitude: browserPosition.altitude,
        accuracy: browserPosition.accuracy,
        source: 'browser',
      });
      setManualGps(false);
    }
  }, [browserPosition]);

  const applyManualGps = () => {
    const lat = parseFloat(manualLat);
    const lng = parseFloat(manualLng);
    if (isNaN(lat) || isNaN(lng) || lat < -90 || lat > 90 || lng < -180 || lng > 180) return;
    setGps({ latitude: lat, longitude: lng, altitude: null, accuracy: 0, source: 'manual' });
  };

  const submitWithGps = (data: InstallationWithZoneFormData) => {
    onSubmit(data, gps);
  };

  const zoneOptions = zones.map((z) => ({
    value: z.id,
    label: `${z.name} (${ZONE_TYPE_LABELS[z.type as keyof typeof ZONE_TYPE_LABELS] ?? z.type})`,
  }));

  return (
    <form onSubmit={handleSubmit(submitWithGps)} className='space-y-6'>
      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>Grunddaten</h2>
        <div className='mt-4 grid gap-4 sm:grid-cols-2'>
          <FormSelect
            label='Typ *'
            error={errors.type}
            register={register('type')}
            options={typeOptions}
            placeholder='Typ auswählen'
          />
          <FormSelect
            label='Status'
            error={errors.status}
            register={register('status')}
            options={statusOptions}
          />
          <div className='sm:col-span-2'>
            <FormSelect
              label='Zone *'
              error={errors.zoneId}
              register={register('zoneId')}
              options={zoneOptions}
              placeholder='Zone auswählen'
            />
          </div>
        </div>
      </div>

      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>Position</h2>
        <div className='mt-4'>
          {gps ? (
            <div className='flex items-center gap-4'>
              <div className='flex-1 rounded-lg bg-green-50 px-4 py-3'>
                <p className='text-sm font-medium text-green-800'>
                  {gps.latitude.toFixed(6)}, {gps.longitude.toFixed(6)}
                  {gps.accuracy > 0 && (
                    <span className='ml-2 text-green-600'>
                      (&plusmn;{gps.accuracy.toFixed(1)} m)
                    </span>
                  )}
                </p>
                <p className='text-xs text-green-600'>
                  {gps.source === 'browser' ? 'Browser-GPS' : 'Manuell eingegeben'}
                </p>
              </div>
              <Button type='button' variant='secondary' onClick={() => { setGps(null); setManualGps(false); }}>
                Entfernen
              </Button>
            </div>
          ) : manualGps ? (
            <div className='space-y-3'>
              <div className='grid gap-3 sm:grid-cols-2'>
                <div>
                  <label className='block text-sm font-medium text-gray-700'>Breitengrad *</label>
                  <input
                    type='number'
                    step='any'
                    placeholder='z.B. 48.137154'
                    value={manualLat}
                    onChange={(e) => setManualLat(e.target.value)}
                    className='mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500'
                  />
                </div>
                <div>
                  <label className='block text-sm font-medium text-gray-700'>Längengrad *</label>
                  <input
                    type='number'
                    step='any'
                    placeholder='z.B. 11.576124'
                    value={manualLng}
                    onChange={(e) => setManualLng(e.target.value)}
                    className='mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500'
                  />
                </div>
              </div>
              <div className='flex gap-2'>
                <Button type='button' onClick={applyManualGps}>
                  Übernehmen
                </Button>
                <Button type='button' variant='secondary' onClick={() => setManualGps(false)}>
                  Abbrechen
                </Button>
              </div>
            </div>
          ) : (
            <div className='flex gap-3'>
              <Button
                type='button'
                variant='secondary'
                onClick={capturePosition}
                disabled={capturing}
                className='inline-flex items-center gap-2'
              >
                {capturing ? (
                  <div className='h-4 w-4 animate-spin rounded-full border-2 border-gray-300 border-t-blue-600' />
                ) : (
                  <svg className='h-4 w-4' fill='none' viewBox='0 0 24 24' stroke='currentColor' strokeWidth={1.5}>
                    <path strokeLinecap='round' strokeLinejoin='round' d='M15 10.5a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z' />
                    <path strokeLinecap='round' strokeLinejoin='round' d='M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1 1 15 0Z' />
                  </svg>
                )}
                Browser-GPS erfassen
              </Button>
              <Button type='button' variant='secondary' onClick={() => setManualGps(true)}>
                Manuell eingeben
              </Button>
            </div>
          )}
          {gpsError && <p className='mt-2 text-sm text-red-600'>{gpsError}</p>}
        </div>
      </div>

      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>Komponente</h2>
        <div className='mt-4 grid gap-4 sm:grid-cols-2'>
          <FormField
            label='Hersteller'
            error={errors.manufacturer}
            register={register('manufacturer')}
          />
          <FormField
            label='Modell'
            error={errors.model}
            register={register('model')}
          />
          <FormField
            label='Seriennummer'
            error={errors.serialNumber}
            register={register('serialNumber')}
          />
        </div>
      </div>

      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>
          Kabel / Elektrisch
        </h2>
        <div className='mt-4 grid gap-4 sm:grid-cols-3'>
          <FormField
            label='Kabeltyp'
            error={errors.cableType}
            register={register('cableType')}
            placeholder='z.B. NYM-J'
          />
          <FormField
            label='Querschnitt'
            error={errors.crossSectionMm2}
            register={register('crossSectionMm2')}
            type='number'
            step='0.1'
            suffix='mm²'
          />
          <FormField
            label='Länge'
            error={errors.lengthM}
            register={register('lengthM')}
            type='number'
            step='0.1'
            suffix='m'
          />
          <FormField
            label='Stromkreis-ID'
            error={errors.circuitId}
            register={register('circuitId')}
          />
          <FormField
            label='Sicherungstyp'
            error={errors.fuseType}
            register={register('fuseType')}
          />
          <FormField
            label='Nennstrom'
            error={errors.fuseRatingA}
            register={register('fuseRatingA')}
            type='number'
            suffix='A'
          />
          <FormField
            label='Spannung'
            error={errors.voltageV}
            register={register('voltageV')}
            type='number'
            suffix='V'
          />
          <FormSelect
            label='Phase'
            error={errors.phase}
            register={register('phase')}
            options={phaseOptions}
            placeholder='—'
          />
          <FormField
            label='Tiefe'
            error={errors.depthMm}
            register={register('depthMm')}
            type='number'
            suffix='mm'
          />
        </div>
      </div>

      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>Notizen</h2>
        <div className='mt-4'>
          <textarea
            {...register('notes')}
            rows={3}
            className='block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500'
            placeholder='Zusätzliche Bemerkungen...'
          />
        </div>
      </div>

      <div className='flex justify-end gap-3'>
        <Button type='button' variant='secondary' onClick={onCancel}>
          Abbrechen
        </Button>
        <Button type='submit' disabled={isSubmitting}>
          {isSubmitting ? 'Speichert...' : 'Installation erstellen'}
        </Button>
      </div>
    </form>
  );
}
