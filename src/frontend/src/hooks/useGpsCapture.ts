import { useState, useCallback } from "react";
import * as Location from "expo-location";

export interface GpsPosition {
  gpsLat: number;
  gpsLng: number;
  gpsAltitude: number | null;
  gpsAccuracy: number;
  gpsSource: "internal_gps";
  gpsCorrService: "none";
  gpsRtkStatus: "autonomous";
  gpsSatCount: number | null;
  gpsHdop: number | null;
  gpsCorrAge: number | null;
}

export interface UseGpsCaptureReturn {
  position: GpsPosition | null;
  capturing: boolean;
  error: string | null;
  capturePosition: () => Promise<GpsPosition | null>;
  clearPosition: () => void;
}

export function useGpsCapture(): UseGpsCaptureReturn {
  const [position, setPosition] = useState<GpsPosition | null>(null);
  const [capturing, setCapturing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const capturePosition = useCallback(async (): Promise<GpsPosition | null> => {
    setCapturing(true);
    setError(null);

    try {
      const { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== "granted") {
        setError("Standort-Berechtigung wurde verweigert.");
        setCapturing(false);
        return null;
      }

      const location = await Location.getCurrentPositionAsync({
        accuracy: Location.Accuracy.BestForNavigation,
      });

      const gps: GpsPosition = {
        gpsLat: location.coords.latitude,
        gpsLng: location.coords.longitude,
        gpsAltitude: location.coords.altitude,
        gpsAccuracy: location.coords.accuracy ?? 0,
        gpsSource: "internal_gps",
        gpsCorrService: "none",
        gpsRtkStatus: "autonomous",
        gpsSatCount: null,
        gpsHdop: null,
        gpsCorrAge: null,
      };

      setPosition(gps);
      setCapturing(false);
      return gps;
    } catch (e) {
      const msg =
        e instanceof Error ? e.message : "GPS-Position konnte nicht erfasst werden.";
      setError(msg);
      setCapturing(false);
      return null;
    }
  }, []);

  const clearPosition = useCallback(() => {
    setPosition(null);
    setError(null);
  }, []);

  return { position, capturing, error, capturePosition, clearPosition };
}
