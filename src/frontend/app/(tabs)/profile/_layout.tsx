import { Stack } from "expo-router";
import { ScreenErrorFallback } from "../../../src/components/common/ScreenErrorFallback";

export function ErrorBoundary(props: { error: Error; retry: () => void }) {
  return <ScreenErrorFallback {...props} />;
}

export default function ProfileLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" />
      <Stack.Screen name="login" />
    </Stack>
  );
}
