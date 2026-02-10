import { Paths, Directory, File } from "expo-file-system";
import { generateId } from "./uuid";

const PHOTO_DIR = new Directory(Paths.document, "photos");

export function ensurePhotoDir(): void {
  if (!PHOTO_DIR.exists) {
    PHOTO_DIR.create();
  }
}

export function savePhoto(tempUri: string): string {
  ensurePhotoDir();
  const filename = `${generateId()}.jpg`;
  const destFile = new File(PHOTO_DIR, filename);
  const sourceFile = new File(tempUri);
  sourceFile.copy(destFile);
  return destFile.uri;
}

export function deletePhotoFile(path: string): void {
  const file = new File(path);
  if (file.exists) {
    file.delete();
  }
}
