import type { Measurement } from "@baudoku/shared-types";
import { StatusBadge } from "../common/StatusBadge";

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

function TrashIcon() {
  return (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
    </svg>
  );
}
