import { View, Text, TextInput, StyleSheet } from 'react-native';
import { BottomSheet } from '@/components/common';
import { Button } from '@/components/core';
import {
  PhotoGallery,
  PhotoSourceSheet,
  PhotoTypeSheet,
  type PhotoType,
  PhotoViewer,
} from '@/components/installations';
import { usePhotoFlow } from '@/hooks/usePhotoFlow';
import { Colors, Spacing, FontSize, Radius } from '@/styles/tokens';
import { UPLOAD_STATUS_COLORS, UPLOAD_STATUS_LABELS } from '@/utils/uploadStatus';
import type { Photo } from '@/db/repositories/types';
import type { InstallationId, PhotoId, Latitude, Longitude } from '@baudoku/core';

const UPLOAD_STATUSES = ['pending', 'uploading', 'uploaded', 'failed'] as const;

type InstallationPhotoSectionProps = {
  installationId: InstallationId;
  photos: Photo[];
  onAddPhoto: (params: {
    installationId: InstallationId;
    localPath: string;
    type: PhotoType;
    caption: string | null;
    exifLatitude: Latitude | null;
    exifLongitude: Longitude | null;
    exifDateTime: string | null;
    exifCameraModel: string | null;
    takenAt: Date;
    uploadStatus: 'pending';
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
  const {
    handleCameraCapture,
    handleGalleryPick,
    showTypeSheet,
    onCloseTypeSheet,
    handlePhotoTypeSelect,
    showCaptionModal,
    onCloseCaptionModal,
    captionText,
    setCaptionText,
    handleCaptionConfirm,
    viewerPhoto,
    showViewer,
    openViewer,
    closeViewer,
    handleDeletePhoto,
  } = usePhotoFlow({
    installationId,
    onAddPhoto,
    onDeletePhoto,
    onShowSourceSheet,
  });

  return (
    <>
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Fotos</Text>
        <PhotoGallery
          photos={photos}
          onPhotoPress={openViewer}
          onAddPhoto={() => onShowSourceSheet(true)}
        />
        {photos.length > 0 && (
          <View style={styles.uploadSummary}>
            {UPLOAD_STATUSES.map((status) => {
              const count = photos.filter(({ uploadStatus }) => uploadStatus === status).length;
              if (count === 0) return null;
              return (
                <View
                  key={status}
                  style={[styles.uploadBadge, { backgroundColor: UPLOAD_STATUS_COLORS[status] ?? Colors.textTertiary }]}
                >
                  <Text style={styles.uploadBadgeText}>
                    {count} {UPLOAD_STATUS_LABELS[status] ?? 'ausstehend'}
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
        onClose={onCloseTypeSheet}
      />
      <BottomSheet
        visible={showCaptionModal}
        onClose={onCloseCaptionModal}
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
          title={captionText.trim() ? 'Hinzufügen' : 'Überspringen'}
          onPress={handleCaptionConfirm}
        />
      </BottomSheet>
      <PhotoViewer
        photo={viewerPhoto}
        visible={showViewer}
        onClose={closeViewer}
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
    fontWeight: '600',
    marginBottom: Spacing.md,
  },
  uploadSummary: {
    flexDirection: 'row',
    flexWrap: 'wrap',
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
    fontWeight: '600',
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
