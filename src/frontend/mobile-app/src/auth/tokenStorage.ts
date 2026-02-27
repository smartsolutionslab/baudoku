import * as SecureStore from "expo-secure-store";

const ACCESS_TOKEN_KEY = "baudoku_access_token";
const REFRESH_TOKEN_KEY = "baudoku_refresh_token";
const ID_TOKEN_KEY = "baudoku_id_token";

export async function saveTokens(accessToken: string, refreshToken: string, idToken: string): Promise<void> {
  await SecureStore.setItemAsync(ACCESS_TOKEN_KEY, accessToken);
  await SecureStore.setItemAsync(REFRESH_TOKEN_KEY, refreshToken);
  await SecureStore.setItemAsync(ID_TOKEN_KEY, idToken);
}

export async function loadTokens(): Promise<{
  accessToken: string | null;
  refreshToken: string | null;
  idToken: string | null;
}> {
  const accessToken = await SecureStore.getItemAsync(ACCESS_TOKEN_KEY);
  const refreshToken = await SecureStore.getItemAsync(REFRESH_TOKEN_KEY);
  const idToken = await SecureStore.getItemAsync(ID_TOKEN_KEY);
  return { accessToken, refreshToken, idToken };
}

export async function clearTokens(): Promise<void> {
  await SecureStore.deleteItemAsync(ACCESS_TOKEN_KEY);
  await SecureStore.deleteItemAsync(REFRESH_TOKEN_KEY);
  await SecureStore.deleteItemAsync(ID_TOKEN_KEY);
}
