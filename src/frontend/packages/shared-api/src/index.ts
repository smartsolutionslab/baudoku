import type {
  SyncDeltaDto,
  ProcessSyncBatchResult,
  ChangeSetResult,
  ConflictDto,
  PhotoUploadResult,
} from "@baudoku/shared-types";

// ─── Configuration ──────────────────────────────────────────────

let baseUrl = "";
let authToken: string | null = null;

type UnauthorizedListener = () => void;
const unauthorizedListeners: UnauthorizedListener[] = [];

export function setBaseUrl(url: string): void {
  baseUrl = url.replace(/\/$/, "");
}

export function getBaseUrl(): string {
  return baseUrl;
}

export function setAuthToken(token: string | null): void {
  authToken = token;
}

export function onUnauthorized(listener: UnauthorizedListener): () => void {
  unauthorizedListeners.push(listener);
  return () => {
    const index = unauthorizedListeners.indexOf(listener);
    if (index >= 0) unauthorizedListeners.splice(index, 1);
  };
}

// ─── HTTP Primitives ────────────────────────────────────────────

function getHeaders(contentType?: string): Record<string, string> {
  const headers: Record<string, string> = {};
  if (contentType) headers["Content-Type"] = contentType;
  if (authToken) headers["Authorization"] = `Bearer ${authToken}`;
  return headers;
}

function handleUnauthorized(): void {
  for (const listener of unauthorizedListeners) {
    listener();
  }
}

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly statusText: string,
    public readonly body: unknown
  ) {
    super(`API Error ${status}: ${statusText}`);
    this.name = "ApiError";
  }
}

export async function apiGet<T>(path: string): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: "GET",
    headers: getHeaders("application/json"),
  });

  if (!response.ok) {
    if (response.status === 401) handleUnauthorized();
    const body = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, body);
  }

  return response.json() as Promise<T>;
}

export async function apiPost<T>(path: string, body: unknown): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: "POST",
    headers: getHeaders("application/json"),
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    if (response.status === 401) handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export async function apiPut<T>(path: string, body: unknown): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: "PUT",
    headers: getHeaders("application/json"),
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    if (response.status === 401) handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export async function apiDelete(path: string): Promise<void> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: "DELETE",
    headers: getHeaders("application/json"),
  });

  if (!response.ok) {
    if (response.status === 401) handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }
}

export async function apiUpload<T>(
  path: string,
  formData: FormData
): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: "POST",
    headers: getHeaders(),
    body: formData,
  });

  if (!response.ok) {
    if (response.status === 401) handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

// ─── Typed API Functions ────────────────────────────────────────

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

export async function uploadPhoto(
  installationId: string,
  file: File,
  caption?: string
): Promise<PhotoUploadResult> {
  const formData = new FormData();
  formData.append("file", file);
  if (caption) formData.append("caption", caption);

  return apiUpload<PhotoUploadResult>(
    `/api/documentation/installations/${installationId}/photos`,
    formData
  );
}
