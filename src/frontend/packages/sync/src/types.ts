export type SyncDeltaDto = {
  entityType: string;
  entityId: string;
  operation: string;
  baseVersion: number;
  payload: string;
  timestamp: string;
};

export type ServerDeltaDto = {
  entityType: string;
  entityId: string;
  operation: string;
  version: number;
  payload: string;
  timestamp: string;
};

export type ConflictDto = {
  id: string;
  entityType: string;
  entityId: string;
  clientPayload: string;
  serverPayload: string;
  clientVersion: number;
  serverVersion: number;
  status: string;
  detectedAt: string;
};

export type ProcessSyncBatchResult = {
  batchId: string;
  appliedCount: number;
  conflictCount: number;
  conflicts: ConflictDto[];
};

export type ChangeSetResult = {
  changes: ServerDeltaDto[];
  serverTimestamp: string;
  hasMore: boolean;
};
