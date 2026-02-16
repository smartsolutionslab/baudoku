import { useEffect } from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type ScreenErrorFallbackProps = {
  error: Error;
  retry: () => void;
};

export function ScreenErrorFallback({ error, retry }: ScreenErrorFallbackProps) {
  useEffect(() => {
    console.error("ScreenErrorFallback:", error);
  }, [error]);

  return (
    <View style={styles.container}>
      <FontAwesome
        name="exclamation-triangle"
        size={48}
        color={Colors.danger}
        style={styles.icon}
      />
      <Text style={styles.title}>Etwas ist schiefgelaufen</Text>
      <Text style={styles.subtitle}>{error.message}</Text>
      <TouchableOpacity style={styles.button} onPress={retry}>
        <Text style={styles.buttonText}>Erneut versuchen</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    paddingHorizontal: Spacing.xl,
    paddingTop: 40,
  },
  icon: {
    marginBottom: Spacing.lg,
  },
  title: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    color: Colors.textPrimary,
    textAlign: "center",
    marginBottom: Spacing.sm,
  },
  subtitle: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
    textAlign: "center",
    marginBottom: Spacing.lg,
  },
  button: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.md,
    borderRadius: Radius.md,
  },
  buttonText: {
    color: Colors.white,
    fontSize: FontSize.body,
    fontWeight: "600",
  },
});
