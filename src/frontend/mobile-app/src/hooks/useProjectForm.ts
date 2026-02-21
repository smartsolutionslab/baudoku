import { useState, useCallback } from "react";
import { Alert } from "react-native";
import { projectSchema, type ProjectFormData } from "../validation/schemas";
import { useFormValidation } from "./useFormValidation";

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

export function useProjectForm({ initialValues, onSubmit }: UseProjectFormOptions): UseProjectFormReturn {
  const [form, setForm] = useState<Partial<ProjectFormData>>({
    status: "active",
    ...initialValues,
  });
  const { errors, setErrors, validate } = useFormValidation(projectSchema);

  const set = useCallback(
    <K extends keyof ProjectFormData>(key: K, value: ProjectFormData[K]) => {
      setForm((prev) => ({ ...prev, [key]: value }));
      setErrors((prev) => {
        const next = { ...prev };
        delete next[key];
        return next;
      });
    },
    [setErrors]
  );

  const handleSubmit = useCallback(async () => {
    const data = validate(form);
    if (!data) return;
    try {
      await onSubmit(data);
    } catch {
      Alert.alert("Fehler", "Projekt konnte nicht gespeichert werden.");
    }
  }, [form, onSubmit, validate]);

  return { form, errors, set, handleSubmit };
}
