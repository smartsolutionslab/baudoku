import { useState } from "react";
import type { Photo } from "@baudoku/documentation";
import { PHOTO_TYPE_LABELS } from "@baudoku/documentation";
import { getBaseUrl } from "@baudoku/core";

interface PhotoGalleryProps {
  photos: Photo[];
  onDelete?: (photoId: string) => void;
}

export function PhotoGallery({ photos, onDelete }: PhotoGalleryProps) {
  const [viewPhoto, setViewPhoto] = useState<Photo | null>(null);

  if (photos.length === 0) {
    return (
      <p className="py-8 text-center text-sm text-gray-500">
        Noch keine Fotos vorhanden.
      </p>
    );
  }

  return (
    <>
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-4">
        {photos.map((photo) => (
          <div
            key={photo.id}
            className="group relative aspect-square cursor-pointer overflow-hidden rounded-lg bg-gray-100"
            onClick={() => setViewPhoto(photo)}
          >
            <img
              src={
                photo.remotePath
                  ? `${getBaseUrl()}${photo.remotePath}`
                  : photo.thumbnailPath
                    ? `${getBaseUrl()}${photo.thumbnailPath}`
                    : undefined
              }
              alt={photo.caption ?? "Foto"}
              className="h-full w-full object-cover"
            />
            <div className="absolute inset-x-0 bottom-0 bg-gradient-to-t from-black/60 to-transparent p-2">
              <span className="text-xs font-medium text-white">
                {PHOTO_TYPE_LABELS[photo.type as keyof typeof PHOTO_TYPE_LABELS] ?? photo.type}
              </span>
            </div>
            {onDelete && (
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  onDelete(photo.id);
                }}
                className="absolute right-2 top-2 rounded-full bg-black/40 p-1 text-white opacity-0 hover:bg-red-600 group-hover:opacity-100 transition-opacity"
              >
                <XIcon />
              </button>
            )}
          </div>
        ))}
      </div>

      {/* Lightbox */}
      {viewPhoto && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/80 p-4"
          onClick={() => setViewPhoto(null)}
        >
          <button
            className="absolute right-4 top-4 rounded-full bg-black/40 p-2 text-white hover:bg-black/60"
            onClick={() => setViewPhoto(null)}
          >
            <XIcon />
          </button>
          <img
            src={
              viewPhoto.remotePath
                ? `${getBaseUrl()}${viewPhoto.remotePath}`
                : undefined
            }
            alt={viewPhoto.caption ?? "Foto"}
            className="max-h-[90vh] max-w-[90vw] rounded-lg object-contain"
            onClick={(e) => e.stopPropagation()}
          />
          {viewPhoto.caption && (
            <p className="absolute bottom-6 text-center text-sm text-white">
              {viewPhoto.caption}
            </p>
          )}
        </div>
      )}
    </>
  );
}

function XIcon() {
  return (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
    </svg>
  );
}
