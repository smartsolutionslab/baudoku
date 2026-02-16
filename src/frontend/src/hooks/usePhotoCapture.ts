import * as ImagePicker from "expo-image-picker";
import { useCallback } from "react";
import { savePhoto } from "../utils";

export type ExifData = {
  gpsLatitude?: number;
  gpsLongitude?: number;
  dateTime?: string;
  cameraModel?: string;
};

export type CapturedPhoto = {
  localPath: string;
  width: number;
  height: number;
  exif?: ExifData;
};

export type UsePhotoCaptureReturn = {
  takePhoto: () => Promise<CapturedPhoto | null>;
  pickFromGallery: () => Promise<CapturedPhoto | null>;
};

function extractExif(exif: Record<string, unknown> | null | undefined): ExifData | undefined {
  if (!exif) return undefined;

  const data: ExifData = {};

  if (typeof exif.GPSLatitude === "number") data.gpsLatitude = exif.GPSLatitude;
  if (typeof exif.GPSLongitude === "number")
    data.gpsLongitude = exif.GPSLongitude;

  if (typeof exif.DateTimeOriginal === "string")
    data.dateTime = exif.DateTimeOriginal;
  else if (typeof exif.DateTime === "string") data.dateTime = exif.DateTime;

  const model = exif.Model ?? exif.model;
  if (typeof model === "string") data.cameraModel = model;

  if (!data.gpsLatitude && !data.gpsLongitude && !data.dateTime && !data.cameraModel)
    return undefined;

  return data;
}

function processResult(result: ImagePicker.ImagePickerResult): CapturedPhoto | null {
  if (result.canceled || result.assets.length === 0) return null;
  const asset = result.assets[0];
  const localPath = savePhoto(asset.uri);

  const exif = extractExif(
    asset.exif as Record<string, unknown> | null | undefined
  );

  return {
    localPath,
    width: asset.width,
    height: asset.height,
    exif,
  };
}

export function usePhotoCapture(): UsePhotoCaptureReturn {
  const takePhoto = useCallback(async (): Promise<CapturedPhoto | null> => {
    const permission = await ImagePicker.requestCameraPermissionsAsync();
    if (!permission.granted) return null;

    const result = await ImagePicker.launchCameraAsync({
      quality: 0.8,
      allowsEditing: false,
      exif: true,
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
        exif: true,
      });
      return processResult(result);
    },
    []
  );

  return { takePhoto, pickFromGallery };
}
