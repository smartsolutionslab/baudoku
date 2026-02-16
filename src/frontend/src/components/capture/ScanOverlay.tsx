import { View, Text, StyleSheet, Dimensions } from "react-native";
import { Colors, FontSize, Radius } from "../../styles/tokens";

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
    fontSize: FontSize.callout,
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
    borderRadius: Radius.xl,
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
    borderTopLeftRadius: Radius.xl,
  },
  topRight: {
    top: 0,
    right: 0,
    borderTopWidth: CORNER_WIDTH,
    borderRightWidth: CORNER_WIDTH,
    borderColor: Colors.white,
    borderTopRightRadius: Radius.xl,
  },
  bottomLeft: {
    bottom: 0,
    left: 0,
    borderBottomWidth: CORNER_WIDTH,
    borderLeftWidth: CORNER_WIDTH,
    borderColor: Colors.white,
    borderBottomLeftRadius: Radius.xl,
  },
  bottomRight: {
    bottom: 0,
    right: 0,
    borderBottomWidth: CORNER_WIDTH,
    borderRightWidth: CORNER_WIDTH,
    borderColor: Colors.white,
    borderBottomRightRadius: Radius.xl,
  },
});
