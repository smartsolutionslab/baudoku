import type { Zone } from "@baudoku/shared-types";

// ─── Zone Tree ──────────────────────────────────────────────────

export interface ZoneNode {
  zone: Zone;
  children: ZoneNode[];
  level: number;
}

export function buildZoneTree(zones: Zone[]): ZoneNode[] {
  const byParent = new Map<string | null, Zone[]>();
  for (const z of zones) {
    const key = z.parentZoneId ?? null;
    const list = byParent.get(key) ?? [];
    list.push(z);
    byParent.set(key, list);
  }

  function recurse(parentId: string | null, level: number): ZoneNode[] {
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

// ─── JWT Utilities ──────────────────────────────────────────────

export function parseJwtPayload(token: string): Record<string, unknown> {
  const parts = token.split(".");
  if (parts.length !== 3) throw new Error("Invalid JWT format");
  const payload = parts[1]!;
  const decoded = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
  return JSON.parse(decoded);
}

export function isTokenExpired(token: string, bufferSeconds = 30): boolean {
  try {
    const payload = parseJwtPayload(token);
    const exp = payload.exp as number | undefined;
    if (!exp) return true;
    return Date.now() / 1000 > exp - bufferSeconds;
  } catch {
    return true;
  }
}
