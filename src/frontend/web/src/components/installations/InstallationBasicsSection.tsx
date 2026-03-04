import type { FieldError, UseFormRegister } from 'react-hook-form';
import { FormSelect } from '../common/FormSelect';
import { FormSection } from '../common/FormSection';

type BasicsProps = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any -- register must accept any form shape
  register: UseFormRegister<any>;
  errors: Record<string, FieldError | undefined>;
  typeOptions: { value: string; label: string }[];
  statusOptions: { value: string; label: string }[];
  zoneOptions: { value: string; label: string }[];
};

export function InstallationBasicsSection({
  register,
  errors,
  typeOptions,
  statusOptions,
  zoneOptions,
}: BasicsProps) {
  return (
    <FormSection title="Grunddaten">
      <FormSelect
        label="Typ *"
        error={errors.type}
        register={register('type')}
        options={typeOptions}
        placeholder="Typ auswählen"
      />
      <FormSelect
        label="Status"
        error={errors.status}
        register={register('status')}
        options={statusOptions}
      />
      <div className="sm:col-span-2">
        <FormSelect
          label="Zone *"
          error={errors.zoneId}
          register={register('zoneId')}
          options={zoneOptions}
          placeholder="Zone auswählen"
        />
      </div>
    </FormSection>
  );
}
