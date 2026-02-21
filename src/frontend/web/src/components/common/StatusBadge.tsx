import { PROJECT_STATUS_LABELS } from "@baudoku/projects";
import { INSTALLATION_STATUS_LABELS } from "@baudoku/documentation";

const statusStyles: Record<string, string> = {
  active: "bg-green-100 text-green-700",
  completed: "bg-blue-100 text-blue-700",
  archived: "bg-gray-100 text-gray-600",
  planned: "bg-yellow-100 text-yellow-700",
  in_progress: "bg-orange-100 text-orange-700",
  inspected: "bg-purple-100 text-purple-700",
  passed: "bg-green-100 text-green-700",
  failed: "bg-red-100 text-red-700",
  warning: "bg-yellow-100 text-yellow-700",
};

const allLabels: Record<string, string> = {
  ...PROJECT_STATUS_LABELS,
  ...INSTALLATION_STATUS_LABELS,
  passed: "Bestanden",
  failed: "Nicht bestanden",
  warning: "Warnung",
};

interface StatusBadgeProps {
  status: string;
  className?: string;
}

export function StatusBadge({ status, className = "" }: StatusBadgeProps) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${
        statusStyles[status] ?? "bg-gray-100 text-gray-600"
      } ${className}`}
    >
      {allLabels[status] ?? status}
    </span>
  );
}
