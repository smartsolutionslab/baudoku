import { useState, useEffect } from 'react';
import { useGpsCapture } from '@/hooks/useGpsCapture';
import { latitude as toLatitude, longitude as toLongitude } from '@baudoku/core';
import type { Latitude, Longitude } from '@baudoku/core';
import { Button } from '../common/Button';
import { GpsIcon } from '../icons';

export type GpsFormData = {
  latitude: Latitude;
  longitude: Longitude;
  altitude: number | null;
  accuracy: number;
  source: 'browser' | 'manual';
};

type GpsPositionSelectorProps = {
  gps: GpsFormData | null;
  onGpsChange: (gps: GpsFormData | null) => void;
  error?: string | null;
};

export function GpsPositionSelector({ gps, onGpsChange, error }: GpsPositionSelectorProps) {
  const [manualGps, setManualGps] = useState(false);
  const [manualLat, setManualLat] = useState('');
  const [manualLng, setManualLng] = useState('');
  const {
    position: browserPosition,
    capturing,
    error: captureError,
    capturePosition,
  } = useGpsCapture();

  useEffect(() => {
    if (browserPosition) {
      onGpsChange({
        latitude: browserPosition.latitude,
        longitude: browserPosition.longitude,
        altitude: browserPosition.altitude,
        accuracy: browserPosition.accuracy,
        source: 'browser',
      });
      // eslint-disable-next-line react-hooks/set-state-in-effect -- sync state when browser position arrives
      setManualGps(false);
    }
  }, [browserPosition, onGpsChange]);

  const applyManualGps = () => {
    const lat = parseFloat(manualLat);
    const lng = parseFloat(manualLng);
    if (isNaN(lat) || isNaN(lng) || lat < -90 || lat > 90 || lng < -180 || lng > 180) return;
    onGpsChange({
      latitude: toLatitude(lat),
      longitude: toLongitude(lng),
      altitude: null,
      accuracy: 0,
      source: 'manual',
    });
  };

  const displayError = error || captureError;

  return (
    <div className="rounded-xl border border-gray-200 bg-white p-6">
      <h2 className="text-base font-semibold text-gray-900">Position</h2>
      <div className="mt-4">
        {gps ? (
          <div className="flex items-center gap-4">
            <div className="flex-1 rounded-lg bg-green-50 px-4 py-3">
              <p className="text-sm font-medium text-green-800">
                {gps.latitude.toFixed(6)}, {gps.longitude.toFixed(6)}
                {gps.accuracy > 0 && (
                  <span className="ml-2 text-green-600">(&plusmn;{gps.accuracy.toFixed(1)} m)</span>
                )}
              </p>
              <p className="text-xs text-green-600">
                {gps.source === 'browser' ? 'Browser-GPS' : 'Manuell eingegeben'}
              </p>
            </div>
            <Button
              type="button"
              variant="secondary"
              onClick={() => {
                onGpsChange(null);
                setManualGps(false);
              }}
            >
              Entfernen
            </Button>
          </div>
        ) : manualGps ? (
          <div className="space-y-3">
            <div className="grid gap-3 sm:grid-cols-2">
              <div>
                <label className="block text-sm font-medium text-gray-700">Breitengrad *</label>
                <input
                  type="number"
                  step="any"
                  placeholder="z.B. 48.137154"
                  value={manualLat}
                  onChange={(e) => setManualLat(e.target.value)}
                  className="mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Längengrad *</label>
                <input
                  type="number"
                  step="any"
                  placeholder="z.B. 11.576124"
                  value={manualLng}
                  onChange={(e) => setManualLng(e.target.value)}
                  className="mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
              </div>
            </div>
            <div className="flex gap-2">
              <Button type="button" onClick={applyManualGps}>
                Übernehmen
              </Button>
              <Button type="button" variant="secondary" onClick={() => setManualGps(false)}>
                Abbrechen
              </Button>
            </div>
          </div>
        ) : (
          <div className="flex gap-3">
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
              Browser-GPS erfassen
            </Button>
            <Button type="button" variant="secondary" onClick={() => setManualGps(true)}>
              Manuell eingeben
            </Button>
          </div>
        )}
        {displayError && <p className="mt-2 text-sm text-red-600">{displayError}</p>}
      </div>
    </div>
  );
}
