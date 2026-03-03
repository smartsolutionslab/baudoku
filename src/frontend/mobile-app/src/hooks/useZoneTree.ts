import { useMemo } from 'react';
import { buildZoneTree } from '@baudoku/projects';
import type { ZoneNode } from '@baudoku/projects';
import type { Zone } from '../db/repositories/types';

export type { ZoneNode };

export function useZoneTree(zones: Zone[] | undefined): ZoneNode[] {
  return useMemo(
    () => (zones ? buildZoneTree(zones as Parameters<typeof buildZoneTree>[0]) : []),
    [zones],
  );
}
