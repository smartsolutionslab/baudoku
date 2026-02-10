import React from "react";
import { View, Image, Text, StyleSheet, Dimensions } from "react-native";
import type { Photo } from "../../db/repositories/types";
import { StatusBadge } from "../common/StatusBadge";
import { EmptyState } from "../common/EmptyState";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

const photoTypeLabels: Record<string, string> = {
  before: "Vorher",
  after: "Nachher",
  detail: "Detail",
  overview: "Übersicht",
};

interface PhotoGalleryProps {
  photos: Photo[];
}

const SCREEN_WIDTH = Dimensions.get("window").width;
const GAP = Spacing.sm;
const IMAGE_SIZE = (SCREEN_WIDTH - Spacing.lg * 2 - Spacing.lg * 2 - GAP) / 2;

export function PhotoGallery({ photos }: PhotoGalleryProps) {
  if (photos.length === 0) {
    return (
      <EmptyState icon="camera" title="Noch keine Fotos" />
    );
  }

  return (
    <View style={styles.grid}>
      {photos.map((photo) => (
        <View key={photo.id} style={styles.item}>
          <Image
            source={{ uri: photo.localPath }}
            style={styles.image}
            resizeMode="cover"
          />
          <View style={styles.badge}>
            <StatusBadge
              status={photo.type}
              label={photoTypeLabels[photo.type] ?? photo.type}
            />
          </View>
        </View>
      ))}
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
    borderRadius: 8,
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
});
