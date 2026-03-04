import { Colors } from '@/styles/tokens';

export const UPLOAD_STATUS_COLORS: Record<string, string> = {
  uploaded: Colors.success,
  uploading: Colors.primary,
  failed: Colors.danger,
  pending: Colors.textTertiary,
};

export const UPLOAD_STATUS_LABELS: Record<string, string> = {
  uploaded: 'hochgeladen',
  uploading: 'lädt hoch',
  failed: 'fehlgeschlagen',
  pending: 'ausstehend',
};
