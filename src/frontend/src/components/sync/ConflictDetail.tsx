import React from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Alert,
  ScrollView,
} from "react-native";
import { Colors } from "../../styles/tokens";
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
    backgroundColor: "#F2F2F7",
  },
  header: {
    flexDirection: "row",
    alignItems: "center",
    padding: 16,
    backgroundColor: Colors.white,
    gap: 8,
  },
  badge: {
    backgroundColor: "#007AFF",
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 12,
  },
  badgeText: {
    color: Colors.white,
    fontSize: 12,
    fontWeight: "600",
    textTransform: "capitalize",
  },
  entityId: {
    flex: 1,
    fontSize: 13,
    color: "#8E8E93",
    fontFamily: "SpaceMono",
  },
  versionRow: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    backgroundColor: Colors.white,
    borderTopWidth: StyleSheet.hairlineWidth,
    borderTopColor: "#E5E5EA",
  },
  versionLabel: {
    fontSize: 14,
    color: "#666",
  },
  sectionTitle: {
    fontSize: 17,
    fontWeight: "600",
    margin: 16,
    marginBottom: 8,
  },
  actions: {
    flexDirection: "row",
    padding: 16,
    gap: 12,
  },
  button: {
    flex: 1,
    paddingVertical: 14,
    borderRadius: 10,
    alignItems: "center",
  },
  clientButton: {
    backgroundColor: "#007AFF",
  },
  serverButton: {
    backgroundColor: "#34C759",
  },
  buttonText: {
    color: Colors.white,
    fontSize: 16,
    fontWeight: "600",
  },
});
