import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import {
  installationSchema,
  type InstallationFormData,
} from "@baudoku/shared-validation";
import {
  INSTALLATION_STATUS_LABELS,
  INSTALLATION_TYPES,
  ZONE_TYPE_LABELS,
} from "@baudoku/shared-constants";
import { FormField } from "../common/FormField";
import { FormSelect } from "../common/FormSelect";
import type { Zone } from "@baudoku/shared-types";

interface InstallationFormProps {
  zones: Zone[];
  defaultValues?: Partial<InstallationFormData & { zoneId: string }>;
  onSubmit: (data: InstallationFormData & { zoneId: string }) => void | Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
}

const statusOptions = Object.entries(INSTALLATION_STATUS_LABELS).map(
  ([value, label]) => ({ value, label })
);

const typeOptions = INSTALLATION_TYPES.map((t) => ({ value: t, label: t }));

export function InstallationForm({
  zones,
  defaultValues,
  onSubmit,
  onCancel,
  isSubmitting,
}: InstallationFormProps) {
  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<InstallationFormData & { zoneId: string }>({
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    resolver: zodResolver(
      installationSchema.extend({
        zoneId: z.string().min(1, "Zone erforderlich"),
      })
    ) as any,
    defaultValues: {
      status: "planned",
      zoneId: "",
      ...defaultValues,
    },
  });

  const zoneOptions = zones.map((z) => ({
    value: z.id,
    label: `${z.name} (${ZONE_TYPE_LABELS[z.type as keyof typeof ZONE_TYPE_LABELS] ?? z.type})`,
  }));

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <h2 className="text-base font-semibold text-gray-900">Grunddaten</h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2">
          <FormSelect
            label="Typ *"
            error={errors.type}
            register={register("type")}
            options={typeOptions}
            placeholder="Typ auswählen"
          />
          <FormSelect
            label="Status"
            error={errors.status}
            register={register("status")}
            options={statusOptions}
          />
          <div className="sm:col-span-2">
            <FormSelect
              label="Zone *"
              error={(errors as any).zoneId}
              register={register("zoneId")}
              options={zoneOptions}
              placeholder="Zone auswählen"
            />
          </div>
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <h2 className="text-base font-semibold text-gray-900">Komponente</h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2">
          <FormField
            label="Hersteller"
            error={errors.manufacturer}
            register={register("manufacturer")}
          />
          <FormField
            label="Modell"
            error={errors.model}
            register={register("model")}
          />
          <FormField
            label="Seriennummer"
            error={errors.serialNumber}
            register={register("serialNumber")}
          />
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <h2 className="text-base font-semibold text-gray-900">
          Kabel / Elektrisch
        </h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-3">
          <FormField
            label="Kabeltyp"
            error={errors.cableType}
            register={register("cableType")}
            placeholder="z.B. NYM-J"
          />
          <FormField
            label="Querschnitt (mm²)"
            error={errors.crossSectionMm2}
            register={register("crossSectionMm2")}
            type="number"
            step="0.1"
          />
          <FormField
            label="Länge (m)"
            error={errors.lengthM}
            register={register("lengthM")}
            type="number"
            step="0.1"
          />
          <FormField
            label="Stromkreis-ID"
            error={errors.circuitId}
            register={register("circuitId")}
          />
          <FormField
            label="Sicherungstyp"
            error={errors.fuseType}
            register={register("fuseType")}
          />
          <FormField
            label="Nennstrom (A)"
            error={errors.fuseRatingA}
            register={register("fuseRatingA")}
            type="number"
          />
          <FormField
            label="Spannung (V)"
            error={errors.voltageV}
            register={register("voltageV")}
            type="number"
          />
          <FormSelect
            label="Phase"
            error={errors.phase}
            register={register("phase")}
            options={[
              { value: "L1", label: "L1" },
              { value: "L2", label: "L2" },
              { value: "L3", label: "L3" },
              { value: "N", label: "N" },
              { value: "PE", label: "PE" },
            ]}
            placeholder="—"
          />
          <FormField
            label="Tiefe (mm)"
            error={errors.depthMm}
            register={register("depthMm")}
            type="number"
          />
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <h2 className="text-base font-semibold text-gray-900">Notizen</h2>
        <div className="mt-4">
          <textarea
            {...register("notes")}
            rows={3}
            className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="Zusätzliche Bemerkungen..."
          />
        </div>
      </div>

      <div className="flex justify-end gap-3">
        <button
          type="button"
          onClick={onCancel}
          className="rounded-lg border border-gray-300 px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50"
        >
          Abbrechen
        </button>
        <button
          type="submit"
          disabled={isSubmitting}
          className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
        >
          {isSubmitting ? "Speichert..." : "Installation erstellen"}
        </button>
      </div>
    </form>
  );
}
