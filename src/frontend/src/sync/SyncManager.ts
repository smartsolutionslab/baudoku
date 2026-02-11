import * as syncRepo from "../db/repositories/syncRepo";
import * as photoRepo from "../db/repositories/photoRepo";
import * as syncApi from "./syncApi";
import { applyServerDelta } from "./applyServerDelta";
import { getDeviceId } from "../utils/deviceId";
import type { SyncDeltaDto, ProcessSyncBatchResult, ChangeSetResult } from "./syncApi";

export interface SyncResult {
  pushed: number;
  pulled: number;
  conflicts: number;
  errors: string[];
}

export class SyncManager {
  async push(): Promise<{
    appliedCount: number;
    conflictCount: number;
    errors: string[];
  }> {
    const pending = await syncRepo.getPendingEntries();
    const failed = await syncRepo.getFailedEntries();
    const entries = [...pending, ...failed];

    if (entries.length === 0) {
      return { appliedCount: 0, conflictCount: 0, errors: [] };
    }

    const deviceId = await getDeviceId();
    const ids = entries.map((e) => e.id);

    await syncRepo.markAsSyncing(ids);

    const deltas: SyncDeltaDto[] = entries.map((e) => ({
      entityType: e.entityType,
      entityId: e.entityId,
      operation: e.operation,
      baseVersion: 0,
      payload: e.payload,
      timestamp: e.timestamp instanceof Date
        ? e.timestamp.toISOString()
        : String(e.timestamp),
    }));

    try {
      const result: ProcessSyncBatchResult = await syncApi.pushBatch(
        deviceId,
        deltas
      );

      await syncRepo.markAsSynced(ids);

      return {
        appliedCount: result.appliedCount,
        conflictCount: result.conflictCount,
        errors: [],
      };
    } catch (error) {
      await syncRepo.markAsFailed(ids);
      const message =
        error instanceof Error ? error.message : "Unbekannter Fehler";
      return { appliedCount: 0, conflictCount: 0, errors: [message] };
    }
  }

  async pull(): Promise<{ pulled: number; errors: string[] }> {
    const deviceId = await getDeviceId();
    const lastSync = await syncRepo.getLastSyncTimestamp();

    try {
      const result: ChangeSetResult = await syncApi.pullChanges(
        deviceId,
        lastSync
      );

      for (const delta of result.changes) {
        await applyServerDelta(delta);
      }

      await syncRepo.setLastSyncTimestamp(result.serverTimestamp);

      return { pulled: result.changes.length, errors: [] };
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Unbekannter Fehler";
      return { pulled: 0, errors: [message] };
    }
  }

  async pushPhotos(): Promise<{ uploaded: number; errors: string[] }> {
    const pending = await photoRepo.getPendingUpload();
    const failed = await photoRepo.getFailedUpload();
    const photos = [...pending, ...failed];

    if (photos.length === 0) {
      return { uploaded: 0, errors: [] };
    }

    let uploaded = 0;
    const errors: string[] = [];

    for (const photo of photos) {
      try {
        await photoRepo.updateUploadStatus(photo.id, "uploading");

        const fileName = photo.localPath.split("/").pop() ?? `photo_${photo.id}.jpg`;
        const result = await syncApi.uploadPhoto(
          photo.installationId,
          photo.localPath,
          fileName,
          "image/jpeg"
        );

        await photoRepo.updateUploadStatus(photo.id, "uploaded", result.id);
        uploaded++;
      } catch (error) {
        await photoRepo.updateUploadStatus(photo.id, "failed");
        const message =
          error instanceof Error ? error.message : "Foto-Upload fehlgeschlagen";
        errors.push(message);
      }
    }

    return { uploaded, errors };
  }

  async sync(): Promise<SyncResult> {
    const pushResult = await this.push();
    const photoResult = await this.pushPhotos();
    const pullResult = await this.pull();

    return {
      pushed: pushResult.appliedCount + photoResult.uploaded,
      pulled: pullResult.pulled,
      conflicts: pushResult.conflictCount,
      errors: [
        ...pushResult.errors,
        ...photoResult.errors,
        ...pullResult.errors,
      ],
    };
  }
}
