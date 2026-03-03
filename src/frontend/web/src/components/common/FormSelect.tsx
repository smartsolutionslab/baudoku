import type { SelectHTMLAttributes } from 'react';
import type { FieldError, UseFormRegisterReturn } from 'react-hook-form';
import { inputClassName } from './formStyles';

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
      <label className='block text-sm font-medium text-gray-700'>
        {label}
      </label>
      <select
        {...register}
        {...props}
        className={inputClassName(!!error)}
      >
        {placeholder && (
          <option value=''>{placeholder}</option>
        )}
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
      {error && (
        <p className='mt-1 text-sm text-red-600'>{error.message}</p>
      )}
    </div>
  );
}
