import { View, Text, ActivityIndicator, StyleSheet } from "react-native";
import { useRouter } from "expo-router";
import { ConflictList } from "@/components/sync";
import { useConflicts } from "@/hooks";
import { Colors, FontSize, Spacing } from "@/styles/tokens";

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
    backgroundColor: Colors.background,
  },
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  errorText: {
    color: Colors.danger,
    fontSize: FontSize.body,
    textAlign: "center",
    padding: Spacing.xl,
  },
});
