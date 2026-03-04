import type { InputHTMLAttributes } from 'react';
import type { FieldError, UseFormRegisterReturn } from 'react-hook-form';
import { inputClassName } from './formStyles';

type FormFieldProps = InputHTMLAttributes<HTMLInputElement> & {
  label: string;
  error?: FieldError;
  register: UseFormRegisterReturn;
  suffix?: string;
};

export function FormField({ label, error, register, suffix, ...props }: FormFieldProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700">{label}</label>
      {suffix ? (
        <div className="relative mt-1">
          <input
            {...register}
            {...props}
            className={`block w-full rounded-lg border px-3 py-2 pr-12 text-sm shadow-sm focus:outline-none focus:ring-1 ${
              error
                ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
                : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }`}
          />
          <span className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3 text-sm text-gray-500">
            {suffix}
          </span>
        </div>
      ) : (
        <input {...register} {...props} className={inputClassName(!!error)} />
      )}
      {error && <p className="mt-1 text-sm text-red-600">{error.message}</p>}
    </div>
  );
}
