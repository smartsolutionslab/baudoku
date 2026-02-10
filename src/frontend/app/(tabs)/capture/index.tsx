import React from "react";
import { View, Text, StyleSheet } from "react-native";

export default function CaptureScreen() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Erfassen</Text>
      <Text style={styles.subtitle}>
        Installationserfassung wird in Sprint 6 implementiert
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    padding: 20,
    backgroundColor: "#F2F2F7",
  },
  title: {
    fontSize: 24,
    fontWeight: "600",
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 15,
    color: "#8E8E93",
    textAlign: "center",
  },
});
