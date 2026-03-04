import { useState } from 'react';
import { Link, useParams } from '@tanstack/react-router';
import { projectId as toProjectId, installationId as toInstallationId } from '@baudoku/core';
import {
  useInstallation,
  usePhotos,
  useMeasurements,
  useUploadPhoto,
  useDeletePhoto,
  useCreateMeasurement,
  useDeleteMeasurement,
} from '@/hooks';
import { StatusBadge, LoadingSkeleton } from '@/components/common';
import { InfoTab, PhotosTab, MeasurementsTab } from '@/components/installations';

type Tab = 'info' | 'photos' | 'measurements';

const TAB_DEFS: { key: Tab; label: string }[] = [
  { key: 'info', label: 'Details' },
  { key: 'photos', label: 'Fotos' },
  { key: 'measurements', label: 'Messungen' },
];

export function InstallationDetailPage() {
  const { projectId: rawProjectId, installationId: rawInstallationId } = useParams({
    strict: false,
  }) as {
    projectId: string;
    installationId: string;
  };
  const projectId = toProjectId(rawProjectId);
  const installationId = toInstallationId(rawInstallationId);

  const { data: installation, isLoading } = useInstallation(installationId);
  const { data: photos } = usePhotos(installationId);
  const { data: measurements } = useMeasurements(installationId);

  const uploadPhoto = useUploadPhoto(installationId);
  const deletePhoto = useDeletePhoto(installationId);
  const createMeasurement = useCreateMeasurement(installationId);
  const deleteMeasurement = useDeleteMeasurement(installationId);

  const [activeTab, setActiveTab] = useState<Tab>('info');

  if (isLoading) {
    return <LoadingSkeleton count={1} itemClassName="h-48" layout="list" />;
  }

  if (!installation) {
    return (
      <div className="py-12 text-center">
        <p className="text-gray-500">Installation nicht gefunden.</p>
        <Link
          to="/projects/$projectId/installations"
          params={{ projectId }}
          className="mt-2 text-sm text-blue-600 hover:underline"
        >
          Zurück zur Liste
        </Link>
      </div>
    );
  }

  const countByTab: Record<Tab, number | undefined> = {
    info: undefined,
    photos: photos?.length,
    measurements: measurements?.length,
  };

  return (
    <div>
      {/* Header */}
      <Link
        to="/projects/$projectId/installations"
        params={{ projectId }}
        className="text-sm text-gray-500 hover:text-gray-700"
      >
        &larr; Alle Installationen
      </Link>
      <div className="mt-2 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{installation.type}</h1>
          <div className="mt-1 flex items-center gap-3">
            <StatusBadge status={installation.status} />
            {installation.manufacturer && (
              <span className="text-sm text-gray-500">
                {installation.manufacturer}
                {installation.model ? ` ${installation.model}` : ''}
              </span>
            )}
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="mt-6 border-b border-gray-200">
        <nav className="-mb-px flex gap-6">
          {TAB_DEFS.map(({ key, label }) => (
            <button
              key={key}
              onClick={() => setActiveTab(key)}
              className={`border-b-2 pb-3 text-sm font-medium transition-colors ${
                activeTab === key
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'
              }`}
            >
              {label}
              {countByTab[key] != null && (
                <span className="ml-1.5 rounded-full bg-gray-100 px-2 py-0.5 text-xs text-gray-600">
                  {countByTab[key]}
                </span>
              )}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab content */}
      <div className="mt-6">
        {activeTab === 'info' && <InfoTab installation={installation} />}

        {activeTab === 'photos' && (
          <PhotosTab
            photos={photos ?? []}
            onUpload={(file, type, caption) => {
              uploadPhoto.mutate({ file, caption });
            }}
            onDelete={(id) => deletePhoto.mutate(id)}
          />
        )}

        {activeTab === 'measurements' && (
          <MeasurementsTab
            measurements={measurements ?? []}
            onCreateMeasurement={async (data) => {
              await createMeasurement.mutateAsync(data);
            }}
            onDeleteMeasurement={(id) => deleteMeasurement.mutate(id)}
            isCreating={createMeasurement.isPending}
          />
        )}
      </div>
    </div>
  );
}
