import { useState, useEffect } from "react";
import {
  View,
  Text,
  Image,
  TextInput,
  TouchableOpacity,
  Modal,
  StyleSheet,
  SafeAreaView,
  KeyboardAvoidingView,
  Platform,
} from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import type { Photo } from "../../db/repositories/types";
import type { PhotoId } from "../../types/branded";
import { StatusBadge } from "../common";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";
import { formatDateTime } from "../../utils";
import { photoTypeLabels } from "../../constants";

type PhotoViewerProps = {
  photo: Photo | null;
  visible: boolean;
  onClose: () => void;
  onDelete: (photo: Photo) => void;
  onSaveAnnotation?: (photoId: PhotoId, annotation: string) => void;
};

export function PhotoViewer({
  photo,
  visible,
  onClose,
  onDelete,
  onSaveAnnotation,
}: PhotoViewerProps) {
  const [annotation, setAnnotation] = useState("");
  const [dirty, setDirty] = useState(false);

  useEffect(() => {
    if (photo) {
      setAnnotation(photo.annotations ?? "");
      setDirty(false);
    }
  }, [photo]);

  if (!photo) return null;

  const handleSave = () => {
    onSaveAnnotation?.(photo.id, annotation);
    setDirty(false);
  };

  return (
    <Modal visible={visible} animationType="fade" statusBarTranslucent>
      <SafeAreaView style={styles.container}>
        <KeyboardAvoidingView
          style={styles.flex}
          behavior={Platform.OS === "ios" ? "padding" : undefined}
        >
          <View style={styles.header}>
            <TouchableOpacity onPress={onClose} style={styles.closeButton}>
              <FontAwesome name="close" size={22} color={Colors.white} />
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
            <View style={styles.footerTop}>
              <StatusBadge
                status={photo.type}
                label={photoTypeLabels[photo.type] ?? photo.type}
              />
              <Text style={styles.date}>
                {formatDateTime(photo.takenAt ? new Date(photo.takenAt) : null)}
              </Text>
            </View>

            <View style={styles.annotationRow}>
              <TextInput
                style={styles.annotationInput}
                value={annotation}
                onChangeText={(text) => {
                  setAnnotation(text);
                  setDirty(true);
                }}
                placeholder="Notiz hinzufügen..."
                placeholderTextColor={Colors.textTertiary}
                multiline
                maxLength={500}
              />
              {dirty && (
                <TouchableOpacity
                  style={styles.saveBtn}
                  onPress={handleSave}
                >
                  <FontAwesome name="check" size={16} color={Colors.white} />
                </TouchableOpacity>
              )}
            </View>
          </View>
        </KeyboardAvoidingView>
      </SafeAreaView>
    </Modal>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.black,
  },
  flex: {
    flex: 1,
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
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
  },
  footerTop: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: Spacing.sm,
  },
  date: {
    fontSize: FontSize.caption,
    color: Colors.textQuaternary,
  },
  annotationRow: {
    flexDirection: "row",
    alignItems: "flex-end",
    gap: Spacing.sm,
  },
  annotationInput: {
    flex: 1,
    backgroundColor: "rgba(255,255,255,0.1)",
    borderRadius: Radius.sm,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    color: Colors.white,
    fontSize: FontSize.caption,
    maxHeight: 80,
  },
  saveBtn: {
    backgroundColor: Colors.primary,
    width: 36,
    height: 36,
    borderRadius: 18,
    justifyContent: "center",
    alignItems: "center",
  },
});
