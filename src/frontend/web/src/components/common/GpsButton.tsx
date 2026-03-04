import { useEffect } from 'react';
import type { Latitude, Longitude } from '@baudoku/core';
import { useGpsCapture } from '../../hooks/useGpsCapture';
import { Button } from './Button';
import { GpsIcon } from '../icons';

type GpsButtonProps = {
  onCapture: (position: {
    latitude: Latitude;
    longitude: Longitude;
    altitude: number | null;
    accuracy: number;
  }) => void;
};

export function GpsButton({ onCapture }: GpsButtonProps) {
  const { position, capturing, error, capturePosition } = useGpsCapture();

  useEffect(() => {
    if (position) {
      onCapture(position);
    }
  }, [position, onCapture]);

  return (
    <div>
      <Button
        type="button"
        variant="secondary"
        onClick={capturePosition}
        disabled={capturing}
        className="inline-flex items-center gap-2"
      >
        {capturing ? (
          <div className="h-4 w-4 animate-spin rounded-full border-2 border-gray-300 border-t-blue-600" />
        ) : (
          <GpsIcon />
        )}
        GPS erfassen
      </Button>
      {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
    </div>
  );
}
