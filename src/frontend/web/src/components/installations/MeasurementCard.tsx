import type { Measurement } from "@baudoku/shared-types";
import { StatusBadge } from "../common/StatusBadge";
import { TrashIcon } from "@/components/icons";

interface MeasurementCardProps {
  measurement: Measurement;
  onDelete?: (id: string) => void;
}

export function MeasurementCard({ measurement, onDelete }: MeasurementCardProps) {
  return (
    <div className="group flex items-center gap-4 rounded-lg border border-gray-200 bg-white p-4">
      <div className="flex-1 min-w-0">
        <div className="flex items-center gap-2">
          <span className="font-medium text-gray-900">
            {measurement.type}
          </span>
          {measurement.result && (
            <StatusBadge status={measurement.result} />
          )}
        </div>
        <div className="mt-1 flex items-baseline gap-1">
          <span className="text-2xl font-bold text-gray-900">
            {measurement.value}
          </span>
          <span className="text-sm text-gray-500">{measurement.unit}</span>
        </div>
        <div className="mt-1 flex flex-wrap gap-3 text-xs text-gray-400">
          {measurement.minThreshold != null && (
            <span>Min: {measurement.minThreshold} {measurement.unit}</span>
          )}
          {measurement.maxThreshold != null && (
            <span>Max: {measurement.maxThreshold} {measurement.unit}</span>
          )}
          {measurement.measuredBy && (
            <span>Prüfer: {measurement.measuredBy}</span>
          )}
        </div>
        {measurement.notes && (
          <p className="mt-1 text-sm text-gray-500">{measurement.notes}</p>
        )}
      </div>
      {onDelete && (
        <button
          onClick={() => onDelete(measurement.id)}
          className="rounded p-1 text-gray-300 opacity-0 hover:text-red-500 group-hover:opacity-100 transition-opacity"
          title="Löschen"
        >
          <TrashIcon />
        </button>
      )}
    </div>
  );
}

