import { useState, useCallback } from 'react';
import { Platform } from 'react-native';
import * as Location from 'expo-location';
import type { Latitude, Longitude } from '@baudoku/core';
import { latitude as toLatitude, longitude as toLongitude } from '@baudoku/core';
import { useSettingsStore } from '../store';

export type GpsSource = 'internal_gps' | 'external_dgnss' | 'external_rtk';
export type GpsCorrService = 'none' | 'sapos_eps' | 'sapos_heps' | 'sapos_gpps';
export type GpsRtkStatus = 'no_fix' | 'autonomous' | 'dgps' | 'rtk_float' | 'rtk_fixed';

export type CapturedGpsPosition = {
  latitude: Latitude;
  longitude: Longitude;
  altitude: number | null;
  horizontalAccuracy: number;
  gpsSource: GpsSource;
  correctionService: GpsCorrService;
  rtkFixStatus: GpsRtkStatus;
  satelliteCount: number | null;
  hdop: number | null;
  correctionAge: number | null;
  isMocked: boolean;
};

/** Alias used by form components and hooks that handle GPS data. */
export type GpsPosition = CapturedGpsPosition;

export type UseGpsCaptureReturn = {
  position: CapturedGpsPosition | null;
  capturing: boolean;
  error: string | null;
  capturePosition: () => Promise<CapturedGpsPosition | null>;
  clearPosition: () => void;
};

export function useGpsCapture(): UseGpsCaptureReturn {
  const [position, setPosition] = useState<CapturedGpsPosition | null>(null);
  const [capturing, setCapturing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const allowMockLocation = useSettingsStore((s) => s.allowMockLocation);

  const capturePosition = useCallback(async (): Promise<CapturedGpsPosition | null> => {
    setCapturing(true);
    setError(null);

    try {
      const { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== 'granted') {
        setError('Standort-Berechtigung wurde verweigert.');
        setCapturing(false);
        return null;
      }

      const location = await Location.getCurrentPositionAsync({
        accuracy: Location.Accuracy.BestForNavigation,
      });

      const isMocked =
        Platform.OS === 'android' && (location as unknown as { mocked?: boolean }).mocked === true;

      if (isMocked && !allowMockLocation) {
        setError('Externes GPS ist in den Einstellungen deaktiviert.');
        setCapturing(false);
        return null;
      }

      const gps: CapturedGpsPosition = {
        latitude: toLatitude(location.coords.latitude),
        longitude: toLongitude(location.coords.longitude),
        altitude: location.coords.altitude,
        horizontalAccuracy: location.coords.accuracy ?? 0,
        gpsSource: isMocked ? 'external_dgnss' : 'internal_gps',
        correctionService: isMocked ? 'sapos_eps' : 'none',
        rtkFixStatus: isMocked ? 'dgps' : 'autonomous',
        satelliteCount: null,
        hdop: null,
        correctionAge: null,
        isMocked,
      };

      setPosition(gps);
      setCapturing(false);
      return gps;
    } catch (e) {
      const msg = e instanceof Error ? e.message : 'GPS-Position konnte nicht erfasst werden.';
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
