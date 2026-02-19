import { useEffect, useState } from "react";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import { DarkTheme, DefaultTheme, ThemeProvider } from "@react-navigation/native";
import { useFonts } from "expo-font";
import { Stack } from "expo-router";
import * as SplashScreen from "expo-splash-screen";
import { useColorScheme } from "react-native";

import { QueryProvider, AuthProvider, SyncProvider } from "../src/providers";
import { useMigrationsHelper } from "../src/db/useMigrationsHelper";
import { startConnectivityMonitor, stopConnectivityMonitor } from "../src/sync/ConnectivityMonitor";
import { useSettingsStore } from "../src/store";
import { OfflineBanner } from "../src/components/sync";
import { ToastContainer } from "../src/components/core";
import { View, Text, ActivityIndicator, StyleSheet } from "react-native";
import { Colors, Spacing, FontSize } from "../src/styles/tokens";

export { ErrorBoundary } from "expo-router";

export const unstable_settings = {
  initialRouteName: "(tabs)",
};

SplashScreen.preventAutoHideAsync();

export default function RootLayout() {
  const [loaded, error] = useFonts({
    SpaceMono: require("../assets/fonts/SpaceMono-Regular.ttf"),
    ...FontAwesome.font,
  });
  const [timedOut, setTimedOut] = useState(false);

  useEffect(() => {
    if (error) {
      console.warn("Font loading error:", error.message);
    }
  }, [error]);

  useEffect(() => {
    if (loaded) {
      SplashScreen.hideAsync();
    }
  }, [loaded]);

  // Safety timeout: dismiss splash and continue without custom fonts
  // if font loading hangs (e.g. on CI emulators)
  useEffect(() => {
    const timer = setTimeout(() => {
      if (!loaded) {
        console.warn("Font loading timed out after 10s, continuing without custom fonts");
        SplashScreen.hideAsync();
        setTimedOut(true);
      }
    }, 10000);
    return () => clearTimeout(timer);
  }, [loaded]);

  if (!loaded && !timedOut) {
    return null;
  }

  return (
    <QueryProvider>
      <RootLayoutNav />
    </QueryProvider>
  );
}

function RootLayoutNav() {
  const colorScheme = useColorScheme();
  const { migrationSuccess, migrationError } = useMigrationsHelper();

  useEffect(() => {
    startConnectivityMonitor();
    useSettingsStore.getState().hydrate();
    return () => stopConnectivityMonitor();
  }, []);

  if (migrationError) {
    return (
      <View style={styles.center}>
        <Text style={styles.errorText}>
          Datenbankfehler: {migrationError.message}
        </Text>
      </View>
    );
  }

  if (!migrationSuccess) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" />
        <Text style={styles.loadingText}>Datenbank wird initialisiert...</Text>
      </View>
    );
  }

  return (
    <ThemeProvider value={colorScheme === "dark" ? DarkTheme : DefaultTheme}>
      <AuthProvider>
        <SyncProvider>
          <OfflineBanner />
          <ToastContainer />
          <Stack>
            <Stack.Screen name="index" options={{ headerShown: false }} />
            <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
            <Stack.Screen name="+not-found" options={{ headerShown: false }} />
          </Stack>
        </SyncProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}

const styles = StyleSheet.create({
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    padding: Spacing.xl,
  },
  errorText: {
    color: Colors.danger,
    fontSize: FontSize.callout,
    textAlign: "center",
  },
  loadingText: {
    marginTop: Spacing.md,
    fontSize: FontSize.body,
    color: Colors.textSecondary,
  },
});
