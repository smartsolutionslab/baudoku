import {
  calculateGpsQuality as _calculateGpsQuality,
  GPS_SOURCE_LABELS,
  GPS_CORRECTION_SERVICE_LABELS,
  RTK_FIX_STATUS_LABELS,
} from '@baudoku/documentation';
import type { GpsQualityGrade, GpsQualityInput } from '@baudoku/documentation';
import { Colors } from '../styles/tokens';

export type { GpsQualityGrade } from '@baudoku/documentation';
export {
  GPS_CORRECTION_SERVICE_LABELS,
  RTK_FIX_STATUS_LABELS,
} from '@baudoku/documentation';

// Re-export label maps under old names for backward compat
export const gpsSourceLabels = GPS_SOURCE_LABELS as Record<string, string>;
export const corrServiceLabels = GPS_CORRECTION_SERVICE_LABELS;
export const rtkLabels = RTK_FIX_STATUS_LABELS;

// ─── Platform-specific color mapping ────────────────────────────

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

export type GpsQualityResult = {
  grade: GpsQualityGrade;
  label: string;
  color: string;
  bgColor: string;
};

export function calculateGpsQuality(input: GpsQualityInput): GpsQualityResult {
  const { grade, label } = _calculateGpsQuality(input);
  return {
    grade,
    label,
    color: gradeColors[grade],
    bgColor: gradeBgColors[grade],
  };
}
