import * as ImagePicker from "expo-image-picker";
import { useCallback } from "react";
import { savePhoto } from "../utils/photoStorage";

export interface CapturedPhoto {
  localPath: string;
  width: number;
  height: number;
}

export interface UsePhotoCaptureReturn {
  takePhoto: () => Promise<CapturedPhoto | null>;
  pickFromGallery: () => Promise<CapturedPhoto | null>;
}

function processResult(
  result: ImagePicker.ImagePickerResult
): CapturedPhoto | null {
  if (result.canceled || result.assets.length === 0) return null;
  const asset = result.assets[0];
  const localPath = savePhoto(asset.uri);
  return {
    localPath,
    width: asset.width,
    height: asset.height,
  };
}

export function usePhotoCapture(): UsePhotoCaptureReturn {
  const takePhoto = useCallback(async (): Promise<CapturedPhoto | null> => {
    const permission = await ImagePicker.requestCameraPermissionsAsync();
    if (!permission.granted) return null;

    const result = await ImagePicker.launchCameraAsync({
      quality: 0.8,
      allowsEditing: false,
    });
    return processResult(result);
  }, []);

  const pickFromGallery = useCallback(
    async (): Promise<CapturedPhoto | null> => {
      const permission =
        await ImagePicker.requestMediaLibraryPermissionsAsync();
      if (!permission.granted) return null;

      const result = await ImagePicker.launchImageLibraryAsync({
        quality: 0.8,
        mediaTypes: ImagePicker.MediaTypeOptions.Images,
      });
      return processResult(result);
    },
    []
  );

  return { takePhoto, pickFromGallery };
}
