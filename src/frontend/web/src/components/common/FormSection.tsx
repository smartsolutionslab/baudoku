import type { ReactNode } from 'react';

type FormSectionProps = {
  title: string;
  columns?: 2 | 3;
  children: ReactNode;
};

export function FormSection({ title, columns = 2, children }: FormSectionProps) {
  return (
    <div className="rounded-xl border border-gray-200 bg-white p-6">
      <h2 className="text-base font-semibold text-gray-900">{title}</h2>
      <div className={`mt-4 grid gap-4 ${columns === 3 ? 'sm:grid-cols-3' : 'sm:grid-cols-2'}`}>
        {children}
      </div>
    </div>
  );
}
