import type { SelectHTMLAttributes } from 'react';
import type { FieldError, UseFormRegisterReturn } from 'react-hook-form';
import { inputClassName, labelClassName, errorClassName } from './formStyles';

type FormSelectProps = SelectHTMLAttributes<HTMLSelectElement> & {
  label: string;
  error?: FieldError;
  register: UseFormRegisterReturn;
  options: { value: string; label: string }[];
  placeholder?: string;
};

export function FormSelect({
  label,
  error,
  register,
  options,
  placeholder,
  ...props
}: FormSelectProps) {
  return (
    <div>
      <label className={labelClassName}>{label}</label>
      <select {...register} {...props} className={inputClassName(!!error)}>
        {placeholder && <option value="">{placeholder}</option>}
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
      {error && <p className={errorClassName}>{error.message}</p>}
    </div>
  );
}
