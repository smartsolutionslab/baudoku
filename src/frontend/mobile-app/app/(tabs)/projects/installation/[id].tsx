import { useState, useCallback } from "react";
import { View, ScrollView, StyleSheet } from "react-native";
import { useLocalSearchParams, useRouter, Stack } from "expo-router";
import { useQuery } from "@tanstack/react-query";
import * as installationRepo from "@/db/repositories/installationRepo";
import {
  usePhotosByInstallation,
  useMeasurementsByInstallation,
  useAddPhoto,
  useDeletePhoto,
  useUpdatePhotoAnnotation,
  useAddMeasurement,
  useDeleteMeasurement,
  useDeleteInstallation,
  useConfirmDelete,
} from "@/hooks";
import { ActionBar } from "@/components/common";
import {
  InstallationInfoSection,
  InstallationPhotoSection,
  InstallationMeasurementSection,
} from "@/components/installations";
import { Colors, Spacing } from "@/styles/tokens";
import type { Measurement } from "@/db/repositories/types";
import type { MeasurementFormData } from "@/validation/schemas";
import { installationId } from "@/types/branded";

export default function InstallationDetailScreen() {
  const { id: rawId } = useLocalSearchParams<{ id: string }>();
  const id = installationId(rawId!);
  const router = useRouter();

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
  const { confirmDelete } = useConfirmDelete();

  const [showSourceSheet, setShowSourceSheet] = useState(false);
  const [showMeasurementForm, setShowMeasurementForm] = useState(false);

  const handleDeleteMeasurement = useCallback(
    (m: Measurement) => {
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

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: installation.type }} />
      <ScrollView contentContainerStyle={styles.scroll}>
        <InstallationInfoSection installation={installation} />

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

        <InstallationPhotoSection
          installationId={id}
          photos={photos ?? []}
          onAddPhoto={(params) => addPhoto.mutateAsync(params)}
          onDeletePhoto={(photoId) => deletePhoto.mutateAsync(photoId)}
          onSaveAnnotation={(photoId, annotation) => {
            void saveAnnotation({ id: photoId, annotation });
          }}
          showSourceSheet={showSourceSheet}
          onShowSourceSheet={setShowSourceSheet}
        />

        <InstallationMeasurementSection
          measurements={measurements ?? []}
          showForm={showMeasurementForm}
          submitting={addMeasurement.isPending}
          onSubmit={handleAddMeasurement}
          onCancel={() => setShowMeasurementForm(false)}
          onDelete={handleDeleteMeasurement}
        />
      </ScrollView>
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
});
