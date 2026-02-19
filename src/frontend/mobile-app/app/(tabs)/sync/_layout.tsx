import { Stack } from "expo-router";
import { ScreenErrorFallback } from "@/components/common";

export function ErrorBoundary(props: { error: Error; retry: () => void }) {
  return <ScreenErrorFallback {...props} />;
}

export default function SyncLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" />
      <Stack.Screen
        name="conflicts"
        options={{ headerShown: true, title: "Konflikte" }}
      />
      <Stack.Screen
        name="conflict/[id]"
        options={{ headerShown: true, title: "Konflikt" }}
      />
    </Stack>
  );
}
