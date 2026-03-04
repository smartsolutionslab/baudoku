import type { FieldError, UseFormRegister } from 'react-hook-form';
import { FormField } from '../common/FormField';
import { FormSection } from '../common/FormSection';

type ComponentProps = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any -- register must accept any form shape
  register: UseFormRegister<any>;
  errors: Record<string, FieldError | undefined>;
};

export function InstallationComponentSection({ register, errors }: ComponentProps) {
  return (
    <FormSection title="Komponente">
      <FormField
        label="Hersteller"
        error={errors.manufacturer}
        register={register('manufacturer')}
      />
      <FormField label="Modell" error={errors.model} register={register('model')} />
      <FormField
        label="Seriennummer"
        error={errors.serialNumber}
        register={register('serialNumber')}
      />
    </FormSection>
  );
}
