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

export function getAuthToken(): string | null {
  return authToken;
}

export function onUnauthorized(listener: UnauthorizedListener): () => void {
  unauthorizedListeners.push(listener);
  return () => {
    const index = unauthorizedListeners.indexOf(listener);
    if (index >= 0) unauthorizedListeners.splice(index, 1);
  };
}

export function _handleUnauthorized(): void {
  for (const listener of unauthorizedListeners) {
    listener();
  }
}
