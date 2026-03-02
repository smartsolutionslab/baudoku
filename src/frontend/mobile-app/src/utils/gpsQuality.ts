import { Colors } from "../styles/tokens";

export type GpsQualityGrade = "A" | "B" | "C" | "D";

export type GpsQualityResult = {
  grade: GpsQualityGrade;
  label: string;
  color: string;
  bgColor: string;
};

type GpsQualityInput = {
  horizontalAccuracy: number;
  hdop?: number | null;
  satelliteCount?: number | null;
  correctionService?: string | null;
};

const gradeLabels: Record<GpsQualityGrade, string> = {
  A: "Ausgezeichnet",
  B: "Gut",
  C: "Akzeptabel",
  D: "Ungenau",
};

const gradeColors: Record<GpsQualityGrade, string> = {
  A: Colors.success,
  B: Colors.gpsGradeB,
  C: Colors.warning,
  D: Colors.danger,
};

const gradeBgColors: Record<GpsQualityGrade, string> = {
  A: Colors.gpsBgA,
  B: Colors.gpsBgB,
  C: Colors.gpsBgC,
  D: Colors.gpsBgD,
};

const grades: GpsQualityGrade[] = ["A", "B", "C", "D"];

/**
 * Port of backend `GpsPosition.CalculateQualityGrade()`.
 *
 * Stage 1 – base grade from horizontal accuracy:
 *   <1m → A, <5m → B, <30m → C, ≥30m → D
 *
 * Stage 2 – bonus points (positive adjustments):
 *   HDOP < 2        → +1
 *   Satellites ≥ 8   → +1
 *   Correction svc   → +1
 *
 * Stage 3 – penalty points (negative adjustments):
 *   HDOP > 5        → −1
 *   Satellites < 4   → −1
 *
 * Stage 4 – adjustment clamped to [−1, +1], final grade clamped to [0, 3].
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
  if (correctionService != null && correctionService !== "none") bonus++;

  // Stage 3: penalty
  let penalty = 0;
  if (hdop != null && hdop > 5.0) penalty++;
  if (satelliteCount != null && satelliteCount < 4) penalty++;

  // Stage 4: clamp adjustment and final grade
  const adjustment = Math.max(-1, Math.min(1, bonus - penalty));
  const finalGrade = Math.max(0, Math.min(3, baseGrade - adjustment));

  const grade = grades[finalGrade];
  return {
    grade,
    label: gradeLabels[grade],
    color: gradeColors[grade],
    bgColor: gradeBgColors[grade],
  };
}

export const gpsSourceLabels: Record<string, string> = {
  internal_gps: "Internes GPS",
  external_dgnss: "Externes DGNSS",
  external_rtk: "Externes RTK",
};

export const corrServiceLabels: Record<string, string> = {
  none: "Keine",
  sapos_eps: "SAPOS-EPS",
  sapos_heps: "SAPOS-HEPS",
  sapos_gpps: "SAPOS-GPPS",
};

export const rtkLabels: Record<string, string> = {
  no_fix: "Kein Fix",
  autonomous: "Autonom",
  dgps: "DGPS",
  rtk_float: "RTK Float",
  rtk_fixed: "RTK Fixed",
};
