import { useState, useCallback } from "react";
import { Platform } from "react-native";
import * as Location from "expo-location";
import { useSettingsStore } from "../store";

export type GpsSource = "internal_gps" | "external_dgnss" | "external_rtk";
export type GpsCorrService = "none" | "sapos_eps" | "sapos_heps" | "sapos_gpps";
export type GpsRtkStatus = "no_fix" | "autonomous" | "dgps" | "rtk_float" | "rtk_fixed";

export type GpsPosition = {
  gpsLat: number;
  gpsLng: number;
  gpsAltitude: number | null;
  gpsAccuracy: number;
  gpsSource: GpsSource;
  gpsCorrService: GpsCorrService;
  gpsRtkStatus: GpsRtkStatus;
  gpsSatCount: number | null;
  gpsHdop: number | null;
  gpsCorrAge: number | null;
  isMocked: boolean;
};

export type UseGpsCaptureReturn = {
  position: GpsPosition | null;
  capturing: boolean;
  error: string | null;
  capturePosition: () => Promise<GpsPosition | null>;
  clearPosition: () => void;
};

export function useGpsCapture(): UseGpsCaptureReturn {
  const [position, setPosition] = useState<GpsPosition | null>(null);
  const [capturing, setCapturing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const allowMockLocation = useSettingsStore((s) => s.allowMockLocation);

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

      const isMocked =
        Platform.OS === "android" &&
        (location as unknown as { mocked?: boolean }).mocked === true;

      if (isMocked && !allowMockLocation) {
        setError("Externes GPS ist in den Einstellungen deaktiviert.");
        setCapturing(false);
        return null;
      }

      const gps: GpsPosition = {
        gpsLat: location.coords.latitude,
        gpsLng: location.coords.longitude,
        gpsAltitude: location.coords.altitude,
        gpsAccuracy: location.coords.accuracy ?? 0,
        gpsSource: isMocked ? "external_dgnss" : "internal_gps",
        gpsCorrService: isMocked ? "sapos_eps" : "none",
        gpsRtkStatus: isMocked ? "dgps" : "autonomous",
        gpsSatCount: null,
        gpsHdop: null,
        gpsCorrAge: null,
        isMocked,
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
  }, [allowMockLocation]);

  const clearPosition = useCallback(() => {
    setPosition(null);
    setError(null);
  }, []);

  return { position, capturing, error, capturePosition, clearPosition };
}
