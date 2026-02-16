import { useState } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
} from "react-native";
import { useRouter } from "expo-router";
import { Button } from "../../../src/components/core/Button";
import { Colors, Spacing, FontSize, Radius } from "../../../src/styles/tokens";
import { loginWithKeycloak, parseUserFromToken } from "../../../src/auth/keycloak";
import { saveTokens } from "../../../src/auth/tokenStorage";
import { useAuthStore } from "../../../src/store/useAuthStore";
import { setAuthToken } from "../../../src/sync/apiClient";

export default function LoginScreen() {
  const router = useRouter();
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

        <Button
          title="Mit Keycloak anmelden"
          onPress={handleLogin}
          loading={loading}
          style={{ marginBottom: Spacing.md }}
        />

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
    backgroundColor: Colors.errorBg,
    borderRadius: Radius.sm,
    padding: Spacing.md,
    marginBottom: Spacing.lg,
    width: "100%",
  },
  errorText: {
    color: Colors.danger,
    fontSize: FontSize.body,
    textAlign: "center",
  },
  backButton: {
    paddingVertical: Spacing.md,
  },
  backButtonText: {
    color: Colors.textTertiary,
    fontSize: FontSize.body,
  },
});
