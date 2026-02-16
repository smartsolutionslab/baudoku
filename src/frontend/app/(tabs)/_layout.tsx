import React from "react";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import { Tabs } from "expo-router";

import { Colors } from "../../src/styles/tokens";
import { SyncIndicator } from "../../src/components/sync/SyncIndicator";
import { useSyncStatus } from "../../src/hooks/useSyncStatus";

function TabBarIcon(props: {
  name: React.ComponentProps<typeof FontAwesome>["name"];
  color: string;
}) {
  return <FontAwesome size={24} style={{ marginBottom: -3 }} {...props} />;
}

function formatBadge(count: number): string | undefined {
  if (count === 0) return undefined;
  return count > 99 ? "99+" : String(count);
}

export default function TabLayout() {
  const { unsyncedCount, pendingUploads } = useSyncStatus();

  return (
    <Tabs
      screenOptions={{
        tabBarActiveTintColor: Colors.primary,
        headerShown: true,
      }}
    >
      <Tabs.Screen
        name="projects"
        options={{
          title: "Projekte",
          headerShown: false,
          tabBarIcon: ({ color }) => (
            <TabBarIcon name="building" color={color} />
          ),
          headerRight: () => <SyncIndicator />,
        }}
      />
      <Tabs.Screen
        name="capture"
        options={{
          title: "Erfassen",
          headerShown: false,
          tabBarIcon: ({ color }) => (
            <TabBarIcon name="camera" color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="sync"
        options={{
          title: "Sync",
          tabBarIcon: ({ color }) => (
            <TabBarIcon name="refresh" color={color} />
          ),
          tabBarBadge: formatBadge(unsyncedCount + pendingUploads),
        }}
      />
      <Tabs.Screen
        name="profile"
        options={{
          title: "Profil",
          headerShown: false,
          tabBarIcon: ({ color }) => <TabBarIcon name="user" color={color} />,
        }}
      />
    </Tabs>
  );
}
