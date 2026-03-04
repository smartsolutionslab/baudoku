import type { FieldError, UseFormRegister } from 'react-hook-form';
import { FormField } from '../common/FormField';
import { FormSelect } from '../common/FormSelect';
import { FormSection } from '../common/FormSection';

type CableProps = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  register: UseFormRegister<any>;
  errors: Record<string, FieldError | undefined>;
  phaseOptions: { value: string; label: string }[];
};

export function InstallationCableSection({ register, errors, phaseOptions }: CableProps) {
  return (
    <FormSection title="Kabel / Elektrisch" columns={3}>
      <FormField
        label="Kabeltyp"
        error={errors.cableType}
        register={register('cableType')}
        placeholder="z.B. NYM-J"
      />
      <FormField
        label="Querschnitt"
        error={errors.crossSectionMm2}
        register={register('crossSectionMm2')}
        type="number"
        step="0.1"
        suffix="mm²"
      />
      <FormField
        label="Länge"
        error={errors.lengthM}
        register={register('lengthM')}
        type="number"
        step="0.1"
        suffix="m"
      />
      <FormField label="Stromkreis-ID" error={errors.circuitId} register={register('circuitId')} />
      <FormField label="Sicherungstyp" error={errors.fuseType} register={register('fuseType')} />
      <FormField
        label="Nennstrom"
        error={errors.fuseRatingA}
        register={register('fuseRatingA')}
        type="number"
        suffix="A"
      />
      <FormField
        label="Spannung"
        error={errors.voltageV}
        register={register('voltageV')}
        type="number"
        suffix="V"
      />
      <FormSelect
        label="Phase"
        error={errors.phase}
        register={register('phase')}
        options={phaseOptions}
        placeholder="—"
      />
      <FormField
        label="Tiefe"
        error={errors.depthMm}
        register={register('depthMm')}
        type="number"
        suffix="mm"
      />
    </FormSection>
  );
}
