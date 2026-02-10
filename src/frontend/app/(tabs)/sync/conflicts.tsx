import React from "react";
import { View, Text, ActivityIndicator, StyleSheet } from "react-native";
import { useRouter } from "expo-router";
import { ConflictList } from "../../../src/components/sync/ConflictList";
import { useConflicts } from "../../../src/hooks/useConflicts";

export default function ConflictsScreen() {
  const router = useRouter();
  const { data: conflicts, isLoading, error } = useConflicts("unresolved");

  if (isLoading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  if (error) {
    return (
      <View style={styles.center}>
        <Text style={styles.errorText}>
          Fehler beim Laden: {error.message}
        </Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <ConflictList
        conflicts={conflicts ?? []}
        onSelect={(conflict) =>
          router.push(`/(tabs)/sync/conflict/${conflict.id}`)
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#F2F2F7",
  },
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  errorText: {
    color: "#FF3B30",
    fontSize: 15,
    textAlign: "center",
    padding: 20,
  },
});
