import { View, Text, ActivityIndicator, StyleSheet } from "react-native";
import { useLocalSearchParams } from "expo-router";
import { ConflictDetail } from "../../../../src/components/sync";
import { useConflicts, useResolveConflict } from "../../../../src/hooks";
import { Colors, FontSize } from "../../../../src/styles/tokens";

export default function ConflictDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const { data: conflicts, isLoading } = useConflicts();
  const resolveConflict = useResolveConflict();

  const conflict = conflicts?.find((c) => c.id === id);

  if (isLoading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  if (!conflict) {
    return (
      <View style={styles.center}>
        <Text style={styles.errorText}>Konflikt nicht gefunden</Text>
      </View>
    );
  }

  return (
    <ConflictDetail
      conflict={conflict}
      isResolving={resolveConflict.isPending}
      onResolve={(strategy, mergedPayload) => {
        resolveConflict.mutate({
          conflictId: conflict.id,
          strategy,
          mergedPayload,
        });
      }}
    />
  );
}

const styles = StyleSheet.create({
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  errorText: {
    color: Colors.danger,
    fontSize: FontSize.body,
  },
});
