import {
  calculateGpsQuality as _calculateGpsQuality,
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

// Mobile uses full-length labels (shared package has abbreviated "Ext." forms)
export const gpsSourceLabels: Record<string, string> = {
  internal_gps: 'Internes GPS',
  external_dgnss: 'Externes DGNSS',
  external_rtk: 'Externes RTK',
};
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
