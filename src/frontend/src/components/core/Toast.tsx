import React, { useEffect, useRef } from "react";
import {
  Animated,
  Text,
  StyleSheet,
  Pressable,
} from "react-native";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";
import type { ToastType } from "../../store/useToastStore";
import { useToastStore } from "../../store/useToastStore";

const backgroundColors: Record<ToastType, string> = {
  success: Colors.success,
  error: Colors.danger,
  warning: Colors.warning,
  info: Colors.primary,
};

type ToastProps = {
  id: string;
  message: string;
  type: ToastType;
  duration: number;
};

export function Toast({ id, message, type, duration }: ToastProps) {
  const dismiss = useToastStore((s) => s.dismiss);
  const opacity = useRef(new Animated.Value(0)).current;
  const translateY = useRef(new Animated.Value(20)).current;
  const dismissTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    Animated.parallel([
      Animated.timing(opacity, {
        toValue: 1,
        duration: 200,
        useNativeDriver: true,
      }),
      Animated.timing(translateY, {
        toValue: 0,
        duration: 200,
        useNativeDriver: true,
      }),
    ]).start();

    dismissTimer.current = setTimeout(() => {
      Animated.parallel([
        Animated.timing(opacity, {
          toValue: 0,
          duration: 300,
          useNativeDriver: true,
        }),
        Animated.timing(translateY, {
          toValue: 20,
          duration: 300,
          useNativeDriver: true,
        }),
      ]).start(() => dismiss(id));
    }, duration);

    return () => {
      if (dismissTimer.current) clearTimeout(dismissTimer.current);
    };
  }, [id, duration, dismiss, opacity, translateY]);

  const handlePress = () => {
    if (dismissTimer.current) clearTimeout(dismissTimer.current);
    Animated.parallel([
      Animated.timing(opacity, {
        toValue: 0,
        duration: 200,
        useNativeDriver: true,
      }),
      Animated.timing(translateY, {
        toValue: 20,
        duration: 200,
        useNativeDriver: true,
      }),
    ]).start(() => dismiss(id));
  };

  return (
    <Animated.View
      style={[
        styles.container,
        { backgroundColor: backgroundColors[type], opacity, transform: [{ translateY }] },
      ]}
      accessibilityRole="alert"
      accessibilityLiveRegion="polite"
    >
      <Pressable onPress={handlePress} style={styles.pressable}>
        <Text style={styles.text}>{message}</Text>
      </Pressable>
    </Animated.View>
  );
}

const styles = StyleSheet.create({
  container: {
    borderRadius: Radius.md,
    marginBottom: Spacing.sm,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.15,
    shadowRadius: 6,
    elevation: 4,
  },
  pressable: {
    paddingVertical: Spacing.md,
    paddingHorizontal: Spacing.lg,
  },
  text: {
    color: "#fff",
    fontSize: FontSize.body,
    fontWeight: "500",
  },
});
