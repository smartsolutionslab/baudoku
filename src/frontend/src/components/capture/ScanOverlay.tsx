import React from "react";
import { View, Text, StyleSheet, Dimensions } from "react-native";
import { Colors } from "../../styles/tokens";

const { width } = Dimensions.get("window");
const SCAN_SIZE = width * 0.7;

export function ScanOverlay() {
  return (
    <View style={styles.container}>
      <View style={styles.topOverlay}>
        <Text style={styles.hint}>QR-Code in den Rahmen halten</Text>
      </View>
      <View style={styles.middleRow}>
        <View style={styles.sideOverlay} />
        <View style={styles.scanWindow}>
          <View style={[styles.corner, styles.topLeft]} />
          <View style={[styles.corner, styles.topRight]} />
          <View style={[styles.corner, styles.bottomLeft]} />
          <View style={[styles.corner, styles.bottomRight]} />
        </View>
        <View style={styles.sideOverlay} />
      </View>
      <View style={styles.bottomOverlay} />
    </View>
  );
}

const CORNER_SIZE = 24;
const CORNER_WIDTH = 4;

const styles = StyleSheet.create({
  container: {
    ...StyleSheet.absoluteFillObject,
  },
  topOverlay: {
    flex: 1,
    backgroundColor: Colors.overlay,
    justifyContent: "flex-end",
    alignItems: "center",
    paddingBottom: 20,
  },
  hint: {
    color: Colors.white,
    fontSize: 16,
    fontWeight: "600",
  },
  middleRow: {
    flexDirection: "row",
    height: SCAN_SIZE,
  },
  sideOverlay: {
    flex: 1,
    backgroundColor: Colors.overlay,
  },
  scanWindow: {
    width: SCAN_SIZE,
    height: SCAN_SIZE,
    borderRadius: 16,
  },
  bottomOverlay: {
    flex: 1,
    backgroundColor: Colors.overlay,
  },
  corner: {
    position: "absolute",
    width: CORNER_SIZE,
    height: CORNER_SIZE,
  },
  topLeft: {
    top: 0,
    left: 0,
    borderTopWidth: CORNER_WIDTH,
    borderLeftWidth: CORNER_WIDTH,
    borderColor: Colors.white,
    borderTopLeftRadius: 16,
  },
  topRight: {
    top: 0,
    right: 0,
    borderTopWidth: CORNER_WIDTH,
    borderRightWidth: CORNER_WIDTH,
    borderColor: Colors.white,
    borderTopRightRadius: 16,
  },
  bottomLeft: {
    bottom: 0,
    left: 0,
    borderBottomWidth: CORNER_WIDTH,
    borderLeftWidth: CORNER_WIDTH,
    borderColor: Colors.white,
    borderBottomLeftRadius: 16,
  },
  bottomRight: {
    bottom: 0,
    right: 0,
    borderBottomWidth: CORNER_WIDTH,
    borderRightWidth: CORNER_WIDTH,
    borderColor: Colors.white,
    borderBottomRightRadius: 16,
  },
});
