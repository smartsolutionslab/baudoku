import { getBaseUrl, getAuthToken, _handleUnauthorized } from "./auth";

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

function getHeaders(contentType?: string): Record<string, string> {
  const headers: Record<string, string> = {};
  if (contentType) headers["Content-Type"] = contentType;
  const token = getAuthToken();
  if (token) headers["Authorization"] = `Bearer ${token}`;
  return headers;
}

export async function apiGet<T>(path: string): Promise<T> {
  const response = await fetch(`${getBaseUrl()}${path}`, {
    method: "GET",
    headers: getHeaders("application/json"),
  });

  if (!response.ok) {
    if (response.status === 401) _handleUnauthorized();
    const body = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, body);
  }

  return response.json() as Promise<T>;
}

export async function apiPost<T>(path: string, body: unknown): Promise<T> {
  const response = await fetch(`${getBaseUrl()}${path}`, {
    method: "POST",
    headers: getHeaders("application/json"),
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    if (response.status === 401) _handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export async function apiPut<T>(path: string, body: unknown): Promise<T> {
  const response = await fetch(`${getBaseUrl()}${path}`, {
    method: "PUT",
    headers: getHeaders("application/json"),
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    if (response.status === 401) _handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export async function apiDelete(path: string): Promise<void> {
  const response = await fetch(`${getBaseUrl()}${path}`, {
    method: "DELETE",
    headers: getHeaders("application/json"),
  });

  if (!response.ok) {
    if (response.status === 401) _handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }
}

export async function apiUpload<T>(
  path: string,
  formData: FormData
): Promise<T> {
  const response = await fetch(`${getBaseUrl()}${path}`, {
    method: "POST",
    headers: getHeaders(),
    body: formData,
  });

  if (!response.ok) {
    if (response.status === 401) _handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export async function apiRawUpload(
  path: string,
  body: Blob | ArrayBuffer,
  extraHeaders?: Record<string, string>
): Promise<Response> {
  const headers: Record<string, string> = {
    ...extraHeaders,
  };
  const token = getAuthToken();
  if (token) headers["Authorization"] = `Bearer ${token}`;

  const response = await fetch(`${getBaseUrl()}${path}`, {
    method: "POST",
    headers,
    body,
  });

  if (!response.ok) {
    if (response.status === 401) _handleUnauthorized();
    const responseBody = await response.text().catch(() => "");
    throw new ApiError(response.status, response.statusText, responseBody);
  }

  return response;
}
