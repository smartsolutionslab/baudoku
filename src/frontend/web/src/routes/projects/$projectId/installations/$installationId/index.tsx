import { useState } from "react";
import { Link, useParams } from "@tanstack/react-router";
import {
  useInstallation,
  usePhotos,
  useMeasurements,
  useUploadPhoto,
  useDeletePhoto,
  useCreateMeasurement,
  useDeleteMeasurement,
} from "@/hooks/useInstallations";
import { StatusBadge } from "@/components/common/StatusBadge";
import { InfoTab } from "@/components/installations/InfoTab";
import { PhotosTab } from "@/components/installations/PhotosTab";
import { MeasurementsTab } from "@/components/installations/MeasurementsTab";

type Tab = "info" | "photos" | "measurements";

export function InstallationDetailPage() {
  const { projectId, installationId } = useParams({ strict: false }) as {
    projectId: string;
    installationId: string;
  };

  const { data: installation, isLoading } = useInstallation(installationId);
  const { data: photos } = usePhotos(installationId);
  const { data: measurements } = useMeasurements(installationId);

  const uploadPhoto = useUploadPhoto(installationId);
  const deletePhoto = useDeletePhoto(installationId);
  const createMeasurement = useCreateMeasurement(installationId);
  const deleteMeasurement = useDeleteMeasurement(installationId);

  const [activeTab, setActiveTab] = useState<Tab>("info");

  if (isLoading) {
    return (
      <div className="space-y-4">
        <div className="h-8 w-64 animate-pulse rounded bg-gray-200" />
        <div className="h-48 animate-pulse rounded-xl bg-gray-100" />
      </div>
    );
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

  const tabs: { key: Tab; label: string; count?: number }[] = [
    { key: "info", label: "Details" },
    { key: "photos", label: "Fotos", count: photos?.length },
    { key: "measurements", label: "Messungen", count: measurements?.length },
  ];

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
          <h1 className="text-2xl font-bold text-gray-900">
            {installation.type}
          </h1>
          <div className="mt-1 flex items-center gap-3">
            <StatusBadge status={installation.status} />
            {installation.manufacturer && (
              <span className="text-sm text-gray-500">
                {installation.manufacturer}
                {installation.model ? ` ${installation.model}` : ""}
              </span>
            )}
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="mt-6 border-b border-gray-200">
        <nav className="-mb-px flex gap-6">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`border-b-2 pb-3 text-sm font-medium transition-colors ${
                activeTab === tab.key
                  ? "border-blue-600 text-blue-600"
                  : "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700"
              }`}
            >
              {tab.label}
              {tab.count != null && (
                <span className="ml-1.5 rounded-full bg-gray-100 px-2 py-0.5 text-xs text-gray-600">
                  {tab.count}
                </span>
              )}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab content */}
      <div className="mt-6">
        {activeTab === "info" && <InfoTab installation={installation} />}

        {activeTab === "photos" && (
          <PhotosTab
            photos={photos ?? []}
            onUpload={(file, type, caption) => {
              uploadPhoto.mutate({ file, caption });
            }}
            onDelete={(id) => deletePhoto.mutate(id)}
          />
        )}

        {activeTab === "measurements" && (
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
