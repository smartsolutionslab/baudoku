type LoadingSpinnerProps = {
  size?: 'sm' | 'md';
};

const sizeStyles = {
  sm: 'h-4 w-4 border-2',
  md: 'h-8 w-8 border-4',
};

export function LoadingSpinner({ size = 'md' }: LoadingSpinnerProps) {
  return (
    <div
      className={`${sizeStyles[size]} animate-spin rounded-full border-gray-200 border-t-blue-600`}
    />
  );
}
