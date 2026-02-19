import "@expo/metro-runtime";
import React, { useEffect, useState } from "react";
import { Text, View } from "react-native";
import { ExpoRoot } from "expo-router";
import { renderRootComponent } from "expo-router/build/renderRootComponent";
import * as SplashScreen from "expo-splash-screen";

// Explicit require.context so route discovery resolves relative to this file
const ctx = require.context(
  "./app",
  true,
  /^(?:\.\/)(?!(?:(?:(?:.*\+api)|(?:\+html)|(?:\+middleware)))\.[tj]sx?$).*(?:\.ios|\.web)?\.[tj]sx?$/
);

// Debug: log discovered routes
console.log("[BauDoku] require.context keys:", JSON.stringify(ctx.keys()));

// Debug: test route loading
try {
  const layoutModule = ctx("./_layout.tsx");
  console.log("[BauDoku] _layout.tsx exports:", Object.keys(layoutModule));
} catch (e) {
  console.error("[BauDoku] FAILED to load _layout.tsx:", e.message);
}

// Debug: test route tree building
try {
  const { getRoutes } = require("expo-router/build/getRoutes");
  const routeTree = getRoutes(ctx, {
    skipGenerated: true,
    ignoreEntryPoints: true,
    platform: "android",
  });
  console.log("[BauDoku] Route tree:", routeTree ? "BUILT" : "NULL");
  if (routeTree) {
    console.log("[BauDoku] Root route:", routeTree.route, "type:", routeTree.type);
    console.log("[BauDoku] Children:", routeTree.children?.map(c => c.route).join(", "));
  }
} catch (e) {
  console.error("[BauDoku] getRoutes FAILED:", e.message);
}

// Error boundary to catch rendering crashes
class DebugErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { error: null };
  }
  static getDerivedStateFromError(error) {
    return { error };
  }
  componentDidCatch(error, info) {
    console.error("[BauDoku] RENDER ERROR:", error.message);
    console.error("[BauDoku] STACK:", error.stack?.substring(0, 500));
    console.error("[BauDoku] COMPONENT STACK:", info.componentStack?.substring(0, 500));
  }
  render() {
    if (this.state.error) {
      return (
        <View style={{ flex: 1, justifyContent: "center", alignItems: "center", backgroundColor: "red" }}>
          <Text style={{ color: "white", fontSize: 18, textAlign: "center", padding: 20 }}>
            ERROR: {this.state.error.message}
          </Text>
        </View>
      );
    }
    return this.props.children;
  }
}

function App() {
  const [debugInfo, setDebugInfo] = useState("mounting...");

  useEffect(() => {
    console.log("[BauDoku] App mounted");

    // Force-hide splash after 5s to reveal actual screen state
    const timer = setTimeout(() => {
      console.log("[BauDoku] Force-hiding splash screen after 5s");
      SplashScreen.hideAsync().then(() => {
        console.log("[BauDoku] Splash hidden successfully");
      }).catch((e) => {
        console.error("[BauDoku] Splash hide failed:", e.message);
      });
      setDebugInfo("splash hidden, waiting for navigation...");
    }, 5000);

    // Log periodic state after mount
    const interval = setInterval(() => {
      console.log("[BauDoku] App still alive, debugInfo:", debugInfo);
    }, 10000);

    return () => {
      clearTimeout(timer);
      clearInterval(interval);
    };
  }, []);

  return (
    <DebugErrorBoundary>
      <ExpoRoot context={ctx} />
    </DebugErrorBoundary>
  );
}

renderRootComponent(App);
