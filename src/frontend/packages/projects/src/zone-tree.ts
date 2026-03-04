import type { ZoneId } from '@baudoku/core';
import type { Zone } from './types';

export type ZoneNode = {
  zone: Zone;
  children: ZoneNode[];
  level: number;
};

export function buildZoneTree(zones: Zone[]): ZoneNode[] {
  const byParent = new Map<ZoneId | null, Zone[]>();
  for (const z of zones) {
    const key = z.parentZoneId ?? null;
    const list = byParent.get(key) ?? [];
    list.push(z);
    byParent.set(key, list);
  }

  function recurse(parentId: ZoneId | null, level: number): ZoneNode[] {
    const children = byParent.get(parentId) ?? [];
    children.sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0));
    return children.map((zone) => ({
      zone,
      children: recurse(zone.id, level + 1),
      level,
    }));
  }

  return recurse(null, 0);
}

export function flattenTree(nodes: ZoneNode[]): ZoneNode[] {
  const result: ZoneNode[] = [];
  function walk(list: ZoneNode[]) {
    for (const node of list) {
      result.push(node);
      walk(node.children);
    }
  }
  walk(nodes);
  return result;
}
