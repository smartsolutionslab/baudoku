export interface SyncDeltaDto {
  entityType: string;
  entityId: string;
  operation: string;
  baseVersion: number;
  payload: string;
  timestamp: string;
}

export interface ServerDeltaDto {
  entityType: string;
  entityId: string;
  operation: string;
  version: number;
  payload: string;
  timestamp: string;
}

export interface ConflictDto {
  id: string;
  entityType: string;
  entityId: string;
  clientPayload: string;
  serverPayload: string;
  clientVersion: number;
  serverVersion: number;
  status: string;
  detectedAt: string;
}

export interface ProcessSyncBatchResult {
  batchId: string;
  appliedCount: number;
  conflictCount: number;
  conflicts: ConflictDto[];
}

export interface ChangeSetResult {
  changes: ServerDeltaDto[];
  serverTimestamp: string;
  hasMore: boolean;
}
