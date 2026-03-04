import type { UseFormRegister } from 'react-hook-form';
import { FormTextarea } from '../common/FormTextarea';
import { FormSection } from '../common/FormSection';

type NotesProps = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any -- register must accept any form shape
  register: UseFormRegister<any>;
};

export function InstallationNotesSection({ register }: NotesProps) {
  return (
    <FormSection title="Notizen">
      <div className="sm:col-span-2">
        <FormTextarea
          label="Bemerkungen"
          register={register('notes')}
          rows={3}
          placeholder="Zusätzliche Bemerkungen..."
        />
      </div>
    </FormSection>
  );
}
