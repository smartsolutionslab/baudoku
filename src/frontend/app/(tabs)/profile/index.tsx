import React from "react";
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
} from "react-native";
import { router } from "expo-router";
import { useProjects } from "../../../src/hooks/useOfflineData";
import { useSyncStatus } from "../../../src/hooks/useSyncStatus";
import { useAuthStore } from "../../../src/store/useAuthStore";
import { clearTokens } from "../../../src/auth/tokenStorage";
import { setAuthToken } from "../../../src/sync/apiClient";
import { Colors, Spacing, FontSize } from "../../../src/styles/tokens";

export default function ProfileScreen() {
  const { data: projects } = useProjects();
  const { isOnline, unsyncedCount, lastSyncTimestamp } = useSyncStatus();
  const { isAuthenticated, user, clearAuth } = useAuthStore();

  const projectCount = projects?.length ?? 0;

  const handleLogout = async () => {
    clearAuth();
    setAuthToken(null);
    await clearTokens();
  };

  const displayRole = (roles: string[]) => {
    if (roles.includes("admin")) return "Admin";
    if (roles.includes("inspector")) return "Inspektor";
    if (roles.includes("user")) return "Benutzer";
    return roles[0] ?? "Unbekannt";
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.screenTitle}>Profil</Text>

      {/* App-Info */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>App-Info</Text>
        <Row label="App" value="BauDoku" />
        <Row label="Version" value="1.0.0" />
        <Row label="Build" value="Sprint 10" />
      </View>

      {/* Anmeldung */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Anmeldung</Text>
        {isAuthenticated && user ? (
          <>
            <Row label="Name" value={user.name} />
            <Row label="Email" value={user.email} />
            <Row label="Rolle" value={displayRole(user.roles)} />
            <TouchableOpacity
              style={styles.logoutButton}
              onPress={handleLogout}
              activeOpacity={0.7}
            >
              <Text style={styles.logoutButtonText}>Abmelden</Text>
            </TouchableOpacity>
          </>
        ) : (
          <>
            <Text style={styles.notLoggedInText}>Nicht angemeldet</Text>
            <TouchableOpacity
              style={styles.loginButton}
              onPress={() => router.push("/(tabs)/profile/login")}
              activeOpacity={0.7}
            >
              <Text style={styles.loginButtonText}>Anmelden</Text>
            </TouchableOpacity>
          </>
        )}
      </View>

      {/* Gerät */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Gerät</Text>
        <View style={styles.row}>
          <Text style={styles.label}>Online-Status</Text>
          <View style={styles.statusValue}>
            <View
              style={[
                styles.dot,
                { backgroundColor: isOnline ? Colors.success : Colors.danger },
              ]}
            />
            <Text style={styles.value}>
              {isOnline ? "Online" : "Offline"}
            </Text>
          </View>
        </View>
      </View>

      {/* Datenbank */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Datenbank</Text>
        <Row label="Projekte" value={String(projectCount)} />
      </View>

      {/* Sync */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Synchronisation</Text>
        <Row label="Ausstehend" value={`${unsyncedCount} Änderungen`} />
        <Row
          label="Letzte Sync"
          value={lastSyncTimestamp ?? "Noch nicht synchronisiert"}
        />
      </View>
    </ScrollView>
  );
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <View style={styles.row}>
      <Text style={styles.label}>{label}</Text>
      <Text style={styles.value}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  content: {
    padding: Spacing.lg,
    paddingBottom: 40,
  },
  screenTitle: {
    fontSize: FontSize.title,
    fontWeight: "700",
    marginBottom: Spacing.lg,
  },
  card: {
    backgroundColor: Colors.card,
    borderRadius: 12,
    padding: Spacing.lg,
    marginBottom: Spacing.lg,
    gap: Spacing.md,
  },
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
  },
  row: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  label: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
  value: {
    fontSize: FontSize.body,
    fontWeight: "500",
    color: Colors.textPrimary,
  },
  statusValue: {
    flexDirection: "row",
    alignItems: "center",
  },
  dot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6,
  },
  notLoggedInText: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
    fontStyle: "italic",
  },
  loginButton: {
    backgroundColor: Colors.primary,
    borderRadius: 10,
    paddingVertical: 12,
    alignItems: "center",
  },
  loginButtonText: {
    color: "#FFFFFF",
    fontSize: FontSize.body,
    fontWeight: "600",
  },
  logoutButton: {
    backgroundColor: Colors.danger,
    borderRadius: 10,
    paddingVertical: 12,
    alignItems: "center",
  },
  logoutButtonText: {
    color: "#FFFFFF",
    fontSize: FontSize.body,
    fontWeight: "600",
  },
});
