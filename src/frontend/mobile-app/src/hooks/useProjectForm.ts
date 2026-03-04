import { projectSchema, type ProjectFormData } from '../validation/schemas';
import { MUTATION_ERRORS } from '../constants/strings';
import { useEntityForm } from './useEntityForm';

export type UseProjectFormOptions = {
  initialValues?: Partial<ProjectFormData>;
  onSubmit: (data: ProjectFormData) => Promise<void>;
};

export type UseProjectFormReturn = {
  form: Partial<ProjectFormData>;
  errors: Record<string, string>;
  set: <K extends keyof ProjectFormData>(key: K, value: ProjectFormData[K]) => void;
  handleSubmit: () => Promise<void>;
};

export function useProjectForm({
  initialValues,
  onSubmit,
}: UseProjectFormOptions): UseProjectFormReturn {
  return useEntityForm({
    schema: projectSchema,
    initialValues: { status: 'active', ...initialValues },
    onSubmit,
    errorMessage: MUTATION_ERRORS.projectSave,
  });
}
