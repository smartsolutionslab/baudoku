import React, { useState, useCallback } from "react";
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  TextInput,
} from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import * as installationRepo from "../../../../src/db/repositories/installationRepo";
import {
  usePhotosByInstallation,
  useMeasurementsByInstallation,
  useAddPhoto,
  useDeletePhoto,
  useUpdatePhotoAnnotation,
  useAddMeasurement,
  useDeleteMeasurement,
  useDeleteInstallation,
} from "../../../../src/hooks/useOfflineData";
import {
  usePhotoCapture,
  type CapturedPhoto,
} from "../../../../src/hooks/usePhotoCapture";
import { useConfirmDelete } from "../../../../src/hooks/useConfirmDelete";
import { deletePhotoFile } from "../../../../src/utils/photoStorage";
import { StatusBadge } from "../../../../src/components/common/StatusBadge";
import { EmptyState } from "../../../../src/components/common/EmptyState";
import { ActionBar } from "../../../../src/components/common/ActionBar";
import { InstallationDetails } from "../../../../src/components/installations/InstallationDetails";
import { PhotoGallery } from "../../../../src/components/installations/PhotoGallery";
import { PhotoSourceSheet } from "../../../../src/components/installations/PhotoSourceSheet";
import { PhotoTypeSheet, type PhotoType } from "../../../../src/components/installations/PhotoTypeSheet";
import { PhotoViewer } from "../../../../src/components/installations/PhotoViewer";
import { MeasurementCard } from "../../../../src/components/installations/MeasurementCard";
import { MeasurementForm } from "../../../../src/components/installations/MeasurementForm";
import { BottomSheet } from "../../../../src/components/common/BottomSheet";
import { Button } from "../../../../src/components/core/Button";
import { Colors, Spacing, FontSize, Radius } from "../../../../src/styles/tokens";
import type { Photo } from "../../../../src/db/repositories/types";
import type { MeasurementFormData } from "../../../../src/validation/schemas";
import { installationId } from "../../../../src/types/branded";
import type { MeasurementId } from "../../../../src/types/branded";

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

export default function InstallationDetailScreen() {
  const { id: rawId } = useLocalSearchParams<{ id: string }>();
  const id = installationId(rawId!);
  const router = useRouter();
  const queryClient = useQueryClient();

  const { data: installation } = useQuery({
    queryKey: ["installation", id],
    queryFn: () => installationRepo.getById(id),
    enabled: !!id,
  });
  const { data: photos } = usePhotosByInstallation(id);
  const { data: measurements } = useMeasurementsByInstallation(id);

  const addPhoto = useAddPhoto();
  const deletePhoto = useDeletePhoto();
  const { mutateAsync: saveAnnotation } = useUpdatePhotoAnnotation();
  const addMeasurement = useAddMeasurement();
  const deleteMeasurement = useDeleteMeasurement();
  const deleteInstallation = useDeleteInstallation();
  const { takePhoto, pickFromGallery } = usePhotoCapture();
  const { confirmDelete } = useConfirmDelete();

  // Photo flow state
  const [showSourceSheet, setShowSourceSheet] = useState(false);
  const [showTypeSheet, setShowTypeSheet] = useState(false);
  const [showCaptionModal, setShowCaptionModal] = useState(false);
  const [pendingPhoto, setPendingPhoto] = useState<CapturedPhoto | null>(null);
  const [pendingPhotoType, setPendingPhotoType] = useState<PhotoType | null>(null);
  const [captionText, setCaptionText] = useState("");
  const [viewerPhoto, setViewerPhoto] = useState<Photo | null>(null);
  const [showViewer, setShowViewer] = useState(false);

  // Measurement form state
  const [showMeasurementForm, setShowMeasurementForm] = useState(false);

  const handleCameraCapture = useCallback(async () => {
    setShowSourceSheet(false);
    const result = await takePhoto();
    if (result) {
      setPendingPhoto(result);
      setShowTypeSheet(true);
    }
  }, [takePhoto]);

  const handleGalleryPick = useCallback(async () => {
    setShowSourceSheet(false);
    const result = await pickFromGallery();
    if (result) {
      setPendingPhoto(result);
      setShowTypeSheet(true);
    }
  }, [pickFromGallery]);

  const handlePhotoTypeSelect = useCallback(
    (type: PhotoType) => {
      setShowTypeSheet(false);
      setPendingPhotoType(type);
      setCaptionText("");
      setShowCaptionModal(true);
    },
    []
  );

  const handleCaptionConfirm = useCallback(async () => {
    setShowCaptionModal(false);
    if (!pendingPhoto || !pendingPhotoType) return;
    try {
      await addPhoto.mutateAsync({
        installationId: id,
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
  }, [pendingPhoto, pendingPhotoType, captionText, id, addPhoto]);

  const handleDeletePhoto = useCallback(
    (photo: Photo) => {
      setShowViewer(false);
      confirmDelete({
        title: "Foto löschen",
        message: "Dieses Foto wirklich löschen?",
        onConfirm: async () => {
          try {
            await deletePhoto.mutateAsync(photo.id);
            deletePhotoFile(photo.localPath);
          } catch {
            // Global MutationCache.onError shows toast
          }
        },
      });
    },
    [deletePhoto, confirmDelete]
  );

  const handleDeleteMeasurement = useCallback(
    (m: { id: MeasurementId }) => {
      confirmDelete({
        title: "Messung löschen",
        message: "Diese Messung wirklich löschen?",
        onConfirm: async () => {
          try {
            await deleteMeasurement.mutateAsync(m.id);
          } catch {
            // Global MutationCache.onError shows toast
          }
        },
      });
    },
    [deleteMeasurement, confirmDelete]
  );

  const handleAddMeasurement = useCallback(
    async (data: MeasurementFormData) => {
      try {
        await addMeasurement.mutateAsync({
          installationId: id,
          type: data.type,
          value: data.value,
          unit: data.unit,
          minThreshold: data.minThreshold ?? null,
          maxThreshold: data.maxThreshold ?? null,
          notes: data.notes || null,
          measuredBy: data.measuredBy,
          measuredAt: new Date(),
        });
        setShowMeasurementForm(false);
      } catch {
        // Global MutationCache.onError shows toast
      }
    },
    [id, addMeasurement]
  );

  const handleDeleteInstallation = useCallback(() => {
    confirmDelete({
      title: "Installation löschen",
      message:
        "Diese Installation und alle zugehörigen Daten wirklich löschen?",
      onConfirm: async () => {
        try {
          await deleteInstallation.mutateAsync(id);
          router.back();
        } catch {
          // Global MutationCache.onError shows toast
        }
      },
    });
  }, [id, deleteInstallation, confirmDelete, router]);

  if (!installation) return null;

  const subtitle = [installation.manufacturer, installation.model]
    .filter(Boolean)
    .join(" — ");

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: installation.type }} />
      <ScrollView contentContainerStyle={styles.scroll}>
        {/* Basis-Info */}
        <View style={styles.card}>
          <View style={styles.headerRow}>
            <Text style={styles.type}>{installation.type}</Text>
            <StatusBadge status={installation.status} />
          </View>
          {subtitle ? (
            <Text style={styles.subtitle}>{subtitle}</Text>
          ) : null}
          {installation.serialNumber ? (
            <Text style={styles.detail}>
              SN: {installation.serialNumber}
            </Text>
          ) : null}
          {installation.notes ? (
            <Text style={styles.notes}>{installation.notes}</Text>
          ) : null}
        </View>

        {/* ActionBar */}
        <ActionBar
          actions={[
            {
              icon: "camera",
              label: "Foto",
              onPress: () => setShowSourceSheet(true),
            },
            {
              icon: "bar-chart",
              label: "Messung",
              onPress: () => setShowMeasurementForm(true),
            },
            {
              icon: "pencil",
              label: "Bearbeiten",
              onPress: () =>
                router.push(
                  `/(tabs)/projects/installation/edit?id=${id}`
                ),
            },
            {
              icon: "trash",
              label: "Löschen",
              onPress: handleDeleteInstallation,
              color: Colors.danger,
            },
          ]}
        />

        {/* Technische Details */}
        <InstallationDetails installation={installation} />

        {/* Fotos */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Fotos</Text>
          <PhotoGallery
            photos={photos ?? []}
            onPhotoPress={(photo) => {
              setViewerPhoto(photo);
              setShowViewer(true);
            }}
            onAddPhoto={() => setShowSourceSheet(true)}
          />
          {photos && photos.length > 0 && (
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

        {/* Messungen */}
        <View style={styles.measurementsSection}>
          <Text style={styles.cardTitle}>Messungen</Text>
          {showMeasurementForm && (
            <View style={styles.formContainer}>
              <MeasurementForm
                onSubmit={handleAddMeasurement}
                onCancel={() => setShowMeasurementForm(false)}
                submitting={addMeasurement.isPending}
              />
            </View>
          )}
          {!measurements || measurements.length === 0 ? (
            <EmptyState icon="bar-chart" title="Noch keine Messungen" />
          ) : (
            measurements.map((m) => (
              <MeasurementCard
                key={m.id}
                measurement={m}
                onDelete={handleDeleteMeasurement}
              />
            ))
          )}
        </View>
      </ScrollView>

      {/* Photo flow modals */}
      <PhotoSourceSheet
        visible={showSourceSheet}
        onCamera={handleCameraCapture}
        onGallery={handleGalleryPick}
        onClose={() => setShowSourceSheet(false)}
      />
      <PhotoTypeSheet
        visible={showTypeSheet}
        onSelect={handlePhotoTypeSelect}
        onClose={() => {
          setShowTypeSheet(false);
          setPendingPhoto(null);
        }}
      />

      {/* Caption input */}
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
          void saveAnnotation({ id: photoId, annotation });
        }}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  scroll: {
    paddingBottom: 40,
  },
  card: {
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
    borderRadius: 12,
    padding: Spacing.lg,
  },
  headerRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: Spacing.xs,
  },
  type: {
    fontSize: FontSize.title,
    fontWeight: "700",
    color: Colors.textPrimary,
    flex: 1,
    marginRight: Spacing.sm,
  },
  subtitle: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
    marginBottom: Spacing.xs,
  },
  detail: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  notes: {
    fontSize: FontSize.body,
    color: Colors.textSecondary,
    marginTop: Spacing.sm,
  },
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginBottom: Spacing.md,
  },
  measurementsSection: {
    paddingHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
  },
  formContainer: {
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
