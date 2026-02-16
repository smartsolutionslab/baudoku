import { Stack } from "expo-router";

export default function CaptureLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" />
      <Stack.Screen
        name="new"
        options={{ headerShown: true, title: "Neue Installation" }}
      />
      <Stack.Screen
        name="scan"
        options={{ headerShown: false }}
      />
    </Stack>
  );
}
