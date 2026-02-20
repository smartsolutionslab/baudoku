import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { zoneSchema, type ZoneFormData, ZONE_TYPE_LABELS, type Zone } from "@baudoku/projects";
import { FormField } from "../common/FormField";
import { FormSelect } from "../common/FormSelect";

interface ZoneFormProps {
  zones: Zone[];
  defaultValues?: Partial<ZoneFormData>;
  onSubmit: (data: ZoneFormData) => void | Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
}

const typeOptions = Object.entries(ZONE_TYPE_LABELS).map(([value, label]) => ({
  value,
  label,
}));

export function ZoneForm({
  zones,
  defaultValues,
  onSubmit,
  onCancel,
  isSubmitting,
}: ZoneFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ZoneFormData>({
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    resolver: zodResolver(zoneSchema) as any,
    defaultValues: {
      type: "room",
      sortOrder: 0,
      ...defaultValues,
    },
  });

  const parentOptions = [
    { value: "", label: "Keine (Oberste Ebene)" },
    ...zones.map((z) => ({
      value: z.id,
      label: `${z.name} (${ZONE_TYPE_LABELS[z.type as keyof typeof ZONE_TYPE_LABELS] ?? z.type})`,
    })),
  ];

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <div className="grid gap-4 sm:grid-cols-2">
          <div className="sm:col-span-2">
            <FormField
              label="Zonenname *"
              error={errors.name}
              register={register("name")}
              placeholder="z.B. Erdgeschoss"
            />
          </div>
          <FormSelect
            label="Typ *"
            error={errors.type}
            register={register("type")}
            options={typeOptions}
          />
          <FormField
            label="Sortierung"
            error={errors.sortOrder}
            register={register("sortOrder")}
            type="number"
            placeholder="0"
          />
          <div className="sm:col-span-2">
            <FormSelect
              label="Übergeordnete Zone"
              error={errors.parentZoneId}
              register={register("parentZoneId")}
              options={parentOptions}
            />
          </div>
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
          {isSubmitting ? "Speichert..." : "Zone erstellen"}
        </button>
      </div>
    </form>
  );
}
