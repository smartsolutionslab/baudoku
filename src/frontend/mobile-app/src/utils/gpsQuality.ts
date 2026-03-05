import { calculateGpsQuality as _calculateGpsQuality } from '@baudoku/documentation';
import type { GpsSource, GpsQualityGrade, GpsQualityInput } from '@baudoku/documentation';
import { Colors } from '../styles/tokens';

export type { GpsQualityGrade } from '@baudoku/documentation';

// Mobile uses full-length labels (shared package has abbreviated "Ext." forms)
export const gpsSourceLabels: Record<GpsSource, string> = {
  internal_gps: 'Internes GPS',
  external_dgnss: 'Externes DGNSS',
  external_rtk: 'Externes RTK',
};

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
