import {
  View,
  Image,
  Text,
  TouchableOpacity,
  StyleSheet,
  Dimensions,
  ActivityIndicator,
} from "react-native";
import { FontAwesome } from "@expo/vector-icons";
import type { Photo } from "../../db/repositories/types";
import { StatusBadge, EmptyState } from "../common";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";
import { PHOTO_TYPE_LABELS } from "../../constants";

type PhotoGalleryProps = {
  photos: Photo[];
  onPhotoPress?: (photo: Photo) => void;
  onAddPhoto?: () => void;
};

const SCREEN_WIDTH = Dimensions.get("window").width;
const GAP = Spacing.sm;
const IMAGE_SIZE = (SCREEN_WIDTH - Spacing.lg * 2 - Spacing.lg * 2 - GAP) / 2;

export function PhotoGallery({
  photos,
  onPhotoPress,
  onAddPhoto,
}: PhotoGalleryProps) {
  if (photos.length === 0 && !onAddPhoto) {
    return <EmptyState icon="camera" title="Noch keine Fotos" />;
  }

  return (
    <View style={styles.grid}>
      {photos.map((photo) => (
        <TouchableOpacity
          key={photo.id}
          style={styles.item}
          onPress={() => onPhotoPress?.(photo)}
          activeOpacity={onPhotoPress ? 0.7 : 1}
        >
          <Image
            source={{ uri: photo.localPath }}
            style={styles.image}
            resizeMode="cover"
          />
          <View style={styles.badge}>
            <StatusBadge
              status={photo.type}
              label={PHOTO_TYPE_LABELS[photo.type] ?? photo.type}
            />
          </View>
          {photo.uploadStatus === "uploading" && (
            <View style={styles.uploadOverlay}>
              <ActivityIndicator size="small" color={Colors.card} />
            </View>
          )}
          {photo.uploadStatus === "failed" && (
            <View style={styles.uploadOverlay}>
              <FontAwesome name="warning" size={18} color={Colors.danger} />
            </View>
          )}
          {photo.uploadStatus === "pending" && (
            <View style={styles.uploadOverlay}>
              <FontAwesome name="clock-o" size={16} color={Colors.card} />
            </View>
          )}
        </TouchableOpacity>
      ))}

      {onAddPhoto && (
        <TouchableOpacity
          style={[styles.item, styles.addItem]}
          onPress={onAddPhoto}
          activeOpacity={0.7}
        >
          <FontAwesome name="camera" size={28} color={Colors.textTertiary} />
          <Text style={styles.addText}>Hinzufügen</Text>
        </TouchableOpacity>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  grid: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: GAP,
  },
  item: {
    width: IMAGE_SIZE,
    height: IMAGE_SIZE,
    borderRadius: Radius.sm,
    overflow: "hidden",
    backgroundColor: Colors.separator,
  },
  image: {
    width: "100%",
    height: "100%",
  },
  badge: {
    position: "absolute",
    top: 4,
    left: 4,
  },
  addItem: {
    backgroundColor: Colors.background,
    borderWidth: 2,
    borderColor: Colors.separator,
    borderStyle: "dashed",
    justifyContent: "center",
    alignItems: "center",
  },
  addText: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
    marginTop: Spacing.xs,
  },
  uploadOverlay: {
    position: "absolute",
    bottom: 4,
    right: 4,
    backgroundColor: Colors.overlay,
    borderRadius: Radius.lg,
    width: 24,
    height: 24,
    justifyContent: "center",
    alignItems: "center",
  },
});
