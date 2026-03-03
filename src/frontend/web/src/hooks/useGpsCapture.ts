import { useState, useCallback } from 'react';

type GpsPosition = {
  latitude: number;
  longitude: number;
  altitude: number | null;
  accuracy: number;
};

type UseGpsCaptureReturn = {
  position: GpsPosition | null;
  capturing: boolean;
  error: string | null;
  capturePosition: () => void;
  clearPosition: () => void;
};

export function useGpsCapture(): UseGpsCaptureReturn {
  const [position, setPosition] = useState<GpsPosition | null>(null);
  const [capturing, setCapturing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const capturePosition = useCallback(() => {
    if (!navigator.geolocation) {
      setError('GPS wird von diesem Browser nicht unterstützt.');
      return;
    }

    setCapturing(true);
    setError(null);

    navigator.geolocation.getCurrentPosition(
      (pos) => {
        setPosition({
          latitude: pos.coords.latitude,
          longitude: pos.coords.longitude,
          altitude: pos.coords.altitude,
          accuracy: pos.coords.accuracy,
        });
        setCapturing(false);
      },
      (err) => {
        const messages: Record<number, string> = {
          1: 'GPS-Zugriff verweigert. Bitte erlauben Sie den Standortzugriff.',
          2: 'GPS-Position konnte nicht ermittelt werden.',
          3: 'GPS-Anfrage hat zu lange gedauert.',
        };
        setError(messages[err.code] ?? 'Unbekannter GPS-Fehler.');
        setCapturing(false);
      },
      { enableHighAccuracy: true, timeout: 15_000, maximumAge: 0 }
    );
  }, []);

  const clearPosition = useCallback(() => {
    setPosition(null);
    setError(null);
  }, []);

  return { position, capturing, error, capturePosition, clearPosition };
}
