import React from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ActivityIndicator,
} from "react-native";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import * as photoRepo from "../../db/repositories/photoRepo";
import { useSyncContext } from "../../providers/SyncProvider";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";
import type { Photo } from "../../db/repositories/types";

export function UploadQueueCard() {
  const queryClient = useQueryClient();
  const { syncManager } = useSyncContext();
  const [uploading, setUploading] = React.useState(false);

  const { data: pendingPhotos = [] } = useQuery({
    queryKey: ["uploadQueue", "pending"],
    queryFn: () => photoRepo.getPendingUpload(),
  });

  const { data: failedPhotos = [] } = useQuery({
    queryKey: ["uploadQueue", "failed"],
    queryFn: () => photoRepo.getFailedUpload(),
  });

  const totalPending = pendingPhotos.length + failedPhotos.length;

  if (totalPending === 0) return null;

  const handleUploadAll = async () => {
    setUploading(true);
    try {
      await syncManager.pushPhotos();
    } finally {
      setUploading(false);
      queryClient.invalidateQueries({ queryKey: ["uploadQueue"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    }
  };

  const handleRetryOne = async (photo: Photo) => {
    await photoRepo.updateUploadStatus(photo.id, "pending");
    queryClient.invalidateQueries({ queryKey: ["uploadQueue"] });
    queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
  };

  return (
    <View style={styles.card}>
      <View style={styles.header}>
        <Text style={styles.title}>Foto-Uploads</Text>
        <View style={styles.badge}>
          <Text style={styles.badgeText}>{totalPending}</Text>
        </View>
      </View>

      {pendingPhotos.length > 0 && (
        <Text style={styles.subtitle}>
          {pendingPhotos.length} ausstehend
        </Text>
      )}

      {failedPhotos.length > 0 && (
        <>
          <Text style={[styles.subtitle, { color: Colors.danger }]}>
            {failedPhotos.length} fehlgeschlagen
          </Text>
          {failedPhotos.map((photo) => (
            <View key={photo.id} style={styles.failedItem}>
              <View style={styles.failedInfo}>
                <Text style={styles.failedName} numberOfLines={1}>
                  {photo.localPath.split("/").pop() ?? photo.id}
                </Text>
                {photo.lastUploadError && (
                  <Text style={styles.failedError} numberOfLines={2}>
                    {photo.lastUploadError}
                  </Text>
                )}
                {photo.retryCount != null && photo.retryCount > 0 && (
                  <Text style={styles.retryCount}>
                    Versuche: {photo.retryCount}
                  </Text>
                )}
              </View>
              <TouchableOpacity
                style={styles.retryButton}
                onPress={() => void handleRetryOne(photo)}
              >
                <Text style={styles.retryButtonText}>Erneut</Text>
              </TouchableOpacity>
            </View>
          ))}
        </>
      )}

      <TouchableOpacity
        style={[styles.uploadButton, uploading && styles.uploadButtonDisabled]}
        onPress={() => void handleUploadAll()}
        disabled={uploading}
      >
        {uploading ? (
          <ActivityIndicator color={Colors.white} size="small" />
        ) : (
          <Text style={styles.uploadButtonText}>Fotos hochladen</Text>
        )}
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginBottom: Spacing.lg,
    borderRadius: Radius.lg,
    padding: Spacing.lg,
    gap: Spacing.sm,
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  title: {
    fontSize: FontSize.headline,
    fontWeight: "600",
  },
  badge: {
    backgroundColor: Colors.warning,
    minWidth: 24,
    height: 24,
    borderRadius: 12,
    justifyContent: "center",
    alignItems: "center",
    paddingHorizontal: Spacing.xs,
  },
  badgeText: {
    color: Colors.white,
    fontSize: FontSize.footnote,
    fontWeight: "700",
  },
  subtitle: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
  },
  failedItem: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: Colors.background,
    borderRadius: Radius.sm,
    padding: Spacing.sm,
    gap: Spacing.sm,
  },
  failedInfo: {
    flex: 1,
    gap: 2,
  },
  failedName: {
    fontSize: FontSize.caption,
    fontWeight: "500",
  },
  failedError: {
    fontSize: FontSize.footnote,
    color: Colors.danger,
  },
  retryCount: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
  },
  retryButton: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    borderRadius: Radius.sm,
  },
  retryButtonText: {
    color: Colors.white,
    fontSize: FontSize.caption,
    fontWeight: "600",
  },
  uploadButton: {
    backgroundColor: Colors.primary,
    paddingVertical: Spacing.md,
    borderRadius: Radius.md,
    alignItems: "center",
    marginTop: Spacing.xs,
  },
  uploadButtonDisabled: {
    backgroundColor: Colors.disabled,
  },
  uploadButtonText: {
    color: Colors.white,
    fontSize: FontSize.callout,
    fontWeight: "600",
  },
});
