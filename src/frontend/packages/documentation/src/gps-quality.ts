import type { GpsCorrectionService, RtkFixStatus } from './types';

export type GpsQualityGrade = 'A' | 'B' | 'C' | 'D';

export type GpsQualityInput = {
  horizontalAccuracy: number;
  hdop?: number | null;
  satelliteCount?: number | null;
  correctionService?: GpsCorrectionService | null;
};

export type GpsQualityResult = {
  grade: GpsQualityGrade;
  label: string;
};

export const GPS_QUALITY_LABELS: Record<GpsQualityGrade, string> = {
  A: 'Ausgezeichnet',
  B: 'Gut',
  C: 'Akzeptabel',
  D: 'Ungenau',
};

export const GPS_CORRECTION_SERVICE_LABELS: Record<GpsCorrectionService, string> = {
  none: 'Keine',
  sapos_eps: 'SAPOS-EPS',
  sapos_heps: 'SAPOS-HEPS',
  sapos_gpps: 'SAPOS-GPPS',
};

export const RTK_FIX_STATUS_LABELS: Record<RtkFixStatus, string> = {
  no_fix: 'Kein Fix',
  autonomous: 'Autonom',
  dgps: 'DGPS',
  rtk_float: 'RTK Float',
  rtk_fixed: 'RTK Fixed',
};

const grades: GpsQualityGrade[] = ['A', 'B', 'C', 'D'];

/**
 * Port of backend `GpsPosition.CalculateQualityGrade()`.
 *
 * Stage 1 – base grade from horizontal accuracy:
 *   <1m -> A, <5m -> B, <30m -> C, >=30m -> D
 *
 * Stage 2 – bonus points (positive adjustments):
 *   HDOP < 2        -> +1
 *   Satellites >= 8  -> +1
 *   Correction svc   -> +1
 *
 * Stage 3 – penalty points (negative adjustments):
 *   HDOP > 5        -> -1
 *   Satellites < 4   -> -1
 *
 * Stage 4 – adjustment clamped to [-1, +1], final grade clamped to [0, 3].
 */
export function calculateGpsQuality(input: GpsQualityInput): GpsQualityResult {
  const { horizontalAccuracy, hdop, satelliteCount, correctionService } = input;

  // Stage 1: base grade from accuracy
  let baseGrade: number;
  if (horizontalAccuracy < 1) baseGrade = 0;
  else if (horizontalAccuracy < 5) baseGrade = 1;
  else if (horizontalAccuracy < 30) baseGrade = 2;
  else baseGrade = 3;

  // Stage 2: bonus
  let bonus = 0;
  if (hdop != null && hdop < 2.0) bonus++;
  if (satelliteCount != null && satelliteCount >= 8) bonus++;
  if (correctionService != null && correctionService !== 'none') bonus++;

  // Stage 3: penalty
  let penalty = 0;
  if (hdop != null && hdop > 5.0) penalty++;
  if (satelliteCount != null && satelliteCount < 4) penalty++;

  // Stage 4: clamp adjustment and final grade
  const adjustment = Math.max(-1, Math.min(1, bonus - penalty));
  const finalGrade = Math.max(0, Math.min(3, baseGrade - adjustment));

  const grade = grades[finalGrade]!;
  return {
    grade,
    label: GPS_QUALITY_LABELS[grade],
  };
}
