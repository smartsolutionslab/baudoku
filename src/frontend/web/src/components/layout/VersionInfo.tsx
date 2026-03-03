import { useState, useEffect } from 'react';

type ServiceInfo = {
  service: string;
  version: string;
  status?: string;
};

type SystemInfo = {
  gateway: ServiceInfo;
  services: ServiceInfo[];
};

function ServiceRow({
  label,
  version,
  status,
}: {
  label: string;
  version: string;
  status: string;
}) {
  return (
    <div className='flex items-center justify-between'>
      <span>{label}</span>
      <span className='flex items-center gap-1'>
        <span className={`inline-block h-1.5 w-1.5 rounded-full ${status === 'ok' ? 'bg-green-400' : 'bg-red-400'}`}
        />
        {version}
      </span>
    </div>
  );
}

export function VersionInfo() {
  const [expanded, setExpanded] = useState(false);
  const [systemInfo, setSystemInfo] = useState<SystemInfo | null>(null);

  useEffect(() => {
    if (!expanded || systemInfo) return;
    (async () => {
      try {
        const res = await fetch('/api/system/info');
        const data = res.ok ? await res.json() : null;
        setSystemInfo(data);
      } catch {
        // ignore fetch errors
      }
    })();
  }, [expanded, systemInfo]);

  return (
    <div className='border-t border-gray-200 px-4 py-2'>
      <button
        onClick={() => setExpanded(!expanded)}
        className='text-xs text-gray-400 hover:text-gray-600 transition-colors'
      >
        v{__APP_VERSION__}
      </button>
      {expanded && (
        <div className='mt-2 space-y-1 text-xs text-gray-500'>
          <p className='font-medium text-gray-600'>Services</p>
          {systemInfo ? (
            <>
              <ServiceRow
                label='Gateway'
                version={systemInfo.gateway.version}
                status='ok'
              />
              {systemInfo.services.map((s) => (
                <ServiceRow
                  key={s.service}
                  label={s.service.replace('BauDoku.', '').replace('.Api', '')}
                  version={s.version}
                  status={s.status ?? 'ok'}
                />
              ))}
            </>
          ) : (
            <p className='text-gray-400'>Laden...</p>
          )}
        </div>
      )}
    </div>
  );
}
