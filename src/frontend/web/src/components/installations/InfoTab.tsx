export function InfoTab({ installation }: { installation: any }) {
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
