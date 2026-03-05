export const SYNC_STATUSES = ['pending', 'syncing'] as const;

export const SYNC_STATUS_LABELS: Record<(typeof SYNC_STATUSES)[number], string> = {
  pending: 'Ausstehend',
  syncing: 'Wird synchronisiert',
};
