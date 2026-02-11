import { useState, useCallback, useRef } from "react";
import { decodeZoneQr } from "../utils/qrCode";
import type { BarcodeScanningResult } from "expo-camera";

export interface QrScanResult {
  type: "zone";
  projectId: string;
  zoneId: string;
}

export interface UseQrScannerReturn {
  scanned: boolean;
  scanResult: QrScanResult | null;
  error: string | null;
  onBarcodeScanned: (result: BarcodeScanningResult) => void;
  resetScanner: () => void;
}

const DEBOUNCE_MS = 1000;

export function useQrScanner(): UseQrScannerReturn {
  const [scanned, setScanned] = useState(false);
  const [scanResult, setScanResult] = useState<QrScanResult | null>(null);
  const [error, setError] = useState<string | null>(null);
  const lastScanTime = useRef(0);

  const onBarcodeScanned = useCallback(
    (result: BarcodeScanningResult) => {
      const now = Date.now();
      if (now - lastScanTime.current < DEBOUNCE_MS) return;
      lastScanTime.current = now;

      if (result.type !== "qr") return;

      const decoded = decodeZoneQr(result.data);
      if (!decoded) {
        setError("Kein gültiger BauDoku QR-Code.");
        setScanned(true);
        setScanResult(null);
        return;
      }

      setScanResult({
        type: "zone",
        projectId: decoded.projectId,
        zoneId: decoded.zoneId,
      });
      setScanned(true);
      setError(null);
    },
    []
  );

  const resetScanner = useCallback(() => {
    setScanned(false);
    setScanResult(null);
    setError(null);
    lastScanTime.current = 0;
  }, []);

  return { scanned, scanResult, error, onBarcodeScanned, resetScanner };
}
