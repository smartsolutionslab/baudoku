import React, { useState } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
} from "react-native";
import { router } from "expo-router";
import { Colors, Spacing, FontSize } from "../../../src/styles/tokens";
import { loginWithKeycloak, parseUserFromToken } from "../../../src/auth/keycloak";
import { saveTokens } from "../../../src/auth/tokenStorage";
import { useAuthStore } from "../../../src/store/useAuthStore";
import { setAuthToken } from "../../../src/sync/apiClient";

export default function LoginScreen() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const { setTokens, setUser } = useAuthStore();

  const handleLogin = async () => {
    setLoading(true);
    setError(null);

    try {
      const tokens = await loginWithKeycloak();

      await saveTokens(tokens.accessToken, tokens.refreshToken);

      setTokens(tokens.accessToken, tokens.refreshToken);
      setAuthToken(tokens.accessToken);

      const user = parseUserFromToken(tokens.idToken);
      setUser(user);

      router.back();
    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Unbekannter Fehler bei der Anmeldung";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.content}>
        <Text style={styles.title}>BauDoku</Text>
        <Text style={styles.subtitle}>
          Melden Sie sich an, um Ihre Daten mit dem Server zu synchronisieren und
          auf allen Geräten verfügbar zu machen.
        </Text>

        {error && (
          <View style={styles.errorBox}>
            <Text style={styles.errorText}>{error}</Text>
          </View>
        )}

        <TouchableOpacity
          style={[styles.loginButton, loading && styles.loginButtonDisabled]}
          onPress={handleLogin}
          disabled={loading}
          activeOpacity={0.7}
        >
          {loading ? (
            <ActivityIndicator color="#FFFFFF" />
          ) : (
            <Text style={styles.loginButtonText}>Mit Keycloak anmelden</Text>
          )}
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.backButton}
          onPress={() => router.back()}
          activeOpacity={0.7}
        >
          <Text style={styles.backButtonText}>Zurück</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
    justifyContent: "center",
  },
  content: {
    padding: Spacing.xl,
    alignItems: "center",
  },
  title: {
    fontSize: 32,
    fontWeight: "700",
    color: Colors.primary,
    marginBottom: Spacing.md,
  },
  subtitle: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
    textAlign: "center",
    marginBottom: Spacing.xl,
    lineHeight: 22,
  },
  errorBox: {
    backgroundColor: "#FFF0F0",
    borderRadius: 8,
    padding: Spacing.md,
    marginBottom: Spacing.lg,
    width: "100%",
  },
  errorText: {
    color: Colors.danger,
    fontSize: FontSize.body,
    textAlign: "center",
  },
  loginButton: {
    backgroundColor: Colors.primary,
    borderRadius: 12,
    paddingVertical: 14,
    paddingHorizontal: Spacing.xl,
    width: "100%",
    alignItems: "center",
    marginBottom: Spacing.md,
  },
  loginButtonDisabled: {
    opacity: 0.6,
  },
  loginButtonText: {
    color: "#FFFFFF",
    fontSize: FontSize.headline,
    fontWeight: "600",
  },
  backButton: {
    paddingVertical: Spacing.md,
  },
  backButtonText: {
    color: Colors.textTertiary,
    fontSize: FontSize.body,
  },
});
