import "@expo/metro-runtime";
import React from "react";
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
      return null;
    }
    return this.props.children;
  }
}

function App() {
  console.log("[BauDoku] App component rendering");
  return (
    <DebugErrorBoundary>
      <ExpoRoot context={ctx} />
    </DebugErrorBoundary>
  );
}

renderRootComponent(App);
