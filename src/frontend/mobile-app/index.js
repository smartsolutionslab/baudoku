import "@expo/metro-runtime";
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

function App() {
  return <ExpoRoot context={ctx} />;
}

renderRootComponent(App);
