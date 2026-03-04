import type { Installation } from '@baudoku/documentation';

type InfoTabProps = {
  installation: Installation;
};

export function InfoTab({ installation }: InfoTabProps) {
  const {
    manufacturer,
    model,
    serialNumber,
    cableType,
    crossSectionMm2,
    lengthM,
    circuitId,
    fuseType,
    fuseRatingA,
    voltageV,
    phase,
    depthMm,
    position,
    notes,
  } = installation;

  const sections = [
    {
      title: 'Komponente',
      fields: [
        { label: 'Hersteller', value: manufacturer },
        { label: 'Modell', value: model },
        { label: 'Seriennummer', value: serialNumber },
      ],
    },
    {
      title: 'Kabel / Elektrisch',
      fields: [
        { label: 'Kabeltyp', value: cableType },
        {
          label: 'Querschnitt',
          value: crossSectionMm2 ? `${crossSectionMm2} mm²` : null,
        },
        {
          label: 'Länge',
          value: lengthM ? `${lengthM} m` : null,
        },
        { label: 'Stromkreis', value: circuitId },
        { label: 'Sicherung', value: fuseType },
        {
          label: 'Nennstrom',
          value: fuseRatingA ? `${fuseRatingA} A` : null,
        },
        {
          label: 'Spannung',
          value: voltageV ? `${voltageV} V` : null,
        },
        { label: 'Phase', value: phase },
        {
          label: 'Tiefe',
          value: depthMm ? `${depthMm} mm` : null,
        },
      ],
    },
    {
      title: 'GPS-Position',
      fields: [
        {
          label: 'Koordinaten',
          value:
            position?.latitude && position?.longitude
              ? `${position.latitude.toFixed(6)}, ${position.longitude.toFixed(6)}`
              : null,
        },
        {
          label: 'Genauigkeit',
          value: position?.horizontalAccuracy
            ? `±${position.horizontalAccuracy.toFixed(1)} m`
            : null,
        },
        { label: 'Quelle', value: position?.gpsSource },
      ],
    },
  ];

  return (
    <div className="space-y-6">
      {sections.map(({ title, fields }) => {
        const filledFields = fields.filter(({ value }) => value);
        if (filledFields.length === 0) return null;
        return (
          <div key={title} className="rounded-xl border border-gray-200 bg-white p-5">
            <h3 className="text-sm font-semibold text-gray-900">{title}</h3>
            <dl className="mt-3 grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
              {filledFields.map(({ label, value }) => (
                <div key={label}>
                  <dt className="text-xs text-gray-400">{label}</dt>
                  <dd className="mt-0.5 text-sm text-gray-900">{value}</dd>
                </div>
              ))}
            </dl>
          </div>
        );
      })}

      {notes && (
        <div className="rounded-xl border border-gray-200 bg-white p-5">
          <h3 className="text-sm font-semibold text-gray-900">Notizen</h3>
          <p className="mt-2 whitespace-pre-wrap text-sm text-gray-600">{notes}</p>
        </div>
      )}
    </div>
  );
}
