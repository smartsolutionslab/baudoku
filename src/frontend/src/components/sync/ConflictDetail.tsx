import { View, Text, StyleSheet, TouchableOpacity, Alert, ScrollView } from "react-native";
import { Colors, Spacing, FontSize, Radius, FontFamily } from "../../styles/tokens";
import { ConflictDiffView } from "./ConflictDiffView";
import type { ConflictDto } from "../../sync/syncApi";

type ConflictDetailProps = {
  conflict: ConflictDto;
  onResolve: (strategy: string, mergedPayload?: string) => void;
  isResolving: boolean;
};

export function ConflictDetail({
  conflict,
  onResolve,
  isResolving,
}: ConflictDetailProps) {
  const handleClientWins = () => {
    Alert.alert(
      "Eigene Version verwenden",
      "Soll Ihre lokale Version die Server-Version ersetzen?",
      [
        { text: "Abbrechen", style: "cancel" },
        {
          text: "Ja, meine Version",
          onPress: () => onResolve("client_wins"),
        },
      ]
    );
  };

  const handleServerWins = () => {
    Alert.alert(
      "Server-Version verwenden",
      "Soll die Server-Version Ihre lokale Version ersetzen?",
      [
        { text: "Abbrechen", style: "cancel" },
        {
          text: "Ja, Server-Version",
          onPress: () => onResolve("server_wins"),
        },
      ]
    );
  };

  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        <View style={styles.badge}>
          <Text style={styles.badgeText}>{conflict.entityType}</Text>
        </View>
        <Text style={styles.entityId} numberOfLines={1}>
          {conflict.entityId}
        </Text>
      </View>

      <View style={styles.versionRow}>
        <Text style={styles.versionLabel}>
          Client v{conflict.clientVersion} → Server v{conflict.serverVersion}
        </Text>
      </View>

      <Text style={styles.sectionTitle}>Vergleich</Text>
      <ConflictDiffView
        clientPayload={conflict.clientPayload}
        serverPayload={conflict.serverPayload}
      />

      <View style={styles.actions}>
        <TouchableOpacity
          style={[styles.button, styles.clientButton]}
          onPress={handleClientWins}
          disabled={isResolving}
        >
          <Text style={styles.buttonText}>Meine Version</Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.button, styles.serverButton]}
          onPress={handleServerWins}
          disabled={isResolving}
        >
          <Text style={styles.buttonText}>Server-Version</Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    flexDirection: "row",
    alignItems: "center",
    padding: Spacing.lg,
    backgroundColor: Colors.white,
    gap: Spacing.sm,
  },
  badge: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.sm,
    paddingVertical: Spacing.xs,
    borderRadius: Radius.lg,
  },
  badgeText: {
    color: Colors.white,
    fontSize: FontSize.footnote,
    fontWeight: "600",
    textTransform: "capitalize",
  },
  entityId: {
    flex: 1,
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    fontFamily: FontFamily.mono,
  },
  versionRow: {
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.sm,
    backgroundColor: Colors.white,
    borderTopWidth: StyleSheet.hairlineWidth,
    borderTopColor: Colors.separator,
  },
  versionLabel: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
  },
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    margin: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  actions: {
    flexDirection: "row",
    padding: Spacing.lg,
    gap: Spacing.md,
  },
  button: {
    flex: 1,
    paddingVertical: 14,
    borderRadius: Radius.md,
    alignItems: "center",
  },
  clientButton: {
    backgroundColor: Colors.primary,
  },
  serverButton: {
    backgroundColor: Colors.success,
  },
  buttonText: {
    color: Colors.white,
    fontSize: FontSize.callout,
    fontWeight: "600",
  },
});
