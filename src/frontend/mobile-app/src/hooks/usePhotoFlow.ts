import { useState, useCallback } from 'react';
import type { InstallationId, PhotoId, Latitude, Longitude } from '@baudoku/core';
import type { Photo } from '@/db/repositories/types';
import type { PhotoType } from '@/components/installations';
import { usePhotoCapture, type CapturedPhoto } from './usePhotoCapture';
import { useConfirmDelete } from './useConfirmDelete';
import { deletePhotoFile } from '@/utils';

type OnAddPhotoParams = {
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
};

type UsePhotoFlowParams = {
  installationId: InstallationId;
  onAddPhoto: (params: OnAddPhotoParams) => Promise<unknown>;
  onDeletePhoto: (id: PhotoId) => Promise<unknown>;
  onShowSourceSheet: (show: boolean) => void;
};

type UsePhotoFlowReturn = {
  handleCameraCapture: () => Promise<void>;
  handleGalleryPick: () => Promise<void>;
  showTypeSheet: boolean;
  onCloseTypeSheet: () => void;
  handlePhotoTypeSelect: (type: PhotoType) => void;
  showCaptionModal: boolean;
  onCloseCaptionModal: () => void;
  captionText: string;
  setCaptionText: (text: string) => void;
  handleCaptionConfirm: () => Promise<void>;
  viewerPhoto: Photo | null;
  showViewer: boolean;
  openViewer: (photo: Photo) => void;
  closeViewer: () => void;
  handleDeletePhoto: (photo: Photo) => void;
};

export function usePhotoFlow({
  installationId,
  onAddPhoto,
  onDeletePhoto,
  onShowSourceSheet,
}: UsePhotoFlowParams): UsePhotoFlowReturn {
  const { takePhoto, pickFromGallery } = usePhotoCapture();
  const { confirmDelete } = useConfirmDelete();

  const [showTypeSheet, setShowTypeSheet] = useState(false);
  const [showCaptionModal, setShowCaptionModal] = useState(false);
  const [pendingPhoto, setPendingPhoto] = useState<CapturedPhoto | null>(null);
  const [pendingPhotoType, setPendingPhotoType] = useState<PhotoType | null>(null);
  const [captionText, setCaptionText] = useState('');
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
    setCaptionText('');
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
        uploadStatus: 'pending',
      });
    } catch {
      // Global MutationCache.onError shows toast
    }
    setPendingPhoto(null);
    setPendingPhotoType(null);
    setCaptionText('');
  }, [pendingPhoto, pendingPhotoType, captionText, installationId, onAddPhoto]);

  const onCloseTypeSheet = useCallback(() => {
    setShowTypeSheet(false);
    setPendingPhoto(null);
  }, []);

  const onCloseCaptionModal = useCallback(() => {
    setShowCaptionModal(false);
    setPendingPhoto(null);
    setPendingPhotoType(null);
  }, []);

  const openViewer = useCallback((photo: Photo) => {
    setViewerPhoto(photo);
    setShowViewer(true);
  }, []);

  const closeViewer = useCallback(() => {
    setShowViewer(false);
  }, []);

  const handleDeletePhoto = useCallback(
    (photo: Photo) => {
      setShowViewer(false);
      confirmDelete({
        title: 'Foto löschen',
        message: 'Dieses Foto wirklich löschen?',
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
    [onDeletePhoto, confirmDelete],
  );

  return {
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
  };
}
