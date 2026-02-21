import { apiUpload } from "@baudoku/core";
import type { PhotoUploadResult } from "@baudoku/core";

export async function uploadPhoto(
  installationId: string,
  file: File,
  caption?: string
): Promise<PhotoUploadResult> {
  const formData = new FormData();
  formData.append("file", file);
  if (caption) formData.append("caption", caption);

  return apiUpload<PhotoUploadResult>(
    `/api/documentation/installations/${installationId}/photos`,
    formData
  );
}
