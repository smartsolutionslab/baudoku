import { PhotoGallery } from "./PhotoGallery";
import { PhotoCapture } from "./PhotoCapture";
import type { Photo } from "@baudoku/shared-types";

type PhotosTabProps = {
  photos: Photo[];
  onUpload: (file: File, type: string, caption?: string) => void;
  onDelete: (id: string) => void;
};

export function PhotosTab({ photos, onUpload, onDelete }: PhotosTabProps) {
  return (
    <div className="space-y-6">
      <PhotoCapture onCapture={onUpload} />
      <PhotoGallery photos={photos} onDelete={onDelete} />
    </div>
  );
}
