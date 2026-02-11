import React, { useState, useCallback } from "react";
import { View, Text, ScrollView, StyleSheet, Alert } from "react-native";
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
import { usePhotoCapture } from "../../../../src/hooks/usePhotoCapture";
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
import { Colors, Spacing, FontSize } from "../../../../src/styles/tokens";
import type { Photo } from "../../../../src/db/repositories/types";
import type { MeasurementFormData } from "../../../../src/validation/schemas";

export default function InstallationDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const router = useRouter();
  const queryClient = useQueryClient();

  const { data: installation } = useQuery({
    queryKey: ["installation", id],
    queryFn: () => installationRepo.getById(id!),
    enabled: !!id,
  });
  const { data: photos } = usePhotosByInstallation(id!);
  const { data: measurements } = useMeasurementsByInstallation(id!);

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
  const [pendingPhotoPath, setPendingPhotoPath] = useState<string | null>(null);
  const [viewerPhoto, setViewerPhoto] = useState<Photo | null>(null);
  const [showViewer, setShowViewer] = useState(false);

  // Measurement form state
  const [showMeasurementForm, setShowMeasurementForm] = useState(false);

  const handleCameraCapture = useCallback(async () => {
    setShowSourceSheet(false);
    const result = await takePhoto();
    if (result) {
      setPendingPhotoPath(result.localPath);
      setShowTypeSheet(true);
    }
  }, [takePhoto]);

  const handleGalleryPick = useCallback(async () => {
    setShowSourceSheet(false);
    const result = await pickFromGallery();
    if (result) {
      setPendingPhotoPath(result.localPath);
      setShowTypeSheet(true);
    }
  }, [pickFromGallery]);

  const handlePhotoTypeSelect = useCallback(
    async (type: PhotoType) => {
      setShowTypeSheet(false);
      if (!pendingPhotoPath) return;
      await addPhoto.mutateAsync({
        installationId: id!,
        localPath: pendingPhotoPath,
        type,
        takenAt: new Date(),
        uploadStatus: "pending",
      });
      setPendingPhotoPath(null);
    },
    [pendingPhotoPath, id, addPhoto]
  );

  const handleDeletePhoto = useCallback(
    (photo: Photo) => {
      setShowViewer(false);
      confirmDelete({
        title: "Foto löschen",
        message: "Dieses Foto wirklich löschen?",
        onConfirm: async () => {
          await deletePhoto.mutateAsync(photo.id);
          deletePhotoFile(photo.localPath);
        },
      });
    },
    [deletePhoto, confirmDelete]
  );

  const handleDeleteMeasurement = useCallback(
    (m: { id: string }) => {
      confirmDelete({
        title: "Messung löschen",
        message: "Diese Messung wirklich löschen?",
        onConfirm: async () => {
          await deleteMeasurement.mutateAsync(m.id);
        },
      });
    },
    [deleteMeasurement, confirmDelete]
  );

  const handleAddMeasurement = useCallback(
    async (data: MeasurementFormData) => {
      await addMeasurement.mutateAsync({
        installationId: id!,
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
    },
    [id, addMeasurement]
  );

  const handleDeleteInstallation = useCallback(() => {
    confirmDelete({
      title: "Installation löschen",
      message:
        "Diese Installation und alle zugehörigen Daten wirklich löschen?",
      onConfirm: async () => {
        await deleteInstallation.mutateAsync(id!);
        router.back();
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
          setPendingPhotoPath(null);
        }}
      />
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
});
