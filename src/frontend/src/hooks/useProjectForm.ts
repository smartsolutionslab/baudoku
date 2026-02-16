import { useState, useCallback } from "react";
import { Alert } from "react-native";
import { projectSchema, type ProjectFormData } from "../validation/schemas";

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
  const [form, setForm] = useState<Partial<ProjectFormData>>({
    status: "active",
    ...initialValues,
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const set = useCallback(
    <K extends keyof ProjectFormData>(key: K, value: ProjectFormData[K]) => {
      setForm((prev) => ({ ...prev, [key]: value }));
      setErrors((prev) => {
        const next = { ...prev };
        delete next[key];
        return next;
      });
    },
    []
  );

  const handleSubmit = useCallback(async () => {
    const result = projectSchema.safeParse(form);
    if (!result.success) {
      const fieldErrors: Record<string, string> = {};
      for (const issue of result.error.issues) {
        const key = issue.path[0]?.toString();
        if (key && !fieldErrors[key]) {
          fieldErrors[key] = issue.message;
        }
      }
      setErrors(fieldErrors);
      return;
    }
    try {
      await onSubmit(result.data);
    } catch {
      Alert.alert("Fehler", "Projekt konnte nicht gespeichert werden.");
    }
  }, [form, onSubmit]);

  return { form, errors, set, handleSubmit };
}
