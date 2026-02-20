import { apiGet, apiPost } from "@baudoku/core";
import type {
  SyncDeltaDto,
  ProcessSyncBatchResult,
  ChangeSetResult,
  ConflictDto,
} from "./types";

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
