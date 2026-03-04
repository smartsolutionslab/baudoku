import type { TextareaHTMLAttributes } from 'react';
import type { FieldError, UseFormRegisterReturn } from 'react-hook-form';
import { inputClassName } from './formStyles';

type FormTextareaProps = TextareaHTMLAttributes<HTMLTextAreaElement> & {
  label: string;
  error?: FieldError;
  register: UseFormRegisterReturn;
};

export function FormTextarea({ label, error, register, ...props }: FormTextareaProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700">{label}</label>
      <textarea {...register} {...props} className={inputClassName(!!error)} />
      {error && <p className="mt-1 text-sm text-red-600">{error.message}</p>}
    </div>
  );
}
