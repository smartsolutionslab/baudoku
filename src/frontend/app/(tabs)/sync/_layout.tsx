import { Stack } from "expo-router";

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
