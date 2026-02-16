import { Stack } from "expo-router";
import { ScreenErrorFallback } from "../../../src/components/common";

export function ErrorBoundary(props: { error: Error; retry: () => void }) {
  return <ScreenErrorFallback {...props} />;
}

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
        name="edit"
        options={{ headerShown: true, title: "Projekt bearbeiten" }}
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
        name="zone/edit"
        options={{ headerShown: true, title: "Zone bearbeiten" }}
      />
      <Stack.Screen
        name="installation/[id]"
        options={{ headerShown: true, title: "Installation" }}
      />
      <Stack.Screen
        name="installation/edit"
        options={{ headerShown: true, title: "Installation bearbeiten" }}
      />
      <Stack.Screen
        name="search"
        options={{ headerShown: true, title: "Suche" }}
      />
      <Stack.Screen
        name="dashboard"
        options={{ headerShown: true, title: "Dashboard" }}
      />
    </Stack>
  );
}
