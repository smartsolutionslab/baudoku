import { useEffect, useRef } from "react";
import { Animated, Text, StyleSheet } from "react-native";
import { Colors, Spacing, FontSize } from "../../styles/tokens";
import { useSyncStatus } from "../../hooks/useSyncStatus";

export function OfflineBanner() {
  const { isOnline } = useSyncStatus();
  const translateY = useRef(new Animated.Value(-50)).current;

  useEffect(() => {
    Animated.timing(translateY, {
      toValue: isOnline ? -50 : 0,
      duration: 300,
      useNativeDriver: true,
    }).start();
  }, [isOnline, translateY]);

  return (
    <Animated.View
      style={[styles.banner, { transform: [{ translateY }] }]}
      pointerEvents={isOnline ? "none" : "auto"}
    >
      <Text style={styles.text}>
        Keine Internetverbindung — Daten werden lokal gespeichert
      </Text>
    </Animated.View>
  );
}

const styles = StyleSheet.create({
  banner: {
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    backgroundColor: Colors.danger,
    paddingVertical: Spacing.sm,
    paddingHorizontal: Spacing.lg,
    zIndex: 100,
  },
  text: {
    color: Colors.white,
    fontSize: FontSize.caption,
    textAlign: "center",
    fontWeight: "500",
  },
});
