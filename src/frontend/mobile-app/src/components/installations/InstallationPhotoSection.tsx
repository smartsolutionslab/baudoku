import { useState, useCallback } from "react";
import { View, Text, TextInput, StyleSheet } from "react-native";
import { BottomSheet } from "@/components/common";
import { Button } from "@/components/core";
import {
  PhotoGallery,
  PhotoSourceSheet,
  PhotoTypeSheet,
  type PhotoType,
  PhotoViewer,
} from "@/components/installations";
import { usePhotoCapture, type CapturedPhoto, useConfirmDelete } from "@/hooks";
import { deletePhotoFile } from "@/utils";
import { Colors, Spacing, FontSize, Radius } from "@/styles/tokens";
import type { Photo } from "@/db/repositories/types";
import type { InstallationId, PhotoId } from "@/types/branded";

function uploadStatusColor(status: string): string {
  switch (status) {
    case "uploaded": return Colors.success;
    case "uploading": return Colors.primary;
    case "failed": return Colors.danger;
    default: return Colors.textTertiary;
  }
}

function uploadStatusLabel(status: string): string {
  switch (status) {
    case "uploaded": return "hochgeladen";
    case "uploading": return "lädt hoch";
    case "failed": return "fehlgeschlagen";
    default: return "ausstehend";
  }
}

type InstallationPhotoSectionProps = {
  installationId: InstallationId;
  photos: Photo[];
  onAddPhoto: (params: {
    installationId: InstallationId;
    localPath: string;
    type: PhotoType;
    caption: string | null;
    exifLatitude: number | null;
    exifLongitude: number | null;
    exifDateTime: string | null;
    exifCameraModel: string | null;
    takenAt: Date;
    uploadStatus: "pending";
  }) => Promise<unknown>;
  onDeletePhoto: (id: PhotoId) => Promise<unknown>;
  onSaveAnnotation: (photoId: PhotoId, annotation: string) => void;
  showSourceSheet: boolean;
  onShowSourceSheet: (show: boolean) => void;
};

export function InstallationPhotoSection({
  installationId,
  photos,
  onAddPhoto,
  onDeletePhoto,
  onSaveAnnotation,
  showSourceSheet,
  onShowSourceSheet,
}: InstallationPhotoSectionProps) {
  const { takePhoto, pickFromGallery } = usePhotoCapture();
  const { confirmDelete } = useConfirmDelete();

  const [showTypeSheet, setShowTypeSheet] = useState(false);
  const [showCaptionModal, setShowCaptionModal] = useState(false);
  const [pendingPhoto, setPendingPhoto] = useState<CapturedPhoto | null>(null);
  const [pendingPhotoType, setPendingPhotoType] = useState<PhotoType | null>(null);
  const [captionText, setCaptionText] = useState("");
  const [viewerPhoto, setViewerPhoto] = useState<Photo | null>(null);
  const [showViewer, setShowViewer] = useState(false);

  const handleCameraCapture = useCallback(async () => {
    onShowSourceSheet(false);
    const result = await takePhoto();
    if (result) {
      setPendingPhoto(result);
      setShowTypeSheet(true);
    }
  }, [takePhoto, onShowSourceSheet]);

  const handleGalleryPick = useCallback(async () => {
    onShowSourceSheet(false);
    const result = await pickFromGallery();
    if (result) {
      setPendingPhoto(result);
      setShowTypeSheet(true);
    }
  }, [pickFromGallery, onShowSourceSheet]);

  const handlePhotoTypeSelect = useCallback((type: PhotoType) => {
    setShowTypeSheet(false);
    setPendingPhotoType(type);
    setCaptionText("");
    setShowCaptionModal(true);
  }, []);

  const handleCaptionConfirm = useCallback(async () => {
    setShowCaptionModal(false);
    if (!pendingPhoto || !pendingPhotoType) return;
    try {
      await onAddPhoto({
        installationId,
        localPath: pendingPhoto.localPath,
        type: pendingPhotoType,
        caption: captionText.trim() || null,
        exifLatitude: pendingPhoto.exif?.gpsLatitude ?? null,
        exifLongitude: pendingPhoto.exif?.gpsLongitude ?? null,
        exifDateTime: pendingPhoto.exif?.dateTime ?? null,
        exifCameraModel: pendingPhoto.exif?.cameraModel ?? null,
        takenAt: new Date(),
        uploadStatus: "pending",
      });
    } catch {
      // Global MutationCache.onError shows toast
    }
    setPendingPhoto(null);
    setPendingPhotoType(null);
    setCaptionText("");
  }, [pendingPhoto, pendingPhotoType, captionText, installationId, onAddPhoto]);

  const handleDeletePhoto = useCallback(
    (photo: Photo) => {
      setShowViewer(false);
      confirmDelete({
        title: "Foto löschen",
        message: "Dieses Foto wirklich löschen?",
        onConfirm: async () => {
          try {
            await onDeletePhoto(photo.id);
            deletePhotoFile(photo.localPath);
          } catch {
            // Global MutationCache.onError shows toast
          }
        },
      });
    },
    [onDeletePhoto, confirmDelete]
  );

  return (
    <>
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Fotos</Text>
        <PhotoGallery
          photos={photos}
          onPhotoPress={(photo) => {
            setViewerPhoto(photo);
            setShowViewer(true);
          }}
          onAddPhoto={() => onShowSourceSheet(true)}
        />
        {photos.length > 0 && (
          <View style={styles.uploadSummary}>
            {(['pending', 'uploading', 'uploaded', 'failed'] as const).map((status) => {
              const count = photos.filter((p) => p.uploadStatus === status).length;
              if (count === 0) return null;
              return (
                <View key={status} style={[styles.uploadBadge, { backgroundColor: uploadStatusColor(status) }]}>
                  <Text style={styles.uploadBadgeText}>
                    {count} {uploadStatusLabel(status)}
                  </Text>
                </View>
              );
            })}
          </View>
        )}
      </View>

      <PhotoSourceSheet
        visible={showSourceSheet}
        onCamera={handleCameraCapture}
        onGallery={handleGalleryPick}
        onClose={() => onShowSourceSheet(false)}
      />
      <PhotoTypeSheet
        visible={showTypeSheet}
        onSelect={handlePhotoTypeSelect}
        onClose={() => {
          setShowTypeSheet(false);
          setPendingPhoto(null);
        }}
      />
      <BottomSheet
        visible={showCaptionModal}
        onClose={() => {
          setShowCaptionModal(false);
          setPendingPhoto(null);
          setPendingPhotoType(null);
        }}
        title="Beschriftung (optional)"
      >
        <TextInput
          style={styles.captionInput}
          value={captionText}
          onChangeText={setCaptionText}
          placeholder="z.B. Kabeltrasse Nordseite"
          placeholderTextColor={Colors.textTertiary}
          maxLength={200}
          autoFocus
        />
        <Button
          title={captionText.trim() ? "Hinzufügen" : "Überspringen"}
          onPress={handleCaptionConfirm}
        />
      </BottomSheet>
      <PhotoViewer
        photo={viewerPhoto}
        visible={showViewer}
        onClose={() => setShowViewer(false)}
        onDelete={handleDeletePhoto}
        onSaveAnnotation={(photoId, annotation) => {
          onSaveAnnotation(photoId, annotation);
        }}
      />
    </>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
  },
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginBottom: Spacing.md,
  },
  uploadSummary: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: Spacing.xs,
    marginTop: Spacing.sm,
  },
  uploadBadge: {
    paddingHorizontal: Spacing.sm,
    paddingVertical: 2,
    borderRadius: Radius.md,
  },
  uploadBadgeText: {
    color: Colors.white,
    fontSize: FontSize.footnote,
    fontWeight: "600",
  },
  captionInput: {
    borderWidth: 1,
    borderColor: Colors.separator,
    borderRadius: Radius.md,
    padding: Spacing.md,
    fontSize: FontSize.body,
    color: Colors.textPrimary,
    marginBottom: Spacing.lg,
  },
});
