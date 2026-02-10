import { apiGet, apiPost } from "./apiClient";

export interface SyncDeltaDto {
  entityType: string;
  entityId: string;
  operation: string;
  baseVersion: number;
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

export interface ServerDeltaDto {
  entityType: string;
  entityId: string;
  operation: string;
  version: number;
  payload: string;
  timestamp: string;
}

export interface ChangeSetResult {
  changes: ServerDeltaDto[];
  serverTimestamp: string;
  hasMore: boolean;
}

export async function pushBatch(
  deviceId: string,
  deltas: SyncDeltaDto[]
): Promise<ProcessSyncBatchResult> {
  return apiPost<ProcessSyncBatchResult>("/api/sync/batch", {
    deviceId,
    deltas,
  });
}

export async function pullChanges(
  deviceId: string,
  since?: string | null,
  limit?: number
): Promise<ChangeSetResult> {
  const params = new URLSearchParams({ deviceId });
  if (since) params.set("since", since);
  if (limit) params.set("limit", limit.toString());

  return apiGet<ChangeSetResult>(`/api/sync/changes?${params}`);
}

export async function getConflicts(
  deviceId?: string,
  status?: string
): Promise<ConflictDto[]> {
  const params = new URLSearchParams();
  if (deviceId) params.set("deviceId", deviceId);
  if (status) params.set("status", status);

  const qs = params.toString();
  return apiGet<ConflictDto[]>(`/api/sync/conflicts${qs ? `?${qs}` : ""}`);
}

export async function resolveConflict(
  conflictId: string,
  strategy: string,
  mergedPayload?: string
): Promise<void> {
  await apiPost(`/api/sync/conflicts/${conflictId}/resolve`, {
    strategy,
    mergedPayload: mergedPayload ?? null,
  });
}
