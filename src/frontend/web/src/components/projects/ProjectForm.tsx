import { useForm } from 'react-hook-form';
import { projectSchema, type ProjectFormData, PROJECT_STATUS_LABELS } from '@baudoku/projects';
import { typedZodResolver } from '@/hooks/useZodForm';
import { optionsFromLabels } from '@baudoku/core';
import { FormField } from '../common/FormField';
import { FormSelect } from '../common/FormSelect';
import { Button } from '../common/Button';
import { useNavigate } from '@tanstack/react-router';

type ProjectFormProps = {
  defaultValues?: Partial<ProjectFormData>;
  onSubmit: (data: ProjectFormData) => void | Promise<void>;
  isSubmitting?: boolean;
};

const statusOptions = optionsFromLabels(PROJECT_STATUS_LABELS);

export function ProjectForm({
  defaultValues,
  onSubmit,
  isSubmitting,
}: ProjectFormProps) {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ProjectFormData>({
    resolver: typedZodResolver(projectSchema),
    defaultValues: {
      status: 'active',
      ...defaultValues,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className='space-y-6'>
      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>
          Projektdetails
        </h2>
        <div className='mt-4 grid gap-4 sm:grid-cols-2'>
          <div className='sm:col-span-2'>
            <FormField
              label='Projektname *'
              error={errors.name}
              register={register('name')}
              placeholder='z.B. Neubau Bürogebäude'
            />
          </div>
          <FormSelect
            label='Status'
            error={errors.status}
            register={register('status')}
            options={statusOptions}
          />
        </div>
      </div>

      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>Adresse</h2>
        <div className='mt-4 grid gap-4 sm:grid-cols-2'>
          <div className='sm:col-span-2'>
            <FormField
              label='Straße'
              error={errors.street}
              register={register('street')}
              placeholder='Musterstraße 1'
            />
          </div>
          <FormField
            label='PLZ'
            error={errors.zipCode}
            register={register('zipCode')}
            placeholder='12345'
          />
          <FormField
            label='Stadt'
            error={errors.city}
            register={register('city')}
            placeholder='Berlin'
          />
        </div>
      </div>

      <div className='rounded-xl border border-gray-200 bg-white p-6'>
        <h2 className='text-base font-semibold text-gray-900'>Auftraggeber</h2>
        <div className='mt-4 grid gap-4 sm:grid-cols-2'>
          <FormField
            label='Name'
            error={errors.clientName}
            register={register('clientName')}
            placeholder='Firma GmbH'
          />
          <FormField
            label='Kontakt'
            error={errors.clientContact}
            register={register('clientContact')}
            placeholder='Max Mustermann'
          />
        </div>
      </div>

      <div className='flex justify-end gap-3'>
        <Button
          type='button'
          variant='secondary'
          onClick={() => navigate({ to: '/projects' })}
        >
          Abbrechen
        </Button>
        <Button type='submit' disabled={isSubmitting}>
          {isSubmitting ? 'Speichert...' : 'Projekt erstellen'}
        </Button>
      </div>
    </form>
  );
}
