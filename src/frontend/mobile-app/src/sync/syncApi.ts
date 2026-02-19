import { apiPost, apiUpload, apiRawUpload } from "@baudoku/shared-api";
import type { PhotoUploadResult } from "@baudoku/shared-types";

// Re-export types from shared-types
export type {
  SyncDeltaDto,
  ConflictDto,
  ProcessSyncBatchResult,
  ServerDeltaDto,
  ChangeSetResult,
  PhotoUploadResult,
} from "@baudoku/shared-types";

// Re-export shared API functions
export { pushBatch, pullChanges, getConflicts, resolveConflict } from "@baudoku/shared-api";

// ─── Mobile-only: Expo URI-based photo upload ──────────────────

export async function uploadPhoto(installationId: string, fileUri: string, fileName: string, mimeType: string, caption?: string): Promise<PhotoUploadResult> {
  const formData = new FormData();
  formData.append("file", { uri: fileUri, name: fileName, type: mimeType} as unknown as Blob);
  if (caption) formData.append("caption", caption);

  return apiUpload<PhotoUploadResult>(`/api/documentation/installations/${installationId}/photos`, formData);
}

// ─── Mobile-only: Chunked upload ───────────────────────────────

export type ChunkedUploadInitResult = {
  sessionId: string;
};

export async function initChunkedUpload(installationId: string, fileName: string, contentType: string, totalSize: number, totalChunks: number, photoType: string, caption?: string): Promise<ChunkedUploadInitResult> {
  return apiPost<ChunkedUploadInitResult>("/api/documentation/uploads/init", { installationId, fileName, contentType, totalSize, totalChunks, photoType, caption } );
}

export async function uploadChunk(sessionId: string, chunkIndex: number, chunkBlob: Blob | ArrayBuffer): Promise<void> {
  await apiRawUpload(`/api/documentation/uploads/${sessionId}/chunks/${chunkIndex}`, chunkBlob, { "Content-Type": "application/octet-stream" });
}

export async function completeChunkedUpload(sessionId: string): Promise<PhotoUploadResult> {
  return apiPost<PhotoUploadResult>(`/api/documentation/uploads/${sessionId}/complete`, {});
}
