/** Narrows a route param from `string | undefined` to `string`, throwing if missing. */
export function requiredParam(value: string | undefined): string {
  if (value == null) throw new Error('Missing required route parameter');
  return value;
}

/** Strip entries whose value is an empty (or whitespace-only) string before validation. */
export function stripEmptyStrings(obj: Record<string, unknown>): Record<string, unknown> {
  const result: Record<string, unknown> = {};
  for (const [k, v] of Object.entries(obj)) {
    if (typeof v === 'string' && v.trim() === '') continue;
    result[k] = v;
  }
  return result;
}

export { getDeviceId } from './deviceId';
export { formatDate, formatDateTime } from '@baudoku/core';
export { toGpsPosition } from './gpsMapping';
export {
  calculateGpsQuality,
  type GpsQualityGrade,
  type GpsQualityResult,
  gpsSourceLabels,
} from './gpsQuality';
export { ensurePhotoDir, savePhoto, deletePhotoFile } from './photoStorage';
export { encodeZoneQr, decodeZoneQr, type DecodedZoneQr } from './qrCode';
export { generateId } from './uuid';
