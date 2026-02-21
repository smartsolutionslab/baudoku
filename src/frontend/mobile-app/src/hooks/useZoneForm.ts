import { useState, useCallback } from "react";
import { Alert } from "react-native";
import { zoneSchema, type ZoneFormData } from "../validation/schemas";
import { useFormValidation } from "./useFormValidation";

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

export function useZoneForm({ initialValues, defaultParentZoneId, onSubmit }: UseZoneFormOptions): UseZoneFormReturn {
  const [form, setForm] = useState<Partial<ZoneFormData>>({
    type: "building",
    parentZoneId: defaultParentZoneId ?? null,
    ...initialValues,
  });
  const { errors, setErrors, validate } = useFormValidation(zoneSchema);

  const set = useCallback(
    <K extends keyof ZoneFormData>(key: K, value: ZoneFormData[K]) => {
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
      Alert.alert("Fehler", "Zone konnte nicht gespeichert werden.");
    }
  }, [form, onSubmit, validate]);

  return { form, errors, set, handleSubmit };
}
