import { useState, useCallback } from "react";
import { Alert } from "react-native";
import { zoneSchema, type ZoneFormData } from "../validation/schemas";

export type UseZoneFormOptions = {
  initialValues?: Partial<ZoneFormData>;
  defaultParentZoneId?: string | null;
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
  const [form, setForm] = useState<Partial<ZoneFormData>>({
    type: "building",
    parentZoneId: defaultParentZoneId ?? null,
    ...initialValues,
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const set = useCallback(
    <K extends keyof ZoneFormData>(key: K, value: ZoneFormData[K]) => {
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
    const result = zoneSchema.safeParse(form);
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
      Alert.alert("Fehler", "Zone konnte nicht gespeichert werden.");
    }
  }, [form, onSubmit]);

  return { form, errors, set, handleSubmit };
}
