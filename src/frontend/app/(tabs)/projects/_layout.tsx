import { Stack } from "expo-router";

export default function ProjectsLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" />
      <Stack.Screen
        name="new"
        options={{ headerShown: true, title: "Neues Projekt" }}
      />
      <Stack.Screen
        name="[id]"
        options={{ headerShown: true, title: "Projekt" }}
      />
      <Stack.Screen
        name="zone/new"
        options={{ headerShown: true, title: "Neue Zone" }}
      />
      <Stack.Screen
        name="zone/[zoneId]"
        options={{ headerShown: true, title: "Zone" }}
      />
      <Stack.Screen
        name="installation/[id]"
        options={{ headerShown: true, title: "Installation" }}
      />
    </Stack>
  );
}
