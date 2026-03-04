import type { TextareaHTMLAttributes } from 'react';
import type { FieldError, UseFormRegisterReturn } from 'react-hook-form';
import { inputClassName, labelClassName, errorClassName } from './formStyles';

type FormTextareaProps = TextareaHTMLAttributes<HTMLTextAreaElement> & {
  label: string;
  error?: FieldError;
  register: UseFormRegisterReturn;
};

export function FormTextarea({ label, error, register, ...props }: FormTextareaProps) {
  return (
    <div>
      <label className={labelClassName}>{label}</label>
      <textarea {...register} {...props} className={inputClassName(!!error)} />
      {error && <p className={errorClassName}>{error.message}</p>}
    </div>
  );
}
