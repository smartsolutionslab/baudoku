import { View, StyleSheet } from "react-native";
import { useToastStore } from "../../store";
import { Toast } from "./Toast";
import { Spacing } from "../../styles/tokens";

export function ToastContainer() {
  const toasts = useToastStore((s) => s.toasts);

  if (toasts.length === 0) return null;

  return (
    <View style={styles.container} pointerEvents="box-none">
      {toasts.map((toast) => (
        <Toast
          key={toast.id}
          id={toast.id}
          message={toast.message}
          type={toast.type}
          duration={toast.duration}
        />
      ))}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    position: "absolute",
    bottom: 90,
    left: Spacing.lg,
    right: Spacing.lg,
    zIndex: 200,
  },
});
