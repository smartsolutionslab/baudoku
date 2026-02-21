import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { projectSchema, type ProjectFormData, PROJECT_STATUS_LABELS } from "@baudoku/projects";
import { FormField } from "../common/FormField";
import { FormSelect } from "../common/FormSelect";
import { useNavigate } from "@tanstack/react-router";

interface ProjectFormProps {
  defaultValues?: Partial<ProjectFormData>;
  onSubmit: (data: ProjectFormData) => void | Promise<void>;
  isSubmitting?: boolean;
}

const statusOptions = Object.entries(PROJECT_STATUS_LABELS).map(
  ([value, label]) => ({ value, label })
);

export function ProjectForm({
  defaultValues,
  onSubmit,
  isSubmitting,
}: ProjectFormProps) {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ProjectFormData>({
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    resolver: zodResolver(projectSchema) as any,
    defaultValues: {
      status: "active",
      ...defaultValues,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <h2 className="text-base font-semibold text-gray-900">
          Projektdetails
        </h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2">
          <div className="sm:col-span-2">
            <FormField
              label="Projektname *"
              error={errors.name}
              register={register("name")}
              placeholder="z.B. Neubau Bürogebäude"
            />
          </div>
          <FormSelect
            label="Status"
            error={errors.status}
            register={register("status")}
            options={statusOptions}
          />
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <h2 className="text-base font-semibold text-gray-900">Adresse</h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2">
          <div className="sm:col-span-2">
            <FormField
              label="Straße"
              error={errors.street}
              register={register("street")}
              placeholder="Musterstraße 1"
            />
          </div>
          <FormField
            label="PLZ"
            error={errors.zipCode}
            register={register("zipCode")}
            placeholder="12345"
          />
          <FormField
            label="Stadt"
            error={errors.city}
            register={register("city")}
            placeholder="Berlin"
          />
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-6">
        <h2 className="text-base font-semibold text-gray-900">Auftraggeber</h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2">
          <FormField
            label="Name"
            error={errors.clientName}
            register={register("clientName")}
            placeholder="Firma GmbH"
          />
          <FormField
            label="Kontakt"
            error={errors.clientContact}
            register={register("clientContact")}
            placeholder="Max Mustermann"
          />
        </div>
      </div>

      <div className="flex justify-end gap-3">
        <button
          type="button"
          onClick={() => navigate({ to: "/projects" })}
          className="rounded-lg border border-gray-300 px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50"
        >
          Abbrechen
        </button>
        <button
          type="submit"
          disabled={isSubmitting}
          className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
        >
          {isSubmitting ? "Speichert..." : "Projekt erstellen"}
        </button>
      </div>
    </form>
  );
}
