const QR_PREFIX = "baudoku://zone/";

export function encodeZoneQr(projectId: string, zoneId: string): string {
  return `${QR_PREFIX}${projectId}/${zoneId}`;
}

export interface DecodedZoneQr {
  projectId: string;
  zoneId: string;
}

export function decodeZoneQr(data: string): DecodedZoneQr | null {
  if (!data.startsWith(QR_PREFIX)) return null;

  const rest = data.slice(QR_PREFIX.length);
  const parts = rest.split("/");
  if (parts.length !== 2) return null;

  const [projectId, zoneId] = parts;
  if (!projectId || !zoneId) return null;

  return { projectId, zoneId };
}
