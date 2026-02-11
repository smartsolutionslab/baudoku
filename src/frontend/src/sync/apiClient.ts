import { API_BASE_URL } from "../config/environment";

let baseUrl = API_BASE_URL;
let authToken: string | null = null;

type UnauthorizedListener = () => void;
const unauthorizedListeners: UnauthorizedListener[] = [];

export function setBaseUrl(url: string): void {
  baseUrl = url.replace(/\/$/, "");
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
