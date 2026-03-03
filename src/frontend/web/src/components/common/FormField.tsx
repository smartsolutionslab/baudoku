import type { InputHTMLAttributes } from 'react';
import type { FieldError, UseFormRegisterReturn } from 'react-hook-form';
import { inputClassName } from './formStyles';

type FormFieldProps = InputHTMLAttributes<HTMLInputElement> & {
  label: string;
  error?: FieldError;
  register: UseFormRegisterReturn;
};

export function FormField({ label, error, register, ...props }: FormFieldProps) {
  return (
    <div>
      <label className='block text-sm font-medium text-gray-700'>
        {label}
      </label>
      <input
        {...register}
        {...props}
        className={inputClassName(!!error)}
      />
      {error && (
        <p className='mt-1 text-sm text-red-600'>{error.message}</p>
      )}
    </div>
  );
}
