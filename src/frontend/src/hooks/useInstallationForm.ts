import { useState, useCallback } from "react";
import { Alert } from "react-native";
import {
  installationSchema,
  type InstallationFormData,
} from "../validation/schemas";
import type { GpsPosition } from "./useGpsCapture";

export type UseInstallationFormOptions = {
  initialValues?: Partial<InstallationFormData>;
  initialGps?: GpsPosition | null;
  onSubmit: (data: InstallationFormData, gps: GpsPosition | null) => Promise<void>;
};

export type UseInstallationFormReturn = {
  form: Record<string, unknown>;
  errors: Record<string, string>;
  set: (key: string, value: unknown) => void;
  str: (key: string) => string;
  handleSubmit: (currentGps: GpsPosition | null) => Promise<void>;
  hasComponentValues: boolean;
  hasCableValues: boolean;
  hasElectricalValues: boolean;
};

export function useInstallationForm({
  initialValues,
  onSubmit,
}: UseInstallationFormOptions): UseInstallationFormReturn {
  const [form, setForm] = useState<Record<string, unknown>>({
    status: "in_progress",
    ...initialValues,
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const hasComponentValues =
    !!initialValues?.manufacturer ||
    !!initialValues?.model ||
    !!initialValues?.serialNumber;
  const hasCableValues =
    !!initialValues?.cableType ||
    initialValues?.crossSectionMm2 != null ||
    initialValues?.lengthM != null;
  const hasElectricalValues =
    !!initialValues?.circuitId ||
    !!initialValues?.fuseType ||
    initialValues?.fuseRatingA != null ||
    initialValues?.voltageV != null ||
    initialValues?.phase != null;

  const set = useCallback((key: string, value: unknown) => {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => {
      const next = { ...prev };
      delete next[key];
      return next;
    });
  }, []);

  const str = (key: string) => {
    const val = form[key];
    if (val == null) return "";
    return String(val);
  };

  const handleSubmit = useCallback(
    async (currentGps: GpsPosition | null) => {
      // Strip empty strings before validation
      const cleaned: Record<string, unknown> = {};
      for (const [k, v] of Object.entries(form)) {
        if (typeof v === "string" && v.trim() === "") continue;
        cleaned[k] = v;
      }

      const result = installationSchema.safeParse(cleaned);
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
        await onSubmit(result.data, currentGps);
      } catch {
        Alert.alert("Fehler", "Installation konnte nicht gespeichert werden.");
      }
    },
    [form, onSubmit]
  );

  return {
    form,
    errors,
    set,
    str,
    handleSubmit,
    hasComponentValues,
    hasCableValues,
    hasElectricalValues,
  };
}
