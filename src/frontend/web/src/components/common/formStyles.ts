export const labelClassName = 'block text-sm font-medium text-gray-700';

export const errorClassName = 'mt-1 text-sm text-red-600';

export function inputClassName(hasError?: boolean) {
  return `mt-1 block w-full rounded-lg border px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-1 ${
    hasError
      ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
      : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
  }`;
}

export const cardClassName = 'rounded-xl border border-gray-200 bg-white p-6';

export const formActionsClassName = 'flex justify-end gap-3';

export const buttonClassName = {
  primary:
    'inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700 transition-colors',
  secondary:
    'inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors',
};
