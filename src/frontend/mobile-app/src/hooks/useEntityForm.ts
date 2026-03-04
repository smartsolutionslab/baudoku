import { useState, useCallback } from 'react';
import { Alert } from 'react-native';
import type { ZodSchema } from 'zod';
import { useFormValidation } from './useFormValidation';

type UseEntityFormOptions<T> = {
  schema: ZodSchema<T>;
  initialValues: Partial<T>;
  onSubmit: (data: T) => Promise<void>;
  errorMessage: string;
};

type UseEntityFormReturn<T> = {
  form: Partial<T>;
  errors: Record<string, string>;
  set: <K extends keyof T>(key: K, value: T[K]) => void;
  handleSubmit: () => Promise<void>;
};

export function useEntityForm<T>({
  schema,
  initialValues,
  onSubmit,
  errorMessage,
}: UseEntityFormOptions<T>): UseEntityFormReturn<T> {
  const [form, setForm] = useState<Partial<T>>(initialValues);
  const { errors, setErrors, validate } = useFormValidation(schema);

  const set = useCallback(
    <K extends keyof T>(key: K, value: T[K]) => {
      setForm((prev) => ({ ...prev, [key]: value }));
      setErrors((prev) => {
        const { [key as string]: _, ...next } = prev;
        return next;
      });
    },
    [setErrors],
  );

  const handleSubmit = useCallback(async () => {
    const data = validate(form);
    if (!data) return;
    try {
      await onSubmit(data);
    } catch {
      Alert.alert('Fehler', errorMessage);
    }
  }, [form, onSubmit, validate, errorMessage]);

  return { form, errors, set, handleSubmit };
}
