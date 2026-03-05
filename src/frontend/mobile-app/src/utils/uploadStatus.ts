import { Colors } from '@/styles/tokens';

export const UPLOAD_STATUSES = ['pending', 'uploading', 'uploaded', 'failed'] as const;

export const UPLOAD_STATUS_COLORS: Record<(typeof UPLOAD_STATUSES)[number], string> = {
  uploaded: Colors.success,
  uploading: Colors.primary,
  failed: Colors.danger,
  pending: Colors.textTertiary,
};

export const UPLOAD_STATUS_LABELS: Record<(typeof UPLOAD_STATUSES)[number], string> = {
  uploaded: 'hochgeladen',
  uploading: 'lädt hoch',
  failed: 'fehlgeschlagen',
  pending: 'ausstehend',
};
