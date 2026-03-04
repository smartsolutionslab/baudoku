import { useState, useCallback } from 'react';
import type { Latitude, Longitude } from '@baudoku/core';
import { latitude as toLatitude, longitude as toLongitude } from '@baudoku/core';

const GPS_ERRORS = {
  unsupported: 'GPS wird von diesem Browser nicht unterstützt.',
  denied: 'GPS-Zugriff verweigert. Bitte erlauben Sie den Standortzugriff.',
  unavailable: 'GPS-Position konnte nicht ermittelt werden.',
  timeout: 'GPS-Anfrage hat zu lange gedauert.',
  unknown: 'Unbekannter GPS-Fehler.',
} as const;

type GpsPosition = {
  latitude: Latitude;
  longitude: Longitude;
  altitude: number | null;
  accuracy: number;
};

type UseGpsCaptureReturn = {
  position: GpsPosition | null;
  capturing: boolean;
  error: string | null;
  capturePosition: () => void;
};

export function useGpsCapture(): UseGpsCaptureReturn {
  const [position, setPosition] = useState<GpsPosition | null>(null);
  const [capturing, setCapturing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const capturePosition = useCallback(() => {
    if (!navigator.geolocation) {
      setError(GPS_ERRORS.unsupported);
      return;
    }

    setCapturing(true);
    setError(null);

    navigator.geolocation.getCurrentPosition(
      (pos) => {
        setPosition({
          latitude: toLatitude(pos.coords.latitude),
          longitude: toLongitude(pos.coords.longitude),
          altitude: pos.coords.altitude,
          accuracy: pos.coords.accuracy,
        });
        setCapturing(false);
      },
      (err) => {
        const messages: Record<number, string> = {
          1: GPS_ERRORS.denied,
          2: GPS_ERRORS.unavailable,
          3: GPS_ERRORS.timeout,
        };
        setError(messages[err.code] ?? GPS_ERRORS.unknown);
        setCapturing(false);
      },
      { enableHighAccuracy: true, timeout: 15_000, maximumAge: 0 },
    );
  }, []);

  return { position, capturing, error, capturePosition };
}
