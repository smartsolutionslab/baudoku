import { File } from "expo-file-system";
import { initChunkedUpload, uploadChunk, completeChunkedUpload } from "./syncApi";

export const CHUNK_SIZE_BYTES = 1_048_576; // 1 MB

export interface UploadProgress {
  chunksUploaded: number;
  totalChunks: number;
  percentage: number;
}

// Base64 encodes 3 bytes as 4 chars, so 1 MB binary = ceil(1MB * 4/3) base64 chars
const CHUNK_SIZE_BASE64 = Math.ceil(CHUNK_SIZE_BYTES * 4 / 3);

export async function uploadPhotoChunked(
  photoLocalId: string,
  installationId: string,
  localPath: string,
  fileName: string,
  contentType: string,
  photoType: string,
  caption?: string,
  onProgress?: (progress: UploadProgress) => void
): Promise<string> {
  // Read entire file as base64 using new expo-file-system v19 API
  const file = new File(localPath);
  const base64 = await file.base64();

  const totalChunks = Math.ceil(base64.length / CHUNK_SIZE_BASE64);
  // Approximate original binary size from base64 length
  const totalSize = Math.ceil(base64.length * 3 / 4);

  // Init session on server
  const { sessionId } = await initChunkedUpload(
    installationId,
    fileName,
    contentType,
    totalSize,
    totalChunks,
    photoType,
    caption
  );

  // Upload each chunk as binary via the syncApi uploadChunk function
  for (let i = 0; i < totalChunks; i++) {
    const chunkBase64 = base64.slice(i * CHUNK_SIZE_BASE64, (i + 1) * CHUNK_SIZE_BASE64);

    // Decode base64 to binary bytes for upload
    const binaryString = atob(chunkBase64);
    const bytes = new Uint8Array(binaryString.length);
    for (let j = 0; j < binaryString.length; j++) {
      bytes[j] = binaryString.charCodeAt(j);
    }

    await uploadChunk(sessionId, i, bytes.buffer);

    onProgress?.({
      chunksUploaded: i + 1,
      totalChunks,
      percentage: Math.round(((i + 1) / totalChunks) * 100),
    });
  }

  // Complete the upload
  const result = await completeChunkedUpload(sessionId);
  return result.id;
}
