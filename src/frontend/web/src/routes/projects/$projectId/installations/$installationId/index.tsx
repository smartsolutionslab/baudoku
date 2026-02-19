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
import { PhotoGallery } from "@/components/installations/PhotoGallery";
import { PhotoCapture } from "@/components/installations/PhotoCapture";
import { MeasurementCard } from "@/components/installations/MeasurementCard";
import { MeasurementForm } from "@/components/installations/MeasurementForm";
import type { PhotoType } from "@baudoku/shared-types";
import type { MeasurementFormData } from "@baudoku/shared-validation";

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
  const [showMeasurementForm, setShowMeasurementForm] = useState(false);

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
          <div className="space-y-6">
            <PhotoCapture
              onCapture={(file, type, caption) => {
                uploadPhoto.mutate({ file, caption });
              }}
            />
            <PhotoGallery
              photos={photos ?? []}
              onDelete={(id) => deletePhoto.mutate(id)}
            />
          </div>
        )}

        {activeTab === "measurements" && (
          <div className="space-y-4">
            {!showMeasurementForm ? (
              <button
                onClick={() => setShowMeasurementForm(true)}
                className="inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
              >
                <PlusIcon />
                Messung hinzufügen
              </button>
            ) : (
              <MeasurementForm
                onSubmit={async (data: MeasurementFormData) => {
                  await createMeasurement.mutateAsync(data);
                  setShowMeasurementForm(false);
                }}
                onCancel={() => setShowMeasurementForm(false)}
                isSubmitting={createMeasurement.isPending}
              />
            )}

            {measurements && measurements.length > 0 ? (
              <div className="space-y-3">
                {measurements.map((m) => (
                  <MeasurementCard
                    key={m.id}
                    measurement={m}
                    onDelete={(id) => deleteMeasurement.mutate(id)}
                  />
                ))}
              </div>
            ) : (
              !showMeasurementForm && (
                <p className="py-8 text-center text-sm text-gray-500">
                  Noch keine Messungen vorhanden.
                </p>
              )
            )}
          </div>
        )}
      </div>
    </div>
  );
}

function InfoTab({ installation }: { installation: any }) {
  const sections = [
    {
      title: "Komponente",
      fields: [
        { label: "Hersteller", value: installation.manufacturer },
        { label: "Modell", value: installation.model },
        { label: "Seriennummer", value: installation.serialNumber },
      ],
    },
    {
      title: "Kabel / Elektrisch",
      fields: [
        { label: "Kabeltyp", value: installation.cableType },
        {
          label: "Querschnitt",
          value: installation.crossSectionMm2
            ? `${installation.crossSectionMm2} mm²`
            : null,
        },
        {
          label: "Länge",
          value: installation.lengthM ? `${installation.lengthM} m` : null,
        },
        { label: "Stromkreis", value: installation.circuitId },
        { label: "Sicherung", value: installation.fuseType },
        {
          label: "Nennstrom",
          value: installation.fuseRatingA
            ? `${installation.fuseRatingA} A`
            : null,
        },
        {
          label: "Spannung",
          value: installation.voltageV
            ? `${installation.voltageV} V`
            : null,
        },
        { label: "Phase", value: installation.phase },
        {
          label: "Tiefe",
          value: installation.depthMm
            ? `${installation.depthMm} mm`
            : null,
        },
      ],
    },
    {
      title: "GPS-Position",
      fields: [
        {
          label: "Koordinaten",
          value:
            installation.gpsLat && installation.gpsLng
              ? `${installation.gpsLat.toFixed(6)}, ${installation.gpsLng.toFixed(6)}`
              : null,
        },
        {
          label: "Genauigkeit",
          value: installation.gpsAccuracy
            ? `±${installation.gpsAccuracy.toFixed(1)} m`
            : null,
        },
        { label: "Quelle", value: installation.gpsSource },
      ],
    },
  ];

  return (
    <div className="space-y-6">
      {sections.map((section) => {
        const filledFields = section.fields.filter((f) => f.value);
        if (filledFields.length === 0) return null;
        return (
          <div
            key={section.title}
            className="rounded-xl border border-gray-200 bg-white p-5"
          >
            <h3 className="text-sm font-semibold text-gray-900">
              {section.title}
            </h3>
            <dl className="mt-3 grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
              {filledFields.map((field) => (
                <div key={field.label}>
                  <dt className="text-xs text-gray-400">{field.label}</dt>
                  <dd className="mt-0.5 text-sm text-gray-900">
                    {field.value}
                  </dd>
                </div>
              ))}
            </dl>
          </div>
        );
      })}

      {installation.notes && (
        <div className="rounded-xl border border-gray-200 bg-white p-5">
          <h3 className="text-sm font-semibold text-gray-900">Notizen</h3>
          <p className="mt-2 whitespace-pre-wrap text-sm text-gray-600">
            {installation.notes}
          </p>
        </div>
      )}
    </div>
  );
}

function PlusIcon() {
  return (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
    </svg>
  );
}
