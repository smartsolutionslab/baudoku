interface FilterChipsProps {
  options: { value: string; label: string }[];
  selected: string | null;
  onChange: (value: string | null) => void;
}

export function FilterChips({ options, selected, onChange }: FilterChipsProps) {
  return (
    <div className="flex flex-wrap gap-2">
      <button
        onClick={() => onChange(null)}
        className={`rounded-full px-3 py-1 text-sm font-medium transition-colors ${
          selected === null
            ? "bg-blue-100 text-blue-700"
            : "bg-gray-100 text-gray-600 hover:bg-gray-200"
        }`}
      >
        Alle
      </button>
      {options.map((opt) => (
        <button
          key={opt.value}
          onClick={() =>
            onChange(selected === opt.value ? null : opt.value)
          }
          className={`rounded-full px-3 py-1 text-sm font-medium transition-colors ${
            selected === opt.value
              ? "bg-blue-100 text-blue-700"
              : "bg-gray-100 text-gray-600 hover:bg-gray-200"
          }`}
        >
          {opt.label}
        </button>
      ))}
    </div>
  );
}
