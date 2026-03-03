import { useRef, useState } from 'react';
import { CameraIcon } from '@/components/icons';
import { PHOTO_TYPE_LABELS } from '@baudoku/documentation';
import type { PhotoType } from '@baudoku/documentation';
import { optionsFromLabels } from '@baudoku/core';

type PhotoCaptureProps = {
  onCapture: (file: File, type: PhotoType, caption?: string) => void;
};

const typeOptions = optionsFromLabels(PHOTO_TYPE_LABELS);

export function PhotoCapture({ onCapture }: PhotoCaptureProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [photoType, setPhotoType] = useState<PhotoType>('detail');
  const [caption, setCaption] = useState('');

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      onCapture(file, photoType, caption || undefined);
      setCaption('');
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  return (
    <div className='space-y-3 rounded-xl border border-gray-200 bg-white p-4'>
      <div className='flex flex-wrap items-end gap-3'>
        <div>
          <label className='block text-sm font-medium text-gray-700'>
            Fototyp
          </label>
          <select
            value={photoType}
            onChange={(e) => setPhotoType(e.target.value as PhotoType)}
            className='mt-1 rounded-lg border border-gray-300 px-3 py-2 text-sm'
          >
            {typeOptions.map((opt) => (
              <option key={opt.value} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
        </div>
        <div className='flex-1'>
          <label className='block text-sm font-medium text-gray-700'>
            Beschriftung
          </label>
          <input
            type='text'
            value={caption}
            onChange={(e) => setCaption(e.target.value)}
            placeholder='Optional'
            className='mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm'
          />
        </div>
        <div className='flex gap-2'>
          <button
            type='button'
            onClick={() => fileInputRef.current?.click()}
            className='inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700'
          >
            <CameraIcon />
            Foto aufnehmen
          </button>
        </div>
      </div>
      <input
        ref={fileInputRef}
        type='file'
        accept='image/*'
        capture='environment'
        onChange={handleFileChange}
        className='hidden'
      />
    </div>
  );
}
