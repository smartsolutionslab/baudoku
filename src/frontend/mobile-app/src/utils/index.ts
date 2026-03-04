/** Narrows a route param from `string | undefined` to `string`, throwing if missing. */
export function requiredParam(value: string | undefined): string {
  if (value == null) throw new Error('Missing required route parameter');
  return value;
}

export { getDeviceId } from './deviceId';
export { formatDate, formatDateTime } from '@baudoku/core';
export { toGpsPosition } from './gpsMapping';
export {
  calculateGpsQuality,
  type GpsQualityGrade,
  type GpsQualityResult,
  gpsSourceLabels,
  corrServiceLabels,
  rtkLabels,
} from './gpsQuality';
export { ensurePhotoDir, savePhoto, deletePhotoFile } from './photoStorage';
export { encodeZoneQr, decodeZoneQr, type DecodedZoneQr } from './qrCode';
export { generateId } from './uuid';
