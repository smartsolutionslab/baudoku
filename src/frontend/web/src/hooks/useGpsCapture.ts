import { useState, useCallback } from "react";

interface GpsPosition {
  latitude: number;
  longitude: number;
  altitude: number | null;
  accuracy: number;
}

interface UseGpsCaptureResult {
  position: GpsPosition | null;
  loading: boolean;
  error: string | null;
  capture: () => void;
  clear: () => void;
}

export function useGpsCapture(): UseGpsCaptureResult {
  const [position, setPosition] = useState<GpsPosition | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const capture = useCallback(() => {
    if (!navigator.geolocation) {
      setError("GPS wird von diesem Browser nicht unterstützt.");
      return;
    }

    setLoading(true);
    setError(null);

    navigator.geolocation.getCurrentPosition(
      (pos) => {
        setPosition({
          latitude: pos.coords.latitude,
          longitude: pos.coords.longitude,
          altitude: pos.coords.altitude,
          accuracy: pos.coords.accuracy,
        });
        setLoading(false);
      },
      (err) => {
        const messages: Record<number, string> = {
          1: "GPS-Zugriff verweigert. Bitte erlauben Sie den Standortzugriff.",
          2: "GPS-Position konnte nicht ermittelt werden.",
          3: "GPS-Anfrage hat zu lange gedauert.",
        };
        setError(messages[err.code] ?? "Unbekannter GPS-Fehler.");
        setLoading(false);
      },
      { enableHighAccuracy: true, timeout: 15_000, maximumAge: 0 }
    );
  }, []);

  const clear = useCallback(() => {
    setPosition(null);
    setError(null);
  }, []);

  return { position, loading, error, capture, clear };
}
