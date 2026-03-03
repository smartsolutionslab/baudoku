import { useEffect } from 'react';
import { useGpsCapture } from '../../hooks/useGpsCapture';
import { Button } from './Button';

type GpsButtonProps = {
  onCapture: (position: { latitude: number; longitude: number; altitude: number | null; accuracy: number }) => void;
};

export function GpsButton({ onCapture }: GpsButtonProps) {
  const { position, loading, error, capture } = useGpsCapture();

  useEffect(() => {
    if (position) {
      onCapture(position);
    }
  }, [position, onCapture]);

  return (
    <div>
      <Button
        type='button'
        variant='secondary'
        onClick={capture}
        disabled={loading}
        className='inline-flex items-center gap-2'
      >
        {loading ? (
          <div className='h-4 w-4 animate-spin rounded-full border-2 border-gray-300 border-t-blue-600' />
        ) : (
          <svg className='h-4 w-4' fill='none' viewBox='0 0 24 24' stroke='currentColor' strokeWidth={1.5}>
            <path strokeLinecap='round' strokeLinejoin='round' d='M15 10.5a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z' />
            <path strokeLinecap='round' strokeLinejoin='round' d='M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1 1 15 0Z' />
          </svg>
        )}
        GPS erfassen
      </Button>
      {error && <p className='mt-1 text-sm text-red-600'>{error}</p>}
    </div>
  );
}
