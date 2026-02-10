import { eq } from "drizzle-orm";
import { db } from "../client";
import { measurements } from "../schema";
import { generateId } from "../../utils/uuid";
import { createOutboxEntry } from "./syncRepo";
import type { Measurement, NewMeasurement } from "./types";

function evaluateResult(
  value: number,
  minThreshold: number | null,
  maxThreshold: number | null
): "passed" | "failed" | "warning" | null {
  if (minThreshold === null && maxThreshold === null) return null;
  if (minThreshold !== null && value < minThreshold) return "failed";
  if (maxThreshold !== null && value > maxThreshold) return "failed";
  return "passed";
}

export async function getByInstallationId(
  installationId: string
): Promise<Measurement[]> {
  return db
    .select()
    .from(measurements)
    .where(eq(measurements.installationId, installationId))
    .all();
}

export async function getById(
  id: string
): Promise<Measurement | undefined> {
  return db
    .select()
    .from(measurements)
    .where(eq(measurements.id, id))
    .get();
}

export async function create(
  data: Omit<NewMeasurement, "id" | "version" | "result">
): Promise<Measurement> {
  const result = evaluateResult(
    data.value,
    data.minThreshold ?? null,
    data.maxThreshold ?? null
  );

  const measurement: NewMeasurement = {
    ...data,
    id: generateId(),
    result,
    version: 1,
  };

  await db.insert(measurements).values(measurement);
  await createOutboxEntry(
    "measurement",
    measurement.id,
    "create",
    measurement
  );

  return measurement as Measurement;
}

export async function update(
  id: string,
  data: Partial<
    Omit<NewMeasurement, "id" | "version" | "installationId" | "result">
  >
): Promise<Measurement | undefined> {
  const existing = await getById(id);
  if (!existing) return undefined;

  const newValue = data.value ?? existing.value;
  const newMin = data.minThreshold !== undefined ? data.minThreshold : existing.minThreshold;
  const newMax = data.maxThreshold !== undefined ? data.maxThreshold : existing.maxThreshold;
  const result = evaluateResult(newValue, newMin ?? null, newMax ?? null);

  const updated = {
    ...data,
    result,
    version: existing.version + 1,
  };

  await db
    .update(measurements)
    .set(updated)
    .where(eq(measurements.id, id));
  await createOutboxEntry("measurement", id, "update", {
    ...existing,
    ...updated,
  });

  return { ...existing, ...updated } as Measurement;
}

export async function remove(id: string): Promise<void> {
  await db.delete(measurements).where(eq(measurements.id, id));
  await createOutboxEntry("measurement", id, "delete", { id });
}
