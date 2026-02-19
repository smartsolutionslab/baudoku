import "@expo/metro-runtime";
import React, { useEffect } from "react";
import { ExpoRoot } from "expo-router";
import { renderRootComponent } from "expo-router/build/renderRootComponent";

// Explicit require.context so route discovery resolves relative to this file
// (mobile-app/), not relative to the hoisted node_modules/expo-router/_ctx.*.js.
// This is the standard monorepo workaround for expo-router.
const ctx = require.context(
  "./app",
  true,
  /^(?:\.\/)(?!(?:(?:(?:.*\+api)|(?:\+html)|(?:\+middleware)))\.[tj]sx?$).*(?:\.ios|\.web)?\.[tj]sx?$/
);

// Debug: log discovered routes so we can verify in logcat
console.log("[BauDoku] require.context keys:", JSON.stringify(ctx.keys()));

// Debug: try to load the root layout module to check for import errors
try {
  const layoutModule = ctx("./_layout.tsx");
  console.log("[BauDoku] _layout.tsx exports:", Object.keys(layoutModule));
} catch (e) {
  console.error("[BauDoku] FAILED to load _layout.tsx:", e.message);
}

// Debug: try to load getRoutes and see if route tree builds
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
  console.error("[BauDoku] getRoutes stack:", e.stack?.substring(0, 500));
}

// Debug error boundary to catch silent rendering crashes
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
    console.error("[BauDoku] COMPONENT STACK:", info.componentStack);
  }
  render() {
    if (this.state.error) {
      console.error("[BauDoku] ErrorBoundary caught error, rendering null");
      return null;
    }
    return this.props.children;
  }
}

function App() {
  console.log("[BauDoku] App component rendering");
  useEffect(() => {
    console.log("[BauDoku] App useEffect - component mounted");
  }, []);
  return (
    <DebugErrorBoundary>
      <ExpoRoot context={ctx} />
    </DebugErrorBoundary>
  );
}

renderRootComponent(App);
