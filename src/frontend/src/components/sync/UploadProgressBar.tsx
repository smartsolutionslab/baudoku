import { useEffect, useRef } from "react";
import { View, Text, StyleSheet, Animated } from "react-native";
import { useUploadStore } from "../../store/useUploadStore";
import { Colors, Spacing, FontSize, Radius, Shadows } from "../../styles/tokens";

export function UploadProgressBar() {
  const queue = useUploadStore((s) => s.queue);
  const dismissTimer = useRef<ReturnType<typeof setTimeout> | null>(null);
  const opacity = useRef(new Animated.Value(0)).current;

  const total = queue.length;
  const completed = queue.filter((i) => i.status === "completed").length;
  const failed = queue.filter((i) => i.status === "failed").length;
  const uploading = queue.filter(
    (i) => i.status === "uploading" || i.status === "queued"
  ).length;
  const allDone = total > 0 && uploading === 0;
  const hasActive = uploading > 0;

  const overallPercentage =
    total > 0
      ? Math.round(
          queue.reduce((sum, item) => sum + item.percentage, 0) / total
        )
      : 0;

  useEffect(() => {
    if (hasActive || allDone) {
      Animated.timing(opacity, {
        toValue: 1,
        duration: 200,
        useNativeDriver: true,
      }).start();
    }

    if (allDone) {
      dismissTimer.current = setTimeout(() => {
        Animated.timing(opacity, {
          toValue: 0,
          duration: 300,
          useNativeDriver: true,
        }).start(() => {
          useUploadStore.getState().removeCompleted();
        });
      }, 3000);
    }

    return () => {
      if (dismissTimer.current) clearTimeout(dismissTimer.current);
    };
  }, [hasActive, allDone, opacity]);

  if (total === 0) return null;

  const statusText = allDone
    ? `${completed} Foto${completed !== 1 ? "s" : ""} hochgeladen${failed > 0 ? `, ${failed} fehlgeschlagen` : ""}`
    : `Lade ${completed + 1}/${total} Fotos hoch...`;

  return (
    <Animated.View style={[styles.container, { opacity }]}>
      <Text style={styles.text}>{statusText}</Text>
      <View style={styles.track}>
        <View
          style={[
            styles.fill,
            {
              width: `${overallPercentage}%`,
              backgroundColor: allDone
                ? failed > 0
                  ? Colors.warning
                  : Colors.success
                : Colors.primary,
            },
          ]}
        />
      </View>
    </Animated.View>
  );
}

const styles = StyleSheet.create({
  container: {
    position: "absolute",
    bottom: Spacing.lg,
    left: Spacing.lg,
    right: Spacing.lg,
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    padding: Spacing.md,
    ...Shadows.floating,
  },
  text: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  track: {
    height: 4,
    backgroundColor: Colors.separator,
    borderRadius: 2,
    overflow: "hidden",
  },
  fill: {
    height: "100%",
    borderRadius: 2,
  },
});
