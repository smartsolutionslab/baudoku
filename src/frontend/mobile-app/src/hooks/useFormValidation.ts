import { useState, useCallback } from "react";
import type { ZodSchema } from "zod";

export function useFormValidation<T>(schema: ZodSchema<T>) {
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = useCallback(
    (data: unknown): T | null => {
      const result = schema.safeParse(data);
      if (!result.success) {
        const fieldErrors: Record<string, string> = {};
        for (const issue of result.error.issues) {
          const key = issue.path[0]?.toString();
          if (key && !fieldErrors[key]) fieldErrors[key] = issue.message;
        }
        setErrors(fieldErrors);
        return null;
      }
      setErrors({});
      return result.data;
    },
    [schema]
  );

  return { errors, setErrors, validate };
}
