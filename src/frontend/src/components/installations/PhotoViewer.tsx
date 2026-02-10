import React from "react";
import {
  View,
  Text,
  Image,
  TouchableOpacity,
  Modal,
  StyleSheet,
  SafeAreaView,
} from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import type { Photo } from "../../db/repositories/types";
import { StatusBadge } from "../common/StatusBadge";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

const photoTypeLabels: Record<string, string> = {
  before: "Vorher",
  after: "Nachher",
  detail: "Detail",
  overview: "Übersicht",
};

interface PhotoViewerProps {
  photo: Photo | null;
  visible: boolean;
  onClose: () => void;
  onDelete: (photo: Photo) => void;
}

export function PhotoViewer({
  photo,
  visible,
  onClose,
  onDelete,
}: PhotoViewerProps) {
  if (!photo) return null;

  return (
    <Modal visible={visible} animationType="fade" statusBarTranslucent>
      <SafeAreaView style={styles.container}>
        <View style={styles.header}>
          <TouchableOpacity onPress={onClose} style={styles.closeButton}>
            <FontAwesome name="close" size={22} color="#fff" />
          </TouchableOpacity>
          <TouchableOpacity
            onPress={() => onDelete(photo)}
            style={styles.deleteButton}
          >
            <FontAwesome name="trash" size={20} color={Colors.danger} />
          </TouchableOpacity>
        </View>

        <Image
          source={{ uri: photo.localPath }}
          style={styles.image}
          resizeMode="contain"
        />

        <View style={styles.footer}>
          <StatusBadge
            status={photo.type}
            label={photoTypeLabels[photo.type] ?? photo.type}
          />
          <Text style={styles.date}>
            {photo.takenAt
              ? new Date(photo.takenAt).toLocaleDateString("de-DE", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                })
              : ""}
          </Text>
        </View>
      </SafeAreaView>
    </Modal>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#000",
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
  },
  closeButton: {
    padding: Spacing.sm,
  },
  deleteButton: {
    padding: Spacing.sm,
  },
  image: {
    flex: 1,
  },
  footer: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
  },
  date: {
    fontSize: FontSize.caption,
    color: "#aaa",
  },
});
