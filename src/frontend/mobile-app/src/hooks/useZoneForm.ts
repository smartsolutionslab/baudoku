import type { ZoneId } from '@baudoku/core';
import { zoneSchema, type ZoneFormData } from '../validation/schemas';
import { MUTATION_ERRORS } from '../constants/strings';
import { useEntityForm } from './useEntityForm';

export type UseZoneFormOptions = {
  initialValues?: Partial<ZoneFormData>;
  defaultParentZoneId?: ZoneId | null;
  onSubmit: (data: ZoneFormData) => Promise<void>;
};

export type UseZoneFormReturn = {
  form: Partial<ZoneFormData>;
  errors: Record<string, string>;
  set: <K extends keyof ZoneFormData>(key: K, value: ZoneFormData[K]) => void;
  handleSubmit: () => Promise<void>;
};

export function useZoneForm({
  initialValues,
  defaultParentZoneId,
  onSubmit,
}: UseZoneFormOptions): UseZoneFormReturn {
  return useEntityForm({
    schema: zoneSchema,
    initialValues: {
      type: 'building',
      parentZoneId: defaultParentZoneId ?? null,
      ...initialValues,
    },
    onSubmit,
    errorMessage: MUTATION_ERRORS.zoneSave,
  });
}
